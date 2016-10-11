using System.Collections.Generic;

namespace MapperOrm.Repository
{
    internal interface IObjectDetector
    {


        ICollection<EntityStruct> NewObjects { get; }
        ICollection<EntityStruct> DeletedObjects { get; }
        ICollection<EntityStruct> ChangeObjects { get; }

        void InsertedNewObjects(object sender, AddObjInfoEventArgs e);
        void DeletedObjectsObjects(object sender, RemoveObjInfoEventArgs e);
        void RegistrationCleanObjects(object sender, DirtyObjsInfoEventArgs e);
    }
}