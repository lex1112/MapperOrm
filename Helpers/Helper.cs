using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MapperOrm.CommandBuilders;
using MapperOrm.DBContext;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.Helpers
{
    internal static class Helper
    {

        public static ICollection<PropertyInfo> ExceptRelatedType(this ICollection<PropertyInfo> props)
        {
            return props.Where(prop => !prop.PropertyType.IsGenericType || prop.PropertyType.GetGenericTypeDefinition() != typeof(EntitySet<>)).ToList();
        }

        public static ICollection<EntityStruct> GetChangesObjects(this Dictionary<EntityStruct, EntityStruct> dirtyObjs)
        {
            var result = new List<EntityStruct>();
            foreach (var cleanObj in dirtyObjs.Keys)
            {
                if (!(cleanObj.Key == dirtyObjs[cleanObj].Key))
                {
                    throw new Exception("incorrect types");
                }
                if (ChangeDirtyObjs(cleanObj.Value, dirtyObjs[cleanObj].Value, cleanObj.Key))
                {
                    result.Add(cleanObj);
                }
            }

            return result;
        }

        public static bool ChangeDirtyObjs(EntityBase cleanObj, EntityBase dirtyObj, Type type)
        {
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var cleanValue = prop.GetValue(cleanObj, null);
                var dirtyValue = prop.GetValue(dirtyObj, null);
                if (!cleanValue.Equals(dirtyValue))
                {
                    return true;
                }
            }

            return false;

        }




        public static SqlDbType GetDBType(Type theType)
        {
            SqlParameter p1 = new SqlParameter();
            TypeConverter tc = TypeDescriptor.GetConverter(p1.DbType);

            p1.DbType = (DbType)tc.ConvertFrom(theType.Name);

            return p1.SqlDbType;
        }

        public static object CastDbTypes(this object obj, Type type)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var t = type.GetGenericArguments()[0];

                if (DBNull.Value.Equals(obj))
                {
                    return null;
                }
                return Convert.ChangeType(obj, t);
            }
            if (!DBNull.Value.Equals(obj))
            {
                return Convert.ChangeType(obj, type);
            }
            if (type.IsValueType)
            {
                switch (type.ToString())
                {
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.Byte":
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt64":
                    case "System.UInt32":
                        return 0;
                    case "System.Boolean":
                        return false;
                    case "System.DateTime":
                        return DateTime.MinValue;

                }
            }
            return Convert.ChangeType(obj, type);
        }
    }
}
