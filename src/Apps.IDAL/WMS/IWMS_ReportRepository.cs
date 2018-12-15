using Apps.Models.WMS;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_ReportRepository
    {
        /// <summary>
        /// 根据报表定义获取报表的数据源
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        DataSet GetDataSource(WMS_ReportModel report, List<WMS_ReportParamModel> listParam);
    }
}
