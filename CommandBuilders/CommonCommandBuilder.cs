using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using MapperOrm.Attributes;
using MapperOrm.Helpers;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.CommandBuilders
{
    internal static class CommonCommandBuilder
    {
        private static readonly IReflectionWrapper ReflectionWrapper = new CacheReflectionWrapper();
        public const string IdFieldEntityName = "Id";
        public const string DbSchemaName = "dbo";
        private static readonly Dictionary<Type, DbType> TypeMap = NetTypesToDbTypesMapper.Instance;


        public static ICollection<KeyValuePair<string, object>> FieldByValueClauseBuilder(Type type, EntityBase obj)
        {
            var propertiesByFieldNamesAttrs = ReflectionWrapper.GetPropertiesByFieldNamesAttrs(type);
            if (propertiesByFieldNamesAttrs.Count == 0)
                throw new Exception(string.Format("invalid db model {0}", type));
            var props =
                propertiesByFieldNamesAttrs.ExceptRelatedType().Where(y => y.Name != IdFieldEntityName).Select(x =>
                    {
                        var value = x.GetValue(obj, new object[] { });
                        return new KeyValuePair<string, object>(ReflectionWrapper.GetFieldNameAttribute(x).First().Value, value);
                    }).Where(x => x.Value != null);
            return props.ToList();
        }

        public static PropertyInfo GetPropertyByAttrName(Type type, string name)
        {
            var props = ReflectionWrapper.GetPropertiesByFieldNamesAttrs(type);
            foreach (var prop in props)
            {
                var attr = ReflectionWrapper.GetFieldNameAttribute(prop).First();
                if (attr.Value.Equals(name))
                {
                    return prop;
                }
            }
            throw new NotSupportedException(string.Format("Db name ({0}) doesn't exists in type({1}).", name, type));
        }



        public static string WhereBuilder(Type type, string fieldEntityName)
        {
            var tableName = GetTableName(type);
            var prop = ReflectionWrapper.GetPropertiesByFieldNamesAttrs(type).First(p => p.Name == fieldEntityName);
            var attrs = ReflectionWrapper.GetFieldNameAttribute(prop);
            return string.Format("[{0}].[{1}]", tableName, attrs.First().Value);
        }

        public static string WhereByIdClause(IDbCommand command, Type type, EntityStruct obj)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = GetParamsFormat(IdFieldEntityName, obj.Value, command.Connection.Database);
            parameter.DbType = TypeMap[obj.Value.Id.GetType()];
            parameter.Value = obj.Value.Id;
            if (!command.Parameters.Contains(parameter.ParameterName))
                command.Parameters.Add(parameter);
            return string.Format(" WHERE {0} = {1}", WhereBuilder(type, IdFieldEntityName), GetParamsFormat(IdFieldEntityName, obj.Value, command.Connection.Database));
        }



        public static string GetTableName(Type type)
        {
            var tableName = string.Empty;
            var attributes = type.GetCustomAttributes(true);

            foreach (var attr in attributes.OfType<TableNameAttribute>())
            {
                tableName = attr.Value;
                break;
            }
            return tableName;
        }



        public static string GetParamsFormat(string fieldName, object value, string dbName)
        {
            return string.Format("@{0}{1}", fieldName, GetHashCode(fieldName, value, dbName));
        }

        public static int GetHashCode(string fieldName, object value, string dbName)
        {
            unchecked
            {

                var code = fieldName.GetHashCode() ^ value.GetHashCode() ^ dbName.GetHashCode();
                return code & 0x7fffffff;
            }

        }
    }
}