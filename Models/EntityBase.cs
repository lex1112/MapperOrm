using System;

namespace MapperOrm.Models
{
    public abstract class EntityBase : IEntity, ICloneable
    {
        public abstract int Id { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}