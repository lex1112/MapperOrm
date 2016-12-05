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
    internal static class SelectCommandBulder
    {
        private static readonly IReflectionWrapper ReflectionWrapper = new CacheReflectionWrapper();
        private static readonly Dictionary<Type, DbType> TypeMap = NetTypesToDbTypesMapper.Instance;

        public static string Create(IDbCommand command, EntityStruct objPair)
        {
            var type = objPair.Key;
            var selectBody = CreateBody(command.Connection.Database, type);
            return string.Format("{0} WHERE {1}", selectBody, FilterBuilder(command, objPair));
        }
        public static string Create<T>(IDbCommand command, BinaryExpression exp)
            where T : class, IEntity, new()
        {
            var type = typeof(T);
            var selectBody = CreateBody(command.Connection.Database, type);
            return string.Format("{0} WHERE {1}", selectBody, CommonCommandBuilder.BuildClauseByExpression(command,type, exp));
        }

        private static string CreateBody(string dbName, Type type)
        {
            var tableName = CommonCommandBuilder.GetTableName(type);
            var cmdBulder = new StringBuilder();
            foreach (var prop in ReflectionWrapper.GetPropertiesByFieldNamesAttrs(type))
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

        public static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static bool CompareToDefault<T>(T obj1, T obj2)
        {
            if (obj2 == null)
            {
                return false;
            }
            if (obj1 == null && obj2 != null)
                return true;
            return !obj1.Equals(obj2);
        }



        private static string FilterBuilder(IDbCommand command, EntityStruct objPair)
        {
            var result = new StringBuilder();
            foreach (var prop in ReflectionWrapper.GetPropertiesByFieldNamesAttrs(objPair.Key))
            {
                if (CompareToDefault(GetDefault(prop.PropertyType), prop.GetValue(objPair.Value)))
                {
                    var value = GetFieldValue(prop);
                    var valueFormat = string.Format("@{0}", value);
                    result.AppendFormat("{0} = {1} AND ", WhereBuilder(prop, objPair.Key), valueFormat);

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = valueFormat;
                    parameter.DbType = TypeMap[prop.PropertyType];
                    parameter.Value = prop.GetValue(objPair.Value);

                    command.Parameters.Add(parameter);
                }

            }
            var str = result.ToString();
            return str.Remove(str.Length - 4, 4);
        }

        private static string WhereBuilder(PropertyInfo prop, Type type)
        {
            var tableName = CommonCommandBuilder.GetTableName(type);
            return string.Format("[{0}].[{1}]", tableName, GetFieldValue(prop));
        }

        private static string GetFieldValue(PropertyInfo prop)
        {
            var attrs = ReflectionWrapper.GetFieldNameAttribute(prop);
            return attrs.First().Value;
        }
    }
}