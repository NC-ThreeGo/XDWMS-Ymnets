using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using Apps.IDAL;
using Apps.BLL.Core;
using Apps.Common;
using Apps.Models.Sys;
using Apps.Models;
using System.Transactions;
using Apps.IBLL;
using Apps.Locale;
namespace Apps.BLL.Sys
{
    public partial class SysLogBLL
    {
       
        public List<SysLogModel> GetListByUser(ref GridPager pager, string queryStr, string userId)
        {
            IQueryable<SysLog> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(a => a.Message.Contains(queryStr) || a.Module.Contains(queryStr) && a.Operator == userId);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
     
    }
}
