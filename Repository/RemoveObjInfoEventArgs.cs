using System;

namespace MapperOrm.Repository
{
    internal class RemoveObjInfoEventArgs : EventArgs
    {
        public RemoveObjInfoEventArgs(EntityStruct obj)
        {
            RemovedObj = obj;
        }

        public EntityStruct RemovedObj { get; private set; }
    }
}