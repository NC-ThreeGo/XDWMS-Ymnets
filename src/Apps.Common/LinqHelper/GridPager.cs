using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps.Common
{
     public class GridPager
    {
        public int rows { get; set; }//每页行数
        public int page { get; set; }//当前页是第几页
        public string order { get; set; }//排序方式
        public string sort { get; set; }//排序列
        public int totalRows { get; set; }//总行数
        public int totalPages //总页数
        {
            get
            {
                return (int)Math.Ceiling((float)totalRows / (float)rows);
            }
        }
        public string filterRules { get; set; }
    }


     public class GridRows<T>
     {
        public List<T> rows { get; set; }

        public List<T> footer { get; set; }
        public int total { get; set; }

     }
}
