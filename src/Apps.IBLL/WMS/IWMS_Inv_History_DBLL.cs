using System.Collections.Generic;
using Apps.Common;
using Apps.Models.WMS;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_Inv_History_DBLL
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
         void AdditionalCheckExcelData(ref WMS_Inv_History_DModel model);
    
         /// <summary>
         /// 根据where字符串获取列表数据。
         /// </summary>
         /// <param name="pager"></param>
         /// <param name="whereStr"></param>
         List<WMS_Inv_History_DModel> GetListByWhere(ref GridPager pager, string where);

        /// <summary>
        /// 获取库存历史头表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        List<WMS_Inv_History_HModel> GetListParent(ref GridPager pager, string where);
    }
}
