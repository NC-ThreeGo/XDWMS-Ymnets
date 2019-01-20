using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_Inv_AdjustRepository
    {
        /// <summary>
        /// 创建库存调账记录
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="partId"></param>
        /// <param name="invId"></param>
        /// <param name="adjustQty"></param>
        /// <param name="adjustType"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        string CreateInvAdjust(string opt, int? partId, int? invId, string lot, decimal? adjustQty, string adjustType, string remark, ref string invAdjustBillNum);
    }
}
