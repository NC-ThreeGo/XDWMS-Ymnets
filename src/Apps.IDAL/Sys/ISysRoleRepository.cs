using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;

namespace Apps.IDAL.Sys
{
    public partial interface ISysRoleRepository
    {
        IQueryable<SysUser> GetRefSysUser(string id);
        IQueryable<P_Sys_GetUserByRoleId_Result> GetUserByRoleId(string roleId,string DepId);
        void UpdateSysRoleSysUser(string roleId,string[] userIds);
        void P_Sys_InsertSysRight();
                    //清理无用的项
       void P_Sys_ClearUnusedRightOperate();
    }

}
