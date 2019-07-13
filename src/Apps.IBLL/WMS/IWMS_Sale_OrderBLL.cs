using System.Collections.Generic;
using Apps.Common;
using Apps.Models.WMS;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_Sale_OrderBLL
    {
         /// <summary>
         /// 导入Excel文件，当发生导入错误时，回写错误信息，并且全部回滚。
         /// </summary>
         /// <param name="oper">操作员ID</param>
         /// <param name="filePath"></param>
         /// <param name="errors"></param>
         /// <returns></returns>
         bool ImportExcelData(string oper, string filePath, ref ValidationErrors errors);
    
         /// <summary>
         /// 对导入进行附加的校验，例如物料编码是否存在等。
         /// </summary>
         /// <param name="model"></param>
         //void AdditionalCheckExcelData(ref WMS_Sale_OrderModel model);
    
         /// <summary>
         /// 根据where字符串获取列表数据。
         /// </summary>
         /// <param name="pager"></param>
         /// <param name="whereStr"></param>
         List<WMS_Sale_OrderModel> GetListByWhere(ref GridPager pager, string where);

        /// <summary>
        /// 根据where字符串获取按单据编号分组后的列表数据。
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        List<WMS_Sale_OrderModel> GetListByWhereAndGroupBy(ref GridPager pager, string where);

        /// <summary>
        /// 打印销售订单
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="opt"></param>
        /// <param name="saleBillNum"></param>
        /// <returns></returns>
        string PrintSaleOrder(ref ValidationErrors errors, string opt, string saleBillNum, int id);

        /// <summary>
        /// 取消打印备料销售订单
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="opt"></param>
        /// <param name="sellBillNum"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool UnPrintSaleOrder(ref ValidationErrors errors, string opt, string sellBillNum, int id);

        /// <summary>
        /// 确认销售订单
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="opt"></param>
        /// <param name="sellBillNum"></param>
        /// <returns></returns>
        bool ConfirmSaleOrder(ref ValidationErrors errors, string opt, string sellBillNum);
    }
}
