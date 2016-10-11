using System;

namespace MapperOrm.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class TableNameAttribute : Attribute
    {
        private readonly string _value;
        public TableNameAttribute(string value)
        {
            _value = value;
        }

        public string Value { get { return _value; } }
    }
}