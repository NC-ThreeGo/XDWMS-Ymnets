using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.Common;
using Apps.Models.Sys;

namespace Apps.IBLL.Sys
{
   public partial interface ISysRightGetUserRightBLL
    {
        List<P_Sys_GetRightByUser_Result> GetList(string UserId);
    }
}
