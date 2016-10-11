using System;

namespace MapperOrm.Repository
{
    internal class AddObjInfoEventArgs:EventArgs
    {
        public AddObjInfoEventArgs(EntityStruct obj)
        {
            InsertedObj = obj;
        }

        public EntityStruct InsertedObj { get; private set; }
    }
}