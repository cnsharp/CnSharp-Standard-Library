using System;
using System.Collections.Generic;

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
}