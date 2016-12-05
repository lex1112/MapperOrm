using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using MapperOrm.Helpers;
using MapperOrm.Models;

namespace MapperOrm.Repository
{

    class IdentityMapEqualityComparer : IEqualityComparer<EntityStruct>
    {

        public bool Equals(EntityStruct x, EntityStruct y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(EntityStruct obj)
        {
           return obj.GetHashCode();
        }
    }

    internal class DefaultObjectTracker : IObjectTracker
    {
        private readonly Dictionary<EntityStruct, EntityStruct> _dirtyObjects;

        public ICollection<EntityStruct> NewObjects { get; private set; }
        public Dictionary<BinaryExpression, EntityStruct> UpdatedObjects { get; private set; }
        public Dictionary<BinaryExpression, Type> DeletedWhereExp { get; private set; }
        public ICollection<EntityStruct> DeletedObjects { get; private set; }
        public ICollection<EntityStruct> ChangeObjects
        {
            get
            {
                return _dirtyObjects.GetChangesObjects();
            }
        }

        internal DefaultObjectTracker()
        {
            NewObjects = new Collection<EntityStruct>();
            DeletedObjects = new Collection<EntityStruct>();
            _dirtyObjects = new Dictionary<EntityStruct, EntityStruct>(new IdentityMapEqualityComparer());
            UpdatedObjects = new Dictionary<BinaryExpression, EntityStruct>();
            DeletedWhereExp = new Dictionary<BinaryExpression, Type>(); ;
        }

        public void RegInsertedNewObjects(object sender, AddObjInfoEventArgs e)
        {
            NewObjects.Add(e.InsertedObj);
        }

        public void RegDeletedObjects(object sender, RemoveObjInfoEventArgs e)
        {
            DeletedObjects.Add(e.RemovedObj);
        }


        public void RegCleanObjects(object sender, DirtyObjsInfoEventArgs e)
        {
            var objs = e.DirtyObjs;
            foreach (var obj in objs)
            {
                if (!_dirtyObjects.ContainsKey(obj))
                {
                    var cloneObj = new EntityStruct(obj.Key, (EntityBase)obj.Value.Clone());
                    _dirtyObjects.Add(obj, cloneObj);
                }

            }
        }

        public void RegUpdatedObjects(object sender, UpdateObjsInfoEventArgs e)
        {
            var objs = e.UpdateObjs;
            foreach (var obj in objs)
            {
                UpdatedObjects.Add(obj.Key, obj.Value);
            }
        }




        public void RegDeletedExpressions(object sender, DeleteWhereObjsInfoEventArgs e)
        {
            var objs = e.DeleteWhereExp;
            foreach (var obj in objs)
            {
                DeletedWhereExp.Add(obj.Key, obj.Value);
            }
        }
    }
}