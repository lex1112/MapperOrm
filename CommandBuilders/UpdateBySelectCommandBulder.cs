using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MapperOrm.Helpers;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.CommandBuilders
{
    internal static class UpdateBySelectCommandBulder
    {
        private static readonly IReflectionWrapper ReflectionWrapper = new CacheReflectionWrapper();
        private static readonly Dictionary<Type, DbType> TypeMap = NetTypesToDbTypesMapper.Instance;

        public static string Create(IDbCommand command, Dictionary<BinaryExpression, EntityStruct> objs)
        {
            var result = new StringBuilder();
            foreach (var obj in objs)
            {
                result.Append(@CreateByType(command, obj.Key, obj.Value));
            }
            return result.ToString();
        }


        public static string CreateByType(IDbCommand command, BinaryExpression exp, EntityStruct obj )
        {
            var type = obj.Key;
            var tableName = CommonCommandBuilder.GetTableName(type);
            var cmdBulder = new StringBuilder(string.Format(@" UPDATE [{0}].[{1}].[{2}] ", command.Connection.Database, CommonCommandBuilder.DbSchemaName, tableName));
            var nameValuePairs = CommonCommandBuilder.FieldByValueClauseBuilder(type, obj.Value);
            var parameterizatedNames = nameValuePairs.Select(
                x => 
                    new KeyValuePair<string, string>(x.Key, CommonCommandBuilder.GetParamsFormat(x.Key, x.Value, command.Connection.Database))
);
            foreach (var pair in nameValuePairs)
            {
                var prop = CommonCommandBuilder.GetPropertyByAttrName(type, pair.Key);
                var parameter = command.CreateParameter();
                parameter.ParameterName = CommonCommandBuilder.GetParamsFormat(pair.Key, pair.Value, command.Connection.Database);
                parameter.DbType = TypeMap[prop.PropertyType];
                parameter.Value = pair.Value;
                if (!command.Parameters.Contains(parameter.ParameterName))
                    command.Parameters.Add(parameter);
            }
            var updateClause = string.Join(",", parameterizatedNames.Select(x => string.Format(" [{0}].[{1}] = {2}", tableName, x.Key, x.Value)));
            cmdBulder.Append(string.Format(" SET {0} WHERE {1}", updateClause.Trim(','),
                                           CommonCommandBuilder.BuildClauseByExpression(command, type, exp)));
            return cmdBulder.ToString();
        }
    }
}