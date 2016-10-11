using System;
using System.Collections.Generic;
using System.Reflection;
using MapperOrm.Attributes;

namespace MapperOrm.Helpers
{
    internal interface IReflectionWrapper
    {
        ICollection<PropertyInfo> GetPropertiesByFieldNamesAttrs(Type type);
        ICollection<PropertyInfo> GetPropertiesByRelatedEntityAttrs(Type type);
        ICollection<RelatedEntityAttribute> GetRelatedEntityAttribute(PropertyInfo propertyInfo);
        ICollection<FieldNameAttribute> GetFieldNameAttribute(PropertyInfo propertyInfo);

    }
}