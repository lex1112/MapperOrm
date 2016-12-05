using System;
using System.Data;
using System.Linq;
using System.Reflection;
using MapperOrm.Models;

namespace MapperOrm.Helpers
{
    internal static class DefaultTypeMapper
    {
        private static readonly IReflectionWrapper ReflectionWrapper = new CacheReflectionWrapper();

        public static T MappedToType<T>(this IDataReader reader, IDbConnection connection) where T : class, IEntity, new()
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var nameProps = ReflectionWrapper.GetPropertiesByFieldNamesAttrs(typeof(T));
            if (reader.FieldCount < nameProps.Count)
            {
                throw new ArgumentException("Entity could not be mapped. Entity colums is not exists.");
            }
            var result = new T();

          
            foreach (var prop in nameProps)
            {
                try
                {
                    var attrs = ReflectionWrapper.GetFieldNameAttribute(prop);

                    var exit = false;
                    var i = 0;
                    if (attrs.Count != 0)
                    {
                        while (i <= reader.FieldCount - 1 && !exit)
                        {

                            if (reader.GetName(i) == attrs.First().Value)
                            {
                                exit = true;
                                prop.SetValue(result, reader[i].CastDbTypes(prop.PropertyType), null);
                              
                            }
                            i++;
                           
                        }
                    }
                }
                catch (Exception)
                {
                    throw new InvalidCastException(
                        string.Format("Uncorrect mapping fields to .net types in enity:{0}; field name:{1}", typeof(T), prop.Name));
                }
            }

            foreach (var relatedProp in ReflectionWrapper.GetPropertiesByRelatedEntityAttrs(typeof(T)))
            {
                try
                {
                    var relatedAttr = ReflectionWrapper.GetRelatedEntityAttribute(relatedProp);
                    foreach (var resProp in ReflectionWrapper.GetPropertiesByFieldNamesAttrs(result.GetType()))
                    {
                        if (resProp.Name == relatedAttr.First().FieldName)
                        {
                            var id = Convert.ToInt32(resProp.GetValue(result, null));
                            var fieldName = relatedAttr.First().RelatedFieldName;
                            var connString = connection.ConnectionString;
                            var type = relatedProp.PropertyType.GetGenericArguments()[0];

                            var param = Activator.CreateInstance(type);
                            var propertyInfo = type.GetProperty(fieldName);
                            propertyInfo.SetValue(param, id, null);

                            var instance = Activator.CreateInstance(relatedProp.PropertyType, new object[] { param, connString });
                            relatedProp.SetValue(result, instance, null);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw new InvalidCastException(
                        string.Format("Uncorrect mapping related fields to .net types in enity:{0}; field name:{1}", typeof(T), relatedProp.Name));
                }
            }


            return result;
        }
    }
}