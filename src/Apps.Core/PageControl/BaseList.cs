using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps.Core.PageControl
{
    public class BaseList<T> : List<T>, IPagedList
    {
        public BaseList(int pageIndex, int pageSize, int totalItemCount, int totalPage, IEnumerable<T> items)
        {
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            TotalPage = totalPage;
            AddRange(items);
        }

        public int CurrentPageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalItemCount { get; set; }
        public int TotalPage { get; set; }

    }
}
