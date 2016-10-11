using System;

namespace MapperOrm.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class FieldNameAttribute : Attribute
    {
        public FieldNameAttribute(string fieldName)
        {
            Value = fieldName;
        }

        public string Value { get; private set; }
    }
}