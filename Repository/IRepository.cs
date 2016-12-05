using System.Linq.Expressions;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    using System;
    using System.Collections.Generic;

    public interface IRepository<T> where T : IEntity, new()
    {
        void Add(T obj);
        void Remove(T obj);
        ICollection<T> Get(Expression<Func<T, bool>> exp);
        void Update(Expression<Func<T, bool>> exp, Func<T,T> func);
        void RemoveWhere(Expression<Func<T, bool>> exp);
    }
}
