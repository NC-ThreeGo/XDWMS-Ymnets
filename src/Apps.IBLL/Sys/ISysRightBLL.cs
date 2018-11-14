using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.Common;
using Apps.Models.Sys;

namespace Apps.IBLL.Sys
{
   public partial interface ISysRightBLL
    {
        int UpdateRight(SysRightOperateModel model);
        int UpdateDataRight(SysRightDataFilterModel model);
        List<P_Sys_GetRightByRoleAndModule_Result> GetRightByRoleAndModule(string roleId, string moduleId);
        List<P_Sys_GetRightDataByRoleAndModule_Result> GetRightDataByRoleAndModule(string roleId, string moduleId);
    }
}
