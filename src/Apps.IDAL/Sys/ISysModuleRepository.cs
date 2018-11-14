using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;

namespace Apps.IDAL.Sys
{
    public partial interface ISysModuleRepository
    {
        int GetChildrenCount(string id);
        IQueryable<SysModule> GetModuleBySystem(string parentId);
         //分配给角色
        void P_Sys_InsertSysRight();
                    //清理无用的项
        void P_Sys_ClearUnusedRightOperate();
    }
}
