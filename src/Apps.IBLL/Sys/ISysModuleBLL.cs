using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.Common;
using Apps.Models.Sys;

namespace Apps.IBLL.Sys
{
   public partial interface ISysModuleBLL
    {
        List<SysModuleModel> GetList(string parentId);
        List<SysModule> GetModuleBySystem(string parentId);

        bool DeleteAndClear(ref ValidationErrors errors, string id);
    }
}
