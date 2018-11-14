using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.Models;
using Apps.Common;
using System.Transactions;
using Apps.Models.Sys;
using Apps.IBLL;
using Apps.IDAL;
using Apps.BLL.Core;
using Apps.Locale;

namespace Apps.BLL.Sys
{
    public partial class SysModuleDataFilterBLL
    {
        public override List<SysModuleDataFilterModel> GetList(ref GridPager pager, string mid)
        {

            IQueryable<SysModuleDataFilter> queryData = null;
            if (!string.IsNullOrEmpty(mid))
            {
                queryData = m_Rep.GetList(a => a.ModuleId==mid);
            }
            else
            {
                queryData = m_Rep.GetList(a => a.ModuleId == "xxxnull");
            }
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
    }
}
