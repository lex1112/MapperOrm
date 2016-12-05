using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
        public bool Equals(EntityBase obj)
        {
            return Equals(obj);
        }

        public override int GetHashCode()
        {
            var code = Key.GetHashCode() * 25 + Value.Id.GetHashCode();
            return code > 0 ? code : (-1) * code;
        }

        internal class IdentityMap
        {
            private readonly HashSet<EntityBase> _entities = new HashSet<EntityBase>();

            public bool TryAdd(EntityBase obj)
            {
                return _entities.Add(obj);
            }

            public bool TryRemove(EntityBase obj)
            {
                return _entities.Remove(obj);
            }

           

        }

    }
}