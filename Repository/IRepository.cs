using MapperOrm.Models;

namespace MapperOrm.Repository
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Интерфейс определяет спецификацию абстрактного хранилища данных
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>

    public interface IRepository
    {
    }


    public interface IRepository<T> : IRepository where T : IEntity, new()
    {
        ICollection<T> GetByColumnName(Dictionary<string, object> keyValues);
        void Add(T obj);
        void Remove(T obj);

    }
}
