using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_ReturnOrderRepository
    {
        /// <summary>
        /// 批量保存退货单，并返回退货单号，以便打印
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="jsonReturnOrder"></param>
        /// <returns></returns>
        string CreateBatchReturnOrder(string opt, string jsonReturnOrder);
    }
}
