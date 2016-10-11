using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MapperOrm.Attributes;

namespace MapperOrm.Helpers
{
    internal class ReflectionWrapper : IReflectionWrapper
    {

        public ICollection<PropertyInfo> GetPropertiesByFieldNamesAttrs(Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttributes
                                                       (typeof(
                                                            FieldNameAttribute),
                                                        false).Any()).ToList();
        }

        public ICollection<PropertyInfo> GetPropertiesByRelatedEntityAttrs(Type type)
        {
            return type.
                GetProperties().Where(p => p.GetCustomAttributes
                                               (typeof(
                                                    RelatedEntityAttribute),
                                                false).Any()).ToList();
        }

        public ICollection<RelatedEntityAttribute> GetRelatedEntityAttribute(PropertyInfo propertyInfo)
        {
            return (RelatedEntityAttribute[])propertyInfo.GetCustomAttributes
                                                 (typeof(
                                                      RelatedEntityAttribute),
                                                  false);
        }

        public ICollection<FieldNameAttribute> GetFieldNameAttribute(PropertyInfo propertyInfo)
        {
            return (FieldNameAttribute[])propertyInfo.GetCustomAttributes
                                             (typeof(FieldNameAttribute), false);
        }
    }
}