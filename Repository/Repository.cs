using System.Linq;
using System.Linq.Expressions;
using MapperOrm.DBContext;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    using System;
    using System.Collections.Generic;

    public abstract class Repository<T> : IRepository<T> where T : EntityBase, IEntity, new()
    {
        private readonly object _syncObj = new object();

        private IDataSourceProvider _dataSourceProvider;
        private IDataSourceProvider DataSourceProvider
        {
            get
            {
                lock (_syncObj)
                {
                    if (_dataSourceProvider.State == State.Close)
                    {
                        _dataSourceProvider =  GetDataSourceProvider();
                        
                    }
                    return _dataSourceProvider;
                }

            }

        }

        private readonly string _connectionName;

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
            var dataSourceProvider = GetDataSourceProvider();
            if (dataSourceProvider == null)
            {
                throw new ArgumentNullException("dataSourceProvider");
            }
            _dataSourceProvider = dataSourceProvider;
        }

        private IDataSourceProvider GetDataSourceProvider()
        {
            return Session.Current == null ? DataSourceProviderFactory.Create(_connectionName)
                       : new TrackerProvider(_connectionName);
        }

        public ICollection<T> Get(Expression<Func<T, bool>> exp)
        {
            return DataSourceProvider.GetByFields<T>(exp.Body as BinaryExpression);
        }

        public void Update(Expression<Func<T, bool>> exp, Func<T, T> func)
        {
            var obj = func.Invoke(null);
            DataSourceProvider.Update<T>(exp.Body as BinaryExpression, new EntityStruct(typeof(T), obj));

        }

        public void RemoveWhere(Expression<Func<T, bool>> exp)
        {
            _dataSourceProvider.RemoveWhere(exp.Body as BinaryExpression, typeof(T));
           
        }

        public void Add(T obj)
        {
            _dataSourceProvider.Add(new EntityStruct(typeof(T), obj));
            
        }

        public void Remove(T obj)
        {
            _dataSourceProvider.Remove(new EntityStruct(typeof(T), obj));
        }

    }
}
