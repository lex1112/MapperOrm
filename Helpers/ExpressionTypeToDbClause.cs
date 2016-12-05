using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapperOrm.Helpers
{
    internal class ExpressionTypeToDbClause
    {
        static ExpressionTypeToDbClause()
        {
            _instance = new Dictionary<ExpressionType, string >();
            _instance[ExpressionType.AndAlso] = " AND ";
            _instance[ExpressionType.OrElse] = " OR ";
            _instance[ExpressionType.Equal] = " = ";
            _instance[ExpressionType.GreaterThan] = " > ";
            _instance[ExpressionType.GreaterThanOrEqual] = " >= ";
            _instance[ExpressionType.NotEqual] = " <> ";
            _instance[ExpressionType.LessThan] = " < ";
            _instance[ExpressionType.LessThanOrEqual] = " <= ";
            _instance[ExpressionType.TypeEqual] = " = ";
            _instance[ExpressionType.Not] = " <> ";
        }
        private static readonly Dictionary<ExpressionType, string> _instance;

        public static Dictionary<ExpressionType, string> Instance
        {
            get { return _instance; }
        }



    }
}
