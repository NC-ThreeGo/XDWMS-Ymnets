using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.Common;
using Apps.Models.Sys;

namespace Apps.IBLL.Sys
{
   public partial interface ISysRoleBLL
    {
  
        IQueryable<P_Sys_GetUserByRoleId_Result> GetUserByRoleId(ref GridPager pager, string roleId, string depId, string queryStr);
        bool UpdateSysRoleSysUser(string roleId, string[] userIds);
        string GetRefSysUser(string roleId);
    }
}
