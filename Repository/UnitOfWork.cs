using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using MapperOrm.DBContext;

namespace MapperOrm.Repository
{
    public sealed class UnitOfWork : IUnitOfWork, IDetector
    {
        private readonly Dictionary<string, IObjectDetector> _objectDetector;

        private event EventHandler Event;

        Dictionary<string, IObjectDetector> IDetector.ObjectDetector
        {
            get { return _objectDetector; }
        }


        public UnitOfWork()
        {
            _objectDetector = new Dictionary<string, IObjectDetector>();
            foreach (ConnectionStringSettings conName in ConfigurationManager.ConnectionStrings)
            {
                _objectDetector.Add(conName.Name, new DefaultObjectDetector());
            }
            Event += UnitOfWorkManager.OnDisponse;
        }


        public void Commit()
        {
            SaveChanges();
        }

        private async void SaveChanges()
        {
            await Task.Run(() =>
                    {
                        foreach (var objectDetector in _objectDetector)
                        {
                            var provider = DataSourceProviderFactory.Create(objectDetector.Key);
                            provider.Commit(objectDetector.Value.ChangeObjects, objectDetector.Value.DeletedObjects,
                                            objectDetector.Value.NewObjects);
                        }
                    }
                
                );
        }


        void IDisposable.Dispose()
        {
            Disponse();
        }


        internal void Disponse()
        {
            var handler = Event;
            if (handler == null) 
                return;
            handler(this, null);
            GC.SuppressFinalize(this);
        }


        ~UnitOfWork()
        {
            Disponse();
        }

    }
}
