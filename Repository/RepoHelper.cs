using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapperOrm.DBContext;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    public static class RepoHelper
    {

        public static ICollection<T> GetByExpression<T>(this IRepository<T> repo, Func<T, bool> func) where T : IEntity, new()
        {
            return new Collection<T>();
        }

        public static ICollection<T> GreedyLoad<T, T2>(this IRepository<T> repo, Func<IRepository<T>, ICollection<T>> func, Func<T, EntitySet<T2>> prop)
            where T : class, IEntity, new()
            where T2 : class, IEntity, new()
        {
            var res = func.Invoke(repo);
            foreach (var r in res)
            {
                var value = prop.Invoke(r).Data;

            }

            return res;


        }

    }
}