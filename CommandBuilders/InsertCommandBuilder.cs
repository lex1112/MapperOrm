using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using MapperOrm.Helpers;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.CommandBuilders
{
    internal static class InsertCommandBuilder
    {
        private static readonly Dictionary<Type, DbType> TypeMap = NetTypesToDbTypesMapper.Instance;

        public static string Create(IDbCommand command, ICollection<EntityStruct> objs)
        {
            var result = new StringBuilder();

            var dict = new Dictionary<Type, ICollection<EntityBase>>();
            foreach (var o in objs)
            {
                if (!dict.ContainsKey(o.Key))
                {
                    dict.Add(o.Key, new Collection<EntityBase> { o.Value });
                }
                else
                {
                    dict[o.Key].Add(o.Value);
                }
            }
            foreach (var d in dict)
            {
                result.Append(@CreateByType(command, d));
            }
            return result.ToString();
        }

        private static string CreateByType(IDbCommand command, KeyValuePair<Type, ICollection<EntityBase>> objs)
        {
            var type = objs.Key;
            var tableName = CommonCommandBuilder.GetTableName(type);
            var names = CommonCommandBuilder.FieldByValueClauseBuilder(type, objs.Value.First()).Select(x => x.Key);
            var insertedFieldNames = string.Join(",", names);

            var cmdBulder = new StringBuilder(
                string.Format(@" INSERT INTO [{0}].[{1}].[{2}] ({3})  VALUES ",
                              command.Connection.Database, CommonCommandBuilder.DbSchemaName, tableName, insertedFieldNames.Trim(',')));

            var valuesBuilder = new StringBuilder();
            foreach (var obj in objs.Value)
            {
                var nameValuePair = CommonCommandBuilder.FieldByValueClauseBuilder(type, obj);
                var parameterizatedNames =
                    nameValuePair.Select(x=>CommonCommandBuilder.GetParamsFormat(x.Key, x.Value, command.Connection.Database));
                var insertedNames = string.Join(",", parameterizatedNames);
                valuesBuilder.Append(string.Format(" ({0}),", insertedNames.Trim(',')));
                foreach (var pair in nameValuePair)
                {
                    var prop = CommonCommandBuilder.GetPropertyByAttrName(type, pair.Key);
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = CommonCommandBuilder.GetParamsFormat(pair.Key, pair.Value, command.Connection.Database);
                    parameter.DbType = TypeMap[prop.PropertyType];
                    parameter.Value = pair.Value;
                    if (!command.Parameters.Contains(parameter.ParameterName))
                        command.Parameters.Add(parameter);
                }

            }

            cmdBulder.Append(valuesBuilder.ToString().Trim(','));
            return cmdBulder.ToString();
        }


    }
}