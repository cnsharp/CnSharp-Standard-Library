using System;
using System.Linq;
using System.Linq.Expressions;

namespace CnSharp.Expressions
{
    /// <summary>
    /// Expression tree query attacher
    /// </summary>
    public class ExpressionAttacher
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionAttacher"/> class.
        /// </summary>
        public ExpressionAttacher()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the data source for retrieval.
        /// </summary>
        public IQueryable DataSource { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Attaches a query to the data source.
        /// </summary>
        /// <param name="originalElementType">The original element type of the attached query.</param>
        /// <param name="targetExpr">The expression tree of the attached query.</param>
        /// <returns>An interface for the completed query parser.</returns>
        public IQueryableParser Attach(Type originalElementType, Expression targetExpr)
        {
            var builder = new ExpressionRewriteBuilder(originalElementType, DataSource.ElementType);
            var parser = new QueryableParser { DataSource = DataSource, };
            parser.Converter = p =>
            {
                if (p is UnaryExpression)
                    p = (p as UnaryExpression).Operand;

                if (!(p is LambdaExpression))
                    throw new NotSupportedException();

                var lambda = p as LambdaExpression;
                var @parameters = lambda.Parameters.Where(r => r.Type == builder.SourceType).ToArray();

                if (!@parameters.Any())
                    return p;

                if (@parameters.Count() > 1)
                    throw new NotSupportedException();

                var parameterExpr = Expression.Parameter(DataSource.ElementType, @parameters[0].Name);

                var value = builder.Build(lambda.Body, expr =>
                {
                    if (!(expr is ParameterExpression))
                        return expr;

                    var pe = expr as ParameterExpression;
                    if (pe.Name == parameterExpr.Name && pe.Type == parameterExpr.Type)
                        return parameterExpr;
                    else
                        return expr;
                });
                var result = Expression.Lambda(value, parameterExpr);

                return result;
            };

            parser.Build(targetExpr as MethodCallExpression);

            return parser;
        }

        #endregion Public Methods
    }
}
