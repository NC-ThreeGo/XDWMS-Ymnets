using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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

        List<WMS_Feed_ListModel> GetFeedList(ref GridPager pager);

        List<WMS_InvModel> InvAmount(ref GridPager pager, string partcode, string partname);

        List<WMS_AIModel> SupplierDelivery(ref GridPager pager, string po, string suppliername, string partcode, string partname, DateTime beginDate, DateTime endDate);

        List<WMS_Product_EntryModel> ReturnRate(ref GridPager pager, string partcode, string partname, DateTime beginDate, DateTime endDate, string returnRateType);

    }
}
