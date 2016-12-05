using System.Collections.Generic;
using System.Data;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.DBContext
{
    public class EntitySet<T> : IEntitySet where T : EntityBase, IEntity, new()
    {
        private readonly object _syncObj = new object();
        private readonly ICollection<T> _entities = null;
        private readonly T _param;
        private readonly string _connString;
        private bool _isLoad;

        public EntitySet(T param, string connString)
        {
            _isLoad = false;
            _connString = connString;
            _param = param;
        }

        private ICollection<T> Load()
        {
            _isLoad = true;
            return DataSourceProviderFactory
                .CreateByDefaultDataProvider(_connString).GetByFields<T>(new EntityStruct(_param.GetType(), _param));

        }

        public ICollection<T> Data
        {
            get
            {
                lock (_syncObj)
                {
                    return _entities ?? Load();
                }
            }
        }

        public bool IsLoad
        {
            get
            {
                lock (_syncObj)
                {
                    return _isLoad;
                }
            }
        }
    }
}
