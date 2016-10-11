using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MapperOrm.Helpers
{
    internal class NetTypesToDbTypesMapper
    {
        static NetTypesToDbTypesMapper()
        {
            _instance = new Dictionary<Type, DbType>();

            _instance[typeof(byte)] = DbType.Byte;
            _instance[typeof(sbyte)] = DbType.SByte;
            _instance[typeof(short)] = DbType.Int16;
            _instance[typeof(ushort)] = DbType.UInt16;
            _instance[typeof(int)] = DbType.Int32;
            _instance[typeof(uint)] = DbType.UInt32;
            _instance[typeof(long)] = DbType.Int64;
            _instance[typeof(ulong)] = DbType.UInt64;
            _instance[typeof(float)] = DbType.Single;
            _instance[typeof(double)] = DbType.Double;
            _instance[typeof(decimal)] = DbType.Decimal;
            _instance[typeof(bool)] = DbType.Boolean;
            _instance[typeof(string)] = DbType.String;
            _instance[typeof(char)] = DbType.StringFixedLength;
            _instance[typeof(Guid)] = DbType.Guid;
            _instance[typeof(DateTime)] = DbType.DateTime;
            _instance[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            _instance[typeof(byte[])] = DbType.Binary;
            _instance[typeof(byte?)] = DbType.Byte;
            _instance[typeof(sbyte?)] = DbType.SByte;
            _instance[typeof(short?)] = DbType.Int16;
            _instance[typeof(ushort?)] = DbType.UInt16;
            _instance[typeof(int?)] = DbType.Int32;
            _instance[typeof(uint?)] = DbType.UInt32;
            _instance[typeof(long?)] = DbType.Int64;
            _instance[typeof(ulong?)] = DbType.UInt64;
            _instance[typeof(float?)] = DbType.Single;
            _instance[typeof(double?)] = DbType.Double;
            _instance[typeof(decimal?)] = DbType.Decimal;
            _instance[typeof(bool?)] = DbType.Boolean;
            _instance[typeof(char?)] = DbType.StringFixedLength;
            _instance[typeof(Guid?)] = DbType.Guid;
            _instance[typeof(DateTime?)] = DbType.DateTime;
            _instance[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            _instance[typeof(System.Data.Linq.Binary)] = DbType.Binary;


        }
        private static Dictionary<Type, DbType> _instance;

        public static Dictionary<Type, DbType> Instance
        {
            get { return _instance; }
        }



    }
}
