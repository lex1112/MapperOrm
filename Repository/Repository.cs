using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using MapperOrm.Helpers;
using MapperOrm.DBContext;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    using System;
    using System.Collections.Generic;

    public abstract class Repository<T> : IRepository<T> where T : class, IEntity, new()
    {
        private readonly object _syncObj = new object();

        private IDataSourceProvider _dataSourceProvider;
        private event EventHandler<AddObjInfoEventArgs> AddObjEvent;
        private event EventHandler<RemoveObjInfoEventArgs> RemoveObjEvent;
        private event EventHandler<DirtyObjsInfoEventArgs> DirtyObjEvent;
        

        private IDataSourceProvider DataSourceProvider
        {
            get
            {
                lock (_syncObj)
                {
                    if (_dataSourceProvider.State == State.Close)
                    {
                        _dataSourceProvider = DataSourceProviderFactory.Create(_connectionName);
                    }
                    return _dataSourceProvider;
                }

            }

        }
        private readonly string _connectionName;
        private IObjectDetector ObjectDetector
        {
            get
            {
                lock (_syncObj)
                {
                    return GetObjectDetector();
                }
            }
        }

        internal Repository(IDataSourceProvider dataSourceProvider)
        {
            if (dataSourceProvider == null)
                throw new ArgumentNullException("dataSourceProvider");
            _dataSourceProvider = dataSourceProvider;
        }
        protected Repository(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentNullException("connectionName");
            }
            _connectionName = connectionName;
            var dataSourceProvider = DataSourceProviderFactory.Create(connectionName);
            if (dataSourceProvider == null)
            {
                throw new ArgumentNullException("dataSourceProvider");
            }
            _dataSourceProvider = dataSourceProvider;
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            AddObjEvent += ObjectDetector.InsertedNewObjects;
            RemoveObjEvent += ObjectDetector.DeletedObjectsObjects;
            DirtyObjEvent += ObjectDetector.RegistrationCleanObjects;
        }


        public ICollection<T> GetByColumnName(Dictionary<string, object> keyValues)
        {
            var result = DataSourceProvider.ExecuteByField<T>(keyValues);
            var registratedObjs = result.Select(r => new EntityStruct(typeof(T), r as EntityBase)).ToList();

            var handler = DirtyObjEvent;
            if (handler == null)
                throw new EventLogException();

            handler(this, new DirtyObjsInfoEventArgs(registratedObjs));
            return result;

        }
        public void Add(T obj)
        {
            var handler = AddObjEvent;
            if (handler == null)
                return;
            handler(this, new AddObjInfoEventArgs(new EntityStruct(typeof(T), obj as EntityBase)));
        }

        public void Remove(T obj)
        {
            var handler = RemoveObjEvent;
            if (handler == null)
                return;
            handler(this, new RemoveObjInfoEventArgs(new EntityStruct(typeof(T), obj as EntityBase)));
        }

        private IObjectDetector GetObjectDetector()
        {
            var uow = UnitOfWorkManager.Current;
            if (uow == null)
            {
                throw new ApplicationException("Current context of unit of work is did't make. Create unit of work context and using UnitPfWorkManager.");
            }

            var detector = uow as IDetector;
            if (detector == null)
            {
                throw new ApplicationException("Current context of unit of work is did't make. Create unit of work context and using UnitPfWorkManager.");
            }

            return detector.ObjectDetector[_connectionName];
        }
    }
}
