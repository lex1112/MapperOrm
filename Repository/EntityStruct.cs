using System;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    internal struct EntityStruct
    {
        internal Type Key { get; private set; }
        internal EntityBase Value { get; private set; }
        internal EntityStruct(Type key, EntityBase value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public override int GetHashCode()
        {
            var code = Key.GetHashCode() * 25 + Value.GetHashCode();
            return code > 0 ? code : (-1) * code;
        }
    }
}