using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.DBContext
{
    internal class TrackerProvider : IDataSourceProvider
    {
        private event EventHandler<AddObjInfoEventArgs> AddObjEvent;
        private event EventHandler<RemoveObjInfoEventArgs> RemoveObjEvent;
        private event EventHandler<DirtyObjsInfoEventArgs> DirtyObjEvent;
        private event EventHandler<UpdateObjsInfoEventArgs> UpdateObjEvent;
        private event EventHandler<DeleteWhereObjsInfoEventArgs> DeleteWhereObjEvent;


        private readonly IDataSourceProvider _dataSourceProvider;
        private readonly string _connectionName;
        private readonly object _syncObj = new object();

        private IObjectTracker ObjectTracker
        {
            get
            {
                lock (_syncObj)
                {
                    return Session.GetObjectTracker(_connectionName);
                }
            }
        }

        public TrackerProvider(string connectionName)
        {
            _connectionName = connectionName;
            _dataSourceProvider = DataSourceProviderFactory.Create(_connectionName);
            RegisterEvents();
        }


        public State State
        {
            get
            {
                return _dataSourceProvider.State;
            }
        }

        private void RegisterEvents()
        {
            if (Session.Current == null)
            {
                throw new ApplicationException("Session should be used. Create a session.");
            };
            AddObjEvent += ObjectTracker.RegInsertedNewObjects;
            RemoveObjEvent += ObjectTracker.RegDeletedObjects;
            DirtyObjEvent += ObjectTracker.RegCleanObjects;
            UpdateObjEvent += ObjectTracker.RegUpdatedObjects;
            DeleteWhereObjEvent += ObjectTracker.RegDeletedExpressions;
        }


        public ICollection<T> GetByFields<T>(EntityStruct obj) where T : EntityBase, IEntity, new()
        {
            return _dataSourceProvider.GetByFields<T>(obj);
        }

        public void Add(EntityStruct obj)
        {
            var handler = AddObjEvent;
            if (handler == null)
                return;
            handler(this, new AddObjInfoEventArgs(obj));
        }

        public void Remove(EntityStruct obj)
        {
           var handler = RemoveObjEvent;
            if (handler == null)
                return;
            handler(this, new RemoveObjInfoEventArgs(obj));
        }

        public void RemoveWhere(BinaryExpression exp, Type type)
        {
            var handler = DeleteWhereObjEvent;
            if (handler == null)
                return;
            handler(this, new DeleteWhereObjsInfoEventArgs(exp, type));
        }

        public void Commit(ICollection<EntityStruct> updObjs, ICollection<EntityStruct> delObjs, 
            ICollection<EntityStruct> addObjs, 
            Dictionary<BinaryExpression, EntityStruct> packUpdObjs,
                           Dictionary<BinaryExpression, Type> deleteUpdObjs)
        {
            _dataSourceProvider.Commit(updObjs, delObjs, addObjs, packUpdObjs, deleteUpdObjs);
        }

        public ICollection<T> GetByFields<T>(BinaryExpression exp) where T : EntityBase, IEntity, new()
        {
            var result = _dataSourceProvider.GetByFields<T>(exp);
            var registratedObjs = result.Select(r => new EntityStruct(typeof(T), r)).ToList();

            var handler = DirtyObjEvent;
            if (handler == null)
                return result;
            handler(this, new DirtyObjsInfoEventArgs(registratedObjs));
            return result;
        }

        public void Update<T>(BinaryExpression exp, EntityStruct obj) where T : EntityBase, IEntity, new()
        {
            var handler = UpdateObjEvent;
            if (handler == null)
                return;
            handler(this, new UpdateObjsInfoEventArgs(exp, obj));
        }


        public void Dispose()
        {
            _dataSourceProvider.Dispose();
        }
    }
}
