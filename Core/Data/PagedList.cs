using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CnSharp.Data
{
    /// <summary>
    ///     List by paging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        #region Methods

        private void ComputePageCount()
        {
            if (_pageSize < 1)
            {
                throw new ArgumentException("pagesize should be natural number");
            }
            PageCount = (int) Math.Ceiling(_recordCount/(double) _pageSize);
        }

        #endregion

        #region Constants and Fields

        private int _pageSize;

        private int _recordCount;

        #endregion

        #region Constructors and Destructors

        public PagedList()
            : this(new List<T>(), 0, 10)
        {
        }

        public PagedList(List<T> contents, int recordCount, int pageSize)
        {
            ContentList = contents;
            RecordCount = recordCount;
            PageSize = pageSize;
        }

        #endregion

        #region Public Properties

        public List<T> ContentList { get; set; }

        public int PageCount { get; set; }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                ComputePageCount();
            }
        }

        public int RecordCount
        {
            get { return _recordCount; }
            set
            {
                _recordCount = value;
                if (_pageSize > 0)
                {
                    ComputePageCount();
                }
            }
        }

        #endregion
    }

    public static class PageHelper
    {
        /// <summary>
        ///     Perform custom paging using LINQ to SQL
        /// </summary>
        /// <typeparam name="T">Type of the Datasource to be paged</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj">Object to be paged through</param>
        /// <param name="page">Page Number to fetch</param>
        /// <param name="pageSize">Number of rows per page</param>
        /// <param name="keySelector">Sorting Expression</param>
        /// <param name="asc">Sort ascending if true. Otherwise descending</param>
        /// <param name="rowsCount">Output parameter hold total number of rows</param>
        /// <returns>Page of result from the paged object</returns>
        public static IQueryable<T> Page<T, TResult>(this IQueryable<T> obj, int page, int pageSize,
            Expression<Func<T, TResult>> keySelector, bool asc, out int rowsCount)
        {
            rowsCount = obj.Count();
            var innerRows = rowsCount - (page*pageSize);
            if (asc)
                return
                    obj.OrderByDescending(keySelector).Take(innerRows).OrderBy(keySelector).Take(pageSize).AsQueryable();
            return obj.OrderBy(keySelector).Take(innerRows).OrderByDescending(keySelector).Take(pageSize).AsQueryable();
        }
    }
}