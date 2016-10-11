using System.Collections.Generic;
using System.Data;
using System.Text;
using MapperOrm.Repository;

namespace MapperOrm.CommandBuilders
{
    internal static class DeleteCommandBuilder
    {
        public static string Create(IDbCommand command, ICollection<EntityStruct> objs)
        {
            var result = new StringBuilder();
            foreach (var obj in objs)
            {
                result.Append(@CreateByType(command, obj));
            }
            return result.ToString();
        }
        private static string CreateByType(IDbCommand command, EntityStruct objPair)
        {
            var type = objPair.Key;
            var tableName = CommonCommandBuilder.GetTableName(type);
            var cmdBulder = new StringBuilder(
                string.Format(@" DELETE FROM [{0}].[{1}].[{2}] ",
                              command.Connection.Database, CommonCommandBuilder.DbSchemaName, tableName));
            cmdBulder.Append(CommonCommandBuilder.WhereByIdClause(command, type, objPair));
            return cmdBulder.ToString();
        }

    }
}