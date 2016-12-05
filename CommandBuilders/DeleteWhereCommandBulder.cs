using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace MapperOrm.CommandBuilders
{
    internal static class DeleteWhereCommandBulder
    {
        public static string Create(IDbCommand command, Dictionary<BinaryExpression, Type> expDict)
        {
            var result = new StringBuilder();
            foreach (var exp in expDict)
            {
                result.Append(@CreateByType(command, exp.Key, exp.Value));
            }
            return result.ToString();
        }


        private static string CreateByType(IDbCommand command, BinaryExpression exp, Type type)
        {
            var tableName = CommonCommandBuilder.GetTableName(type);
            var cmdBulder = new StringBuilder(
                string.Format(@" DELETE FROM [{0}].[{1}].[{2}] ",
                              command.Connection.Database, CommonCommandBuilder.DbSchemaName, tableName));
            cmdBulder.Append(string.Format(" WHERE {0}", CommonCommandBuilder.BuildClauseByExpression(command, type, exp)));
            return cmdBulder.ToString();
        }
    }
}