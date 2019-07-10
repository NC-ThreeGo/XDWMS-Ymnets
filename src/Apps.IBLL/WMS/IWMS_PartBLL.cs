using Apps.Common;
using Apps.Models.WMS;
using System.Collections.Generic;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_PartBLL
    {
         /// <summary>
         /// 导入Excel文件，当发生导入错误时，回写错误信息，并且全部回滚。
         /// </summary>
         /// <param name="filePath"></param>
         /// <param name="errors"></param>
         /// <returns></returns>
         bool ImportExcelData(string oper, string filePath, ref ValidationErrors errors);

        bool ImportSafeStock(string oper, string filePath, ref ValidationErrors errors);

        bool ImportBelongCustomer(string oper, string filePath, ref ValidationErrors errors);

        bool ImportBelongSupplier(string oper, string filePath, ref ValidationErrors errors);

        /// <summary>
        /// 对导入进行附加的校验，例如物料编码是否存在等。
        /// </summary>
        /// <param name="model"></param>
        //void AdditionalCheckExcelData(WMS_PartModel model);

        List<WMS_PartModel> GetList(ref GridPager pager, string partCode, string partName);

        /// <summary>
        /// 根据where字符串获取列表数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        List<WMS_PartModel> GetListByWhere(ref GridPager pager, string where);
    }
}
