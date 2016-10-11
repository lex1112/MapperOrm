using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapperOrm.Helpers;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    internal class DefaultObjectDetector :IObjectDetector
    {
        private readonly Dictionary<EntityStruct, EntityStruct> _dirtyObjects;

        public ICollection<EntityStruct> NewObjects { get; private set; }
        public ICollection<EntityStruct> DeletedObjects { get; private set; }
        public ICollection<EntityStruct> ChangeObjects { 
            get { return _dirtyObjects.GetChangesObjects(); }
        }

        internal DefaultObjectDetector()
        {
            NewObjects = new Collection<EntityStruct>();
            DeletedObjects = new Collection<EntityStruct>();
            _dirtyObjects = new Dictionary<EntityStruct, EntityStruct>();
        }

        public void InsertedNewObjects(object sender, AddObjInfoEventArgs e)
        {
            NewObjects.Add(e.InsertedObj);
        }

        public void DeletedObjectsObjects(object sender, RemoveObjInfoEventArgs e)
        {
            DeletedObjects.Add(e.RemovedObj);
        }


        public void RegistrationCleanObjects(object sender, DirtyObjsInfoEventArgs e)
        {
            var objs = e.DirtyObjs;
            foreach (var obj in objs)
            {
                var cloneObj = new EntityStruct(obj.Key, (EntityBase)obj.Value.Clone());
                _dirtyObjects.Add(obj, cloneObj);
            }
        }
    }
}