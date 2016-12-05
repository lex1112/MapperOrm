using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    internal class UpdateObjsInfoEventArgs : EventArgs
    {
        public UpdateObjsInfoEventArgs(BinaryExpression exp, EntityStruct obj)
        {
            UpdateObjs.Add(exp, obj);
        }

        public Dictionary<BinaryExpression, EntityStruct> UpdateObjs { get; private set; }
    }
}