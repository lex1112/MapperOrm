using System;
using System.Data;

namespace MapperOrm.Exceptions
{
   
    class ExecuteQueryException : Exception
    {
        public override string Message
        {
            get { return string.Format("During execute exception. {0}", _queryString); }
        }

        private readonly string _queryString;

        public ExecuteQueryException(IDbCommand cmd)
        {
            _queryString = cmd.CommandText;
        }
    }
}
