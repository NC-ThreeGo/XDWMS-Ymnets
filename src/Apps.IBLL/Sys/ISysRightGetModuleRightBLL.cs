using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Common;
using Apps.Models;

namespace Apps.IBLL.Sys
{
   public partial interface ISysRightGetModuleRightBLL
    {
        object GetPerantModule(GridPager pager, string nodeid, string parentid, int? n_level);
        List<SysModuleOperate> GetModuleOperateByModuleId(string moduleId);
        List<P_Sys_GetModule_UserRight_Result> GetModuleUserRight(string moduleId);
        List<P_Sys_GetModule_RoleRight_Result> GetModuleRoleRight(string moduleId);
    }
}
