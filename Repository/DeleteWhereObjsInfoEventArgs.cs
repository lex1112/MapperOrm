using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapperOrm.Repository
{
    internal class DeleteWhereObjsInfoEventArgs : EventArgs
    {
        public DeleteWhereObjsInfoEventArgs(BinaryExpression exp, Type type)
        {
            DeleteWhereExp.Add(exp, type);
        }

        public Dictionary<BinaryExpression, Type> DeleteWhereExp { get; private set; }
    }
}