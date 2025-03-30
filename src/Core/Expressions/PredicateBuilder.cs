using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CnSharp.Expressions
{
    #region PredicateBuilder

    /// <summary>
    /// Predicate Builder
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Combines two Expressions using Expression.AndAlso.
        /// </summary>
        /// <typeparam name="T">The type of elements in the expression</typeparam>
        /// <param name="left">The left Expression</param>
        /// <param name="right">The right Expression</param>
        /// <returns>The combined Expression</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return MakeBinary(left, right, Expression.AndAlso);
        }

        /// <summary>
        /// False clause
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        /// <summary>
        /// Combines two Expressions.
        /// </summary>
        /// <typeparam name="T">The type of elements in the expression</typeparam>
        /// <param name="left">The left Expression</param>
        /// <param name="right">The right Expression</param>
        /// <param name="func">The specific logic for combining the expressions</param>
        /// <returns>The combined Expression</returns>
        public static Expression<Func<T, bool>> MakeBinary<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right, Func<Expression, Expression, Expression> func)
        {
            var data = Combinate(right.Parameters, left.Parameters).ToArray();
            if (left.Parameters.Count != right.Parameters.Count || !data.All(p => p.Key.Type == p.Value.Type))
                throw new InvalidOperationException("Parameter error.");

            right = ParameterReplace.Replace(right, data) as Expression<Func<T, bool>>;
            return Expression.Lambda<Func<T, bool>>(func(left.Body, right.Body), left.Parameters);
        }

        /// <summary>
        /// Combines two Expressions using Expression.Or.
        /// </summary>
        /// <typeparam name="T">The type of elements in the expression</typeparam>
        /// <param name="left">The left Expression</param>
        /// <param name="right">The right Expression</param>
        /// <returns>The combined Expression</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return MakeBinary(left, right, Expression.OrElse);
        }

        /// <summary>
        /// True clause
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        private static IEnumerable<KeyValuePair<T, T>> Combinate<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            var a = left.GetEnumerator();
            var b = right.GetEnumerator();
            while (a.MoveNext() && b.MoveNext())
                yield return new KeyValuePair<T, T>(a.Current, b.Current);
        }
    }

    #endregion PredicateBuilder

    #region class: ParameterReplace

    /// <summary>
    /// Parameter Replace
    /// </summary>
    public class ParameterReplace : ExpressionVisitor
    {
        private Dictionary<ParameterExpression, ParameterExpression> parameters = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterReplace"/> class.
        /// </summary>
        /// <param name="paramList">The param list.</param>
        public ParameterReplace(IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            parameters = paramList.ToDictionary(p => p.Key, p => p.Value, new ParameterEquality());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterReplace"/> class.
        /// </summary>
        private ParameterReplace()
        {
        }

        /// <summary>
        /// Replaces the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="paramList">The param list.</param>
        /// <returns></returns>
        public static Expression Replace(Expression e, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            var item = new ParameterReplace(paramList);
            return item.Visit(e);
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression result;
            if (parameters.TryGetValue(p, out result))
                return result;
            else
                return base.VisitParameter(p);
        }

        #region class: ParameterEquality

        private class ParameterEquality : IEqualityComparer<ParameterExpression>
        {
            public bool Equals(ParameterExpression x, ParameterExpression y)
            {
                if (x == null || y == null)
                    return false;

                return x.Type == y.Type;
            }

            public int GetHashCode(ParameterExpression obj)
            {
                if (obj == null)
                    return 0;

                return obj.Type.GetHashCode();
            }
        }

        #endregion class: ParameterEquality
    }

    #endregion class: ParameterReplace
}
