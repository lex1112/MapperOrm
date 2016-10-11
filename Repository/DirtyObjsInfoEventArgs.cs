using System;
using System.Collections.Generic;

namespace MapperOrm.Repository
{
    internal class DirtyObjsInfoEventArgs : EventArgs
    {
        public DirtyObjsInfoEventArgs(ICollection<EntityStruct> objs)
        {
            DirtyObjs = objs;
        }

        public ICollection<EntityStruct> DirtyObjs { get; private set; }
    }
}