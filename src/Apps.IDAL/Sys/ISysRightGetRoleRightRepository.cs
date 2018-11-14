using System;
using Apps.Models;
using System.Collections.Generic;
using System.Linq;
namespace Apps.IDAL.Sys
{
    public interface ISysRightGetRoleRightRepository
    {
        List<P_Sys_GetRightByRole_Result> GetList(string roleId);
    }
}
