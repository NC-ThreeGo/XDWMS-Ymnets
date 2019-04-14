using System.Collections.Generic;
using Apps.Common;
using Apps.Models.WMS;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_ReturnInspectionBLL
    {
        /// <summary>
        /// 批量创建退货检验单
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="opt"></param>
        /// <param name="jsonReturnInspection"></param>
        /// <returns>退货检验单单据号</returns>
        string CreateBatchReturnInspection(ref ValidationErrors errors, string opt, string jsonReturnInspection);

        /// <summary>
        /// 输入检验结果，并把合格数量入库，不合格数量暂存到退货单上。
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="opt"></param>
        /// <param name="jsonReturnInspectBill"></param>
        /// <returns></returns>
        bool ProcessReturnInspectBill(ref ValidationErrors errors, string opt, string jsonReturnInspectBill);

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
         void AdditionalCheckExcelData(ref WMS_ReturnInspectionModel model);
    
         /// <summary>
         /// 根据where字符串获取列表数据。
         /// </summary>
         /// <param name="pager"></param>
         /// <param name="whereStr"></param>
         List<WMS_ReturnInspectionModel> GetListByWhere(ref GridPager pager, string where);
    }
}
