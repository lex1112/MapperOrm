using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.DBContext
{
    internal interface IDataSourceProvider : IDisposable
    {
        State State { get; }
        ICollection<T> GetByFields<T>(EntityStruct obj) where T : EntityBase, IEntity, new();
        void Add(EntityStruct obj);
        void Remove(EntityStruct obj);
        void RemoveWhere(BinaryExpression exp, Type type);
        void Commit(ICollection<EntityStruct> updObjs,
            ICollection<EntityStruct> delObjs, 
            ICollection<EntityStruct> addObjs, 
            Dictionary<BinaryExpression, EntityStruct> packUpdObjs,
            Dictionary<BinaryExpression, Type> deleteUpdObjs);
        ICollection<T> GetByFields<T>(BinaryExpression exp) where T : EntityBase, IEntity, new();
        void Update<T>(BinaryExpression exp, EntityStruct obj) where T : EntityBase, IEntity, new();
    }
}