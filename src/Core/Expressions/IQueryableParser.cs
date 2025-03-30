using System.Linq;
using System.Linq.Expressions;

namespace CnSharp.Expressions
{
    /// <summary>
    /// Interface for query parser
    /// </summary>
    public interface IQueryableParser
    {
        #region Public Properties

        /// <summary>
        /// Gets the query data source.
        /// </summary>
        IQueryable DataSource { get; }

        /// <summary>
        /// Gets the name of the final method executed: <c>null</c> means no data query was executed.
        /// </summary>
        string FinalMethodName { get; }

        /// <summary>
        /// Gets the result of the final data query executed: if no method was executed, this value is <c>null</c>.
        /// </summary>
        object Value { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Analyzes and executes the query operation on the data source: according to the expression tree, if it does not contain the final execution method, it will only attach query conditions to the data source.
        /// </summary>
        /// <param name="methodCall"></param>
        void Build(MethodCallExpression methodCall);

        #endregion Public Methods
    }
}
