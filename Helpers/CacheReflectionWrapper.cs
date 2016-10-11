using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MapperOrm.Attributes;

namespace MapperOrm.Helpers
{
    internal class CacheReflectionWrapper : IReflectionWrapper
    {
        private object _syncObj = new object();
        private readonly IReflectionWrapper _sourceReflection;
        private readonly Dictionary<Type, ICollection<PropertyInfo>> _cacheDictPropsByFieldNamesAttrs = new Dictionary<Type, ICollection<PropertyInfo>>();
        private readonly Dictionary<Type, ICollection<PropertyInfo>> _cacheDictPropsByRelatedEntityAttrs = new Dictionary<Type, ICollection<PropertyInfo>>();

        private readonly Dictionary<PropertyInfo, ICollection<FieldNameAttribute>> _cacheDictFieldNamesAttrs = new Dictionary<PropertyInfo, ICollection<FieldNameAttribute>>();
        private readonly Dictionary<PropertyInfo, ICollection<RelatedEntityAttribute>> _cacheDictRelatedEntityAttrs = new Dictionary<PropertyInfo, ICollection<RelatedEntityAttribute>>();




        public CacheReflectionWrapper()
        {
            _sourceReflection = new ReflectionWrapper();
        }

        public ICollection<PropertyInfo> GetPropertiesByFieldNamesAttrs(Type type)
        {
            lock (_syncObj)
            {
                if (!_cacheDictPropsByFieldNamesAttrs.ContainsKey(type))
                {
                    var result = _sourceReflection.GetPropertiesByFieldNamesAttrs(type);
                    _cacheDictPropsByFieldNamesAttrs.Add(type, result);
                    foreach (var prop in _cacheDictPropsByFieldNamesAttrs.Values.SelectMany(propertyInfo => propertyInfo))
                    {
                        GetFieldNameAttribute(prop);
                    }
                }
                return _cacheDictPropsByFieldNamesAttrs[type];
            }


        }

        public ICollection<PropertyInfo> GetPropertiesByRelatedEntityAttrs(Type type)
        {
            lock (_syncObj)
            {
                if (!_cacheDictPropsByRelatedEntityAttrs.ContainsKey(type))
                {
                    var result = _sourceReflection.GetPropertiesByRelatedEntityAttrs(type);
                    _cacheDictPropsByRelatedEntityAttrs.Add(type, result);
                    foreach (
                        var prop in _cacheDictPropsByRelatedEntityAttrs.Values.SelectMany(propertyInfo => propertyInfo))
                    {
                        GetRelatedEntityAttribute(prop);
                    }
                }
                return _cacheDictPropsByRelatedEntityAttrs[type];
            }
        }

        public ICollection<RelatedEntityAttribute> GetRelatedEntityAttribute(PropertyInfo propertyInfo)
        {
            lock (_syncObj)
            {
                if (!_cacheDictRelatedEntityAttrs.ContainsKey(propertyInfo))
                {
                    var result = _sourceReflection.GetRelatedEntityAttribute(propertyInfo);
                    _cacheDictRelatedEntityAttrs.Add(propertyInfo, result);
                }
                return _cacheDictRelatedEntityAttrs[propertyInfo];
            }
        }

        public ICollection<FieldNameAttribute> GetFieldNameAttribute(PropertyInfo propertyInfo)
        {
            lock (_syncObj)
            {
                if (!_cacheDictFieldNamesAttrs.ContainsKey(propertyInfo))
                {
                    var result = _sourceReflection.GetFieldNameAttribute(propertyInfo);
                    _cacheDictFieldNamesAttrs.Add(propertyInfo, result);
                }
                return _cacheDictFieldNamesAttrs[propertyInfo];
            }
        }
    }
}