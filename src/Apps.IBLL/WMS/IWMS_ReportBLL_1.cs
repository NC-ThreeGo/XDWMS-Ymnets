using System.Collections.Generic;
using System.Data;
using Apps.Common;
using Apps.Models.WMS;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_ReportBLL
    {
        /// <summary>
        /// 获取当前报表的数据源
        /// </summary>
        /// <returns></returns>
        DataSet GetDataSource(WMS_ReportModel report, List<WMS_ReportParamModel> listParam);
    }
}
