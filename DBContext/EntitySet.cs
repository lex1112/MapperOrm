using System.Collections.Generic;
using System.Data;
using MapperOrm.Models;

namespace MapperOrm.DBContext
{
    public class EntitySet<T> : IEntitySet where T : class, IEntity, new()
    {
        private readonly object _syncObj = new object();
        private readonly ICollection<T> _entities = null;
        private readonly int _entityId;
        private readonly string _fieldName;
        private readonly string _connString = null;
        private bool _isLoad;

        public EntitySet(int id, string fieldName, string connString)
        {
            _isLoad = false;
            _entityId = id;
            _fieldName = fieldName;
            _connString = connString;
        }

        private ICollection<T> Load()
        {
            _isLoad = true;
            var keyValues = new Dictionary<string, object> { { _fieldName, _entityId } };
            return DataSourceProviderFactory.CreateByDefaultDataProvider(_connString).ExecuteByField<T>(keyValues);

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
