namespace CnSharp.Data
{
    public abstract class BaseCriteria
    {
        private int _pageIndex;
        private int _pageSize;
        private int _startIndex = 1;
        protected int DefaultPageSize = 10;

        public int PageIndex
        {
            get { return _pageIndex < 1 ? 1 : _pageIndex; }
            set
            {
                if (value < 1)
                    value = 1;
                _pageIndex = value;
                Compute();
            }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value < 1)
                    value = DefaultPageSize;
                _pageSize = value;
                Compute();
            }
        }

        public int RecordCount { get; set; }

        public int StartIndex
        {
            get { return _startIndex; }
            set
            {
                if (value < 0)
                    value = 0;
                _startIndex = value;
                Compute();
            }
        }

        public int EndIndex
        {
            get { return StartIndex + PageSize -1 ; }
        }

        private void Compute()
        {
            if (PageSize < 1)
                return;
            if (_startIndex >= 0)
                _pageIndex = _startIndex/PageSize + 1;
            else if (_pageIndex > 0)
                _startIndex = (_pageIndex - 1)*PageSize + 1;
        }
    }
}