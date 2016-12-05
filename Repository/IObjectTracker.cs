using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapperOrm.Repository
{
    internal interface IObjectTracker
    {
        ICollection<EntityStruct> NewObjects { get; }
        Dictionary<BinaryExpression, EntityStruct> UpdatedObjects { get; }
        Dictionary<BinaryExpression, Type> DeletedWhereExp { get; }
        ICollection<EntityStruct> DeletedObjects { get; }
        ICollection<EntityStruct> ChangeObjects { get; }

        void RegInsertedNewObjects(object sender, AddObjInfoEventArgs e);
        void RegDeletedObjects(object sender, RemoveObjInfoEventArgs e);
        void RegCleanObjects(object sender, DirtyObjsInfoEventArgs e);
        void RegUpdatedObjects(object sender, UpdateObjsInfoEventArgs e);
        void RegDeletedExpressions(object sender, DeleteWhereObjsInfoEventArgs e);
    }




}