using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MapperOrm.Helpers;
using MapperOrm.Repository;

namespace MapperOrm.CommandBuilders
{
    internal static class UpdateCommandBuilder
    {
        private static readonly Dictionary<Type, DbType> TypeMap = NetTypesToDbTypesMapper.Instance;

        public static string Create(IDbCommand command, ICollection<EntityStruct> objs)
        {
            var result = new StringBuilder();
            foreach (var obj in objs)
            {
                result.Append(@CreateByType(command, obj));
            }
            return result.ToString();
        }

        private static string CreateByType(IDbCommand command, EntityStruct objPair)
        {
            var type = objPair.Key;
            var tableName = CommonCommandBuilder.GetTableName(type);
            var cmdBulder = new StringBuilder(string.Format(@" UPDATE [{0}].[{1}].[{2}] ", command.Connection.Database, CommonCommandBuilder.DbSchemaName, tableName));
            var nameValuePairs = CommonCommandBuilder.FieldByValueClauseBuilder(type, objPair.Value);
            var parameterizatedNames = nameValuePairs.Select(
                x => new KeyValuePair<string, string>(x.Key, CommonCommandBuilder.GetParamsFormat(x.Key, x.Value, command.Connection.Database)));
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
            cmdBulder.Append(string.Format(" SET {0} {1}", updateClause.Trim(','), CommonCommandBuilder.WhereByIdClause(command, type, objPair)));
            return cmdBulder.ToString();
        }


    }
}