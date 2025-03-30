using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CnSharp.Tasks
{
    /// <summary>
    /// Retry method class
    /// </summary>
    public class Retry
    {
        #region Business Properties

        /// <summary>
        /// Gets the allowed number of retries.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Triggered after an exception occurs during execution.
        /// </summary>
        public Action<ExceptionContinueInfomation> AfterException { get; set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// Creates an instance of the <see cref="Retry"/> class.
        /// </summary>
        /// <param name="count">The number of allowed errors: default is 3.</param>
        /// <returns>An instance of the <see cref="Retry"/> class.</returns>
        public static Retry Create(int count = 3)
        {
            return new Retry { RetryCount = count };
        }

        private Retry()
        {
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// Executes the target logic in a loop within the allowed number of retries until successful.
        /// </summary>
        /// <param name="action">The target logic method.</param>
        /// <returns>A collection of exceptions that occurred during the execution of the target logic.</returns>
        public Exception[] Execute(Action action)
        {
            var dic = new Dictionary<int, Exception>();

            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    dic.Add(i, e);

                    if (AfterException != null)
                    {
                        try
                        {
                            var info = new ExceptionContinueInfomation(e);
                            AfterException(info);
                            if (!info.Continue)
                                break;
                        }
                        catch { }
                    }

                    continue;
                }

                break;
            }

            if (dic.Any() && dic.Count == RetryCount)
                throw new RetryException(dic.Values.ToArray());

            return dic.Values.ToArray();
        }

        #endregion Business Methods

        #region Inner Class

        /// <summary>
        /// Retry exception information class
        /// </summary>
        public class RetryException : Exception
        {
            /// <summary>
            /// A collection of exceptions that occurred during the execution of the target logic.
            /// </summary>
            public IList<Exception> InnerExceptions { get; private set; }

            internal RetryException(IEnumerable<Exception> exceptions)
                : base("A retry exception occurred in the system!", exceptions == null ? null : exceptions.FirstOrDefault())
            {
                InnerExceptions = exceptions == null ? new List<Exception>() : exceptions.ToList();
            }
        }

        /// <summary>
        /// Exception retry information
        /// </summary>
        public class ExceptionContinueInfomation
        {
            /// <summary>
            /// Gets the exception information that occurred.
            /// </summary>
            public Exception Exception { get; private set; }

            /// <summary>
            /// Gets or sets whether to continue.
            /// </summary>
            public bool Continue { get; set; }

            internal ExceptionContinueInfomation(Exception e)
            {
                Exception = e;
                Continue = true;
            }
        }

        #endregion Inner Class
    }

    /// <summary>
    /// Extension methods for the retry method class
    /// </summary>
    public static class RetryExtensions
    {
        /// <summary>
        /// Sleeps for a specified duration after an exception occurs during the retry process.
        /// </summary>
        /// <param name="retry">An instance of the <see cref="Retry"/> class.</param>
        /// <param name="timeout">The duration to sleep.</param>
        /// <returns>An instance of the <see cref="Retry"/> class.</returns>
        public static Retry SleepAfterException(this Retry retry, TimeSpan timeout)
        {
            retry.AfterException += p => Thread.Sleep(timeout);

            return retry;
        }
    }
}