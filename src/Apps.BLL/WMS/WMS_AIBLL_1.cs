using Apps.Common;
using Apps.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.IO;
using LinqToExcel;
using ClosedXML.Excel;
using Apps.Models.WMS;
using System.Dynamic;

namespace Apps.BLL.WMS
{
    public  partial class WMS_AIBLL
    {
		public IQueryable<WMS_POForAIModel> GetPOListForAI(ref GridPager pager, string poNo)
        {
			IQueryable<WMS_POForAIModel> queryData = null;
			queryData = m_Rep.GetPOListForAI(poNo);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return queryData;
		}
    }
 }

