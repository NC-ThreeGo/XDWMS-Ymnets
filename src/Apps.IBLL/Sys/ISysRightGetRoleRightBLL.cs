using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.Common;
using Apps.Models.Sys;

namespace Apps.IBLL.Sys
{
   public partial interface ISysRightGetRoleRightBLL
    {
        List<P_Sys_GetRightByRole_Result> GetList(string roleId);
    }
}
