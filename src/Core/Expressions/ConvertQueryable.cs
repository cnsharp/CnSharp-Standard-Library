using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CnSharp.Expressions
{
    /// <summary>
    /// Lambda expression tree conversion query class
    /// </summary>
    public static class ConvertQueryable
    {
        /// <summary>
        /// Create a data source for a conversion query.
        /// </summary>
        /// <typeparam name="T">Explicit query type</typeparam>
        /// <param name="query">A method to get a query interface of the real type.</param>
        /// <returns>A data source for an explicit query.</returns>
        public static IQueryable<T> CreateQuery<T>(Func<IQueryable> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new ConvertQueryable<T> { DataSource = query };
        }

        /// <summary>
        /// Attach additional query conditions to the original query.
        /// </summary>
        /// <param name="source">A data source of the original query.</param>
        /// <param name="query">The query conditions to be attached: ConvertQueryable type.</param>
        /// <returns>The data source after attaching the query conditions.</returns>
        public static IQueryable Attach(this IQueryable source, IQueryable query)
        {
            if (!(query is IElementTypeProvider))
                throw new ArgumentException("The query is not ConvertQueryable<T> type.");

            var attacher = new ExpressionAttacher();
            attacher.DataSource = source;

            var parser = attacher.Attach((query as IElementTypeProvider).OriginalElementType, query.Expression);
            return parser.DataSource;
        }

        /// <summary>
        /// Attach additional query conditions to the original query.
        /// </summary>
        /// <typeparam name="TOriginalElementType">The original element type of the query</typeparam>
        /// <param name="source">A data source of the original query.</param>
        /// <param name="query">The query conditions to be attached.</param>
        /// <returns>The data source after attaching the query conditions.</returns>
        public static IQueryable Attach<TOriginalElementType>(this IQueryable source, IQueryable query)
        {
            var attacher = new ExpressionAttacher();
            attacher.DataSource = source;

            var parser = attacher.Attach(typeof(TOriginalElementType), query.Expression);
            return parser.DataSource;
        }

        /// <summary>
        /// Get the first element that meets the conditions from a conversion query data source, or return <c>null</c> if no element meets the conditions.
        /// </summary>
        /// <param name="source">An interface for a conversion query data source.</param>
        /// <returns>The first element that meets the conditions.</returns>
        public static object FirstOrDefault(IQueryable source)
        {
            return Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// Get the number of elements that meet the conditions from a conversion query data source.
        /// </summary>
        /// <param name="source">An interface for a conversion query data source.</param>
        /// <returns>The number of elements that meet the conditions.</returns>
        public static int Count(IQueryable source)
        {
            return (int)Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// Get the number of elements that meet the conditions from a conversion query data source.
        /// </summary>
        /// <param name="source">An interface for a conversion query data source.</param>
        /// <returns>The number of elements that meet the conditions.</returns>
        public static long LongCount(IQueryable source)
        {
            return (long)Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// Determine whether there are elements that meet the conditions from a conversion query data source: <c>true</c> indicates existence.
        /// </summary>
        /// <param name="source">An interface for a conversion query data source.</param>
        /// <returns><c>true</c> indicates that there are elements that meet the conditions; otherwise, <c>false</c>.</returns>
        public static bool Any(IQueryable source)
        {
            return (bool)Execute(source, MethodInfo.GetCurrentMethod());
        }

        /// <summary>
        /// Get an array of elements that meet the conditions from a conversion query data source.
        /// </summary>
        /// <param name="source">An interface for a conversion query data source.</param>
        /// <returns>An array of elements that meet the conditions.</returns>
        public static object[] ToArray(IQueryable source)
        {
            return (Execute(source, MethodInfo.GetCurrentMethod()) as IEnumerable).OfType<object>().ToArray();
        }

        /// <summary>
        /// Convert the first parameter in the Lambda expression to type {T}.
        /// </summary>
        /// <typeparam name="T">The target element type to be converted.</typeparam>
        /// <param name="lambda">The original expression tree to be converted.</param>
        /// <returns>The target expression tree after conversion.</returns>
        public static Expression<Func<T, bool>> ChangeParameter<T>(this LambdaExpression lambda)
        {
            return ChangeParameter(lambda, typeof(T)) as Expression<Func<T, bool>>;
        }

        /// <summary>
        /// Convert the first parameter in the Lambda expression to type <param name="targetElementType"/>.
        /// </summary>
        /// <param name="lambda">The original expression tree to be converted.</param>
        /// <param name="targetElementType">The target element type to be converted.</param>
        /// <returns>The target expression tree after conversion.</returns>
        public static LambdaExpression ChangeParameter(this LambdaExpression lambda, Type targetElementType)
        {
            var originalParameter = lambda.Parameters[0];
            var parameterExpr = Expression.Parameter(targetElementType, originalParameter.Name);
            var builder = new ExpressionRewriteBuilder(originalParameter.Type, targetElementType);

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
        }

        private static object Execute(IQueryable source, MethodBase method)
        {
            return source.Provider.Execute(Expression.Call(null, (MethodInfo)method, new Expression[]
			{
				source.Expression
			}));
        }
    }

    internal class ConvertQueryable<T> : IOrderedQueryable<T>, IElementTypeProvider
    {
        public Func<IQueryable> DataSource { get; set; }

        public Type OriginalElementType { get; set; }

        public ConvertQueryable()
        {
            Expression = Expression.Constant(this);
            _provider = new Lazy<IQueryProvider>(() => new ConvertQueryProvider<T>
            {
                DataSource = DataSource,
                OriginalElementType = typeof(T),
            }, true);

            OriginalElementType = typeof(T);
        }

        public ConvertQueryable(Expression expression, Type originalElementType)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), "Query expression cannot be null.");

            Expression = expression;
            OriginalElementType = originalElementType;

            _provider = new Lazy<IQueryProvider>(() => new ConvertQueryProvider<T>
            {
                DataSource = DataSource,
                OriginalElementType = originalElementType,
            }, true);
        }

        #region IEnumerable<T> members

        public IEnumerator<T> GetEnumerator()
        {
            var enumerable = Provider.Execute<IEnumerable<T>>(Expression);
            return (enumerable ?? new T[0]).GetEnumerator();
        }

        #endregion IEnumerable<T> members

        #region IEnumerable members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
        }

        #endregion IEnumerable members

        #region IQueryable members

        public Type ElementType => typeof(T);

        public Expression Expression { get; private set; }

        public IQueryProvider Provider => _provider.Value;

        #endregion IQueryable members

        private Lazy<IQueryProvider> _provider;
    }
}
