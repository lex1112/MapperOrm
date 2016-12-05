using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperOrm.Models
{
    public interface IEntity : ICloneable
    {
        int Id { get; set; }
    }
}
