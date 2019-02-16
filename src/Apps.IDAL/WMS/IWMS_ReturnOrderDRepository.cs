using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_ReturnOrder_DRepository
    {
        /// <summary>
        /// 手工创建退货单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="partId"></param>
        /// <param name="supplierId"></param>
        /// <param name="invId"></param>
        /// <param name="qty"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        string CreateReturnOrder(string opt, int? partId, int? supplierId, int? invId, string lot, decimal? qty, string remark);

        /// <summary>
        /// 将所选择的退货单行生成退货单号，以便打印
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="jsonReturnOrder">所选择的退货单行</param>
        /// <returns></returns>
        string PrintReturnOrder(string opt, string jsonReturnOrder);

        /// <summary>
        /// 确认指定单号的退货单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="returnOrderNum"></param>
        void ConfirmReturnOrder(string opt, string returnOrderNum);
    }
}
