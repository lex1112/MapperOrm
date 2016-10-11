using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MapperOrm.Helpers;
using MapperOrm.Models;

namespace MapperOrm.CommandBuilders
{
    internal static class SelectCommandBulder
    {
        private static readonly IReflectionWrapper ReflectionWrapper = new CacheReflectionWrapper();
        private static readonly Dictionary<Type, DbType> TypeMap = NetTypesToDbTypesMapper.Instance;

        public static string Create<T>(IDbCommand command, Dictionary<string, object> keyValues) where T : class, IEntity, new()
        {
            var selectBody = CreateBody<T>(command.Connection.Database);
            return string.Format("{0} WHERE {1}", selectBody, FilterBuilder<T>(command, keyValues));
        }


        private static string CreateBody<T>(string dbName) where T : class, IEntity, new()
        {
            var tableName = CommonCommandBuilder.GetTableName(typeof(T));
            var cmdBulder = new StringBuilder();
            foreach (var prop in ReflectionWrapper.GetPropertiesByFieldNamesAttrs(typeof(T)))
            {
                var attrs = ReflectionWrapper.GetFieldNameAttribute(prop);
                if (attrs.Count == 0)
                {
                    continue;
                }
                cmdBulder.Append(string.Format("[{0}].[{1}],", tableName, attrs.First().Value));
            }
            return string.Format("SELECT {0} FROM [{1}].[dbo].[{2}] WITH(NOLOCK)",
                                 cmdBulder.ToString().Trim(','), dbName, tableName);
        }


        private static string FilterBuilder<T>(IDbCommand command, Dictionary<string, object> keyValues) where T : class, IEntity, new()
        {
            var result = new StringBuilder();
            foreach (var keyValue in keyValues)
            {
                var prop = ReflectionWrapper.GetPropertiesByFieldNamesAttrs(typeof(T)).First(p => p.Name == keyValue.Key);
                var value = GetFieldValue<T>(prop);
                var valueFormat = string.Format("@{0}", value);
                result.AppendFormat("{0} = {1} AND ", WhereBuilder<T>(prop), valueFormat);

                var parameter = command.CreateParameter();
                parameter.ParameterName = valueFormat;
                parameter.DbType = TypeMap[prop.PropertyType];
                parameter.Value = keyValue.Value;

                command.Parameters.Add(parameter);
            }
            var str = result.ToString();
            return str.Remove(str.Length - 4, 4);
        }

        private static string WhereBuilder<T>(PropertyInfo prop) where T : class, IEntity, new()
        {
            var tableName = CommonCommandBuilder.GetTableName(typeof(T));
            return string.Format("[{0}].[{1}]", tableName, GetFieldValue<T>(prop));
        }

        private static string GetFieldValue<T>(PropertyInfo prop) where T : class, IEntity, new()
        {
            var attrs = ReflectionWrapper.GetFieldNameAttribute(prop);
            return attrs.First().Value;
        }
    }
}