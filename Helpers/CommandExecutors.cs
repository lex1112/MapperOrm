using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MapperOrm.Exceptions;
using MapperOrm.Models;

namespace MapperOrm.Helpers
{
    internal static class CommandExecutors
    {
        internal static T ExecuteOnceReader<T>(this IDbCommand cmd) where T : class, IEntity, new()
        {
            var reader = cmd.ExecuteReader();
            T result = null;
            using (reader)
            {
                while (reader != null && reader.Read())
                {
                    result = reader.MappedToType<T>(cmd.Connection);
                }
            }

            return result;
        }

        internal static ICollection<T> ExecuteListReader<T>(this IDbCommand cmd) where T : class, IEntity, new()
        {
            var reader = cmd.ExecuteReader();
            var result = new List<T>();
            using (reader)
            {
                while (reader != null && reader.Read())
                {
                    result.Add(reader.MappedToType<T>(cmd.Connection));
                }
            }
            return result;
        }


    }
}
