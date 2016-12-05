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
        private readonly Dictionary<string, IObjectTracker> _objectDetector;

        private event EventHandler Event;

        Dictionary<string, IObjectTracker> IDetector.ObjectDetector
        {
            get { return _objectDetector; }
        }


        public UnitOfWork()
        {
            _objectDetector = new Dictionary<string, IObjectTracker>();
            foreach (ConnectionStringSettings conName in ConfigurationManager.ConnectionStrings)
            {
                _objectDetector.Add(conName.Name, new DefaultObjectTracker());
            }
            Event += Session.OnDisponse;
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
                            var provider = new TrackerProvider(objectDetector.Key);
                            provider.Commit(
                                objectDetector.Value.ChangeObjects, 
                                objectDetector.Value.DeletedObjects,
                                            objectDetector.Value.NewObjects,
                                            objectDetector.Value.UpdatedObjects,
                                            objectDetector.Value.DeletedWhereExp);
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
