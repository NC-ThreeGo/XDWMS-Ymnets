﻿using Apps.Common;
using Apps.Models.WMS;
using System.Collections.Generic;

namespace Apps.IBLL.WMS
{
    public partial interface IWMS_CustomerBLL
    {
         /// <summary>
         /// 导入Excel文件，当发生导入错误时，回写错误信息，并且全部回滚。
         /// </summary>
         /// <param name="filePath"></param>
         /// <param name="errors"></param>
         /// <returns></returns>
         bool ImportExcelData(string oper, string filePath, ref ValidationErrors errors);

        /// <summary>
        /// 对导入进行附加的校验，例如物料编码是否存在等。
        /// </summary>
        /// <param name="model"></param>
        //void AdditionalCheckExcelData(WMS_CustomerModel model);

        /// 根据where字符串获取列表数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        List<WMS_CustomerModel> GetListByWhere(ref GridPager pager, string where);

        /// <summary>
        /// 根据所属客户代码列表，返回客户数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        List<WMS_CustomerModel> GetListByBelong(ref GridPager pager, string codes);
    }
}
