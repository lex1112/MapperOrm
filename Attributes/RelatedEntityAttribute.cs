using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperOrm.Attributes
{
   
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class RelatedEntityAttribute : Attribute
    {
        private readonly string _fieldName;
        private readonly string _connectionString;
        private readonly string _relatedFieldName;
        public RelatedEntityAttribute(string fieldName, string relatedFieldName, string connectionString = null)
        {
            _fieldName = fieldName;
            _relatedFieldName = relatedFieldName;
            _connectionString = connectionString;
        }

        public string FieldName { get { return _fieldName; } }
        public string RelatedFieldName { get { return _relatedFieldName; } }
        public string ConnectionString { get { return _connectionString; } }
    }

}
