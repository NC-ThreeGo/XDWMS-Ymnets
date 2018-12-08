using Apps.Common;
using Apps.Models.WMS;
using System.Linq;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_AIBLL
    {
        /// <summary>
        /// 获取某个采购订单的到货信息
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="whereStr"></param>
        IQueryable<WMS_POForAIModel> GetPOListForAI(ref GridPager pager, string poNo);
    }
}
