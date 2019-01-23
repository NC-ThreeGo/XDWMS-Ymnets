using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_Sale_OrderRepository
    {
        /// <summary>
        /// 打印销售订单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="saleBillNum"></param>
        /// <param name="sellBillNum"></param>
        /// <returns></returns>
        string PrintSaleOrder(string opt, string saleBillNum, int id, ref string sellBillNum);

        /// <summary>
        /// 取消打印备料销售订单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="sellBillNum"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        string UnPrintSaleOrder(string opt, string sellBillNum, int id);

        /// <summary>
        /// 确认销售订单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="sellBillNum"></param>
        void ConfirmSaleOrder(string opt, string sellBillNum);
    }
}
