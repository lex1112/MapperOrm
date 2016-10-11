using System;
using System.Collections.Generic;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.DBContext
{
    internal interface IDataSourceProvider : IDisposable
    {
        State State { get; }
        ICollection<T> ExecuteByField<T>(Dictionary<string, object> keyValue) where T : class, IEntity, new();
        void Add(ICollection<EntityStruct> objs);
        void Update(ICollection<EntityStruct> objs);
        void Remove(ICollection<EntityStruct> objs);
        void Commit(ICollection<EntityStruct> updObjs, ICollection<EntityStruct> delObjs, ICollection<EntityStruct> addObjs);

    }
}