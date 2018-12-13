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
using System.Data;

namespace Apps.BLL.WMS
{
    public  partial class WMS_ReportBLL
    {
        /// <summary>
        /// 获取当前报表的数据源
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSource(WMS_ReportModel report, List<WMS_ReportParamModel> listParam)
        {
			return m_Rep.GetDataSource(report, listParam);
		}
    }
 }

