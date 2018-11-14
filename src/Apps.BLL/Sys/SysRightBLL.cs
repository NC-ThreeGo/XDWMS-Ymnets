using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Unity.Attributes;
using Apps.IDAL;
using Apps.IBLL;
using Apps.Common;
using Apps.BLL.Core;
using Apps.Models.Sys;
using Apps.Locale;
using Apps.IDAL.Sys;

namespace Apps.BLL.Sys
{
    public partial class SysRightBLL
    {
        
        [Dependency]
        public ISysModuleRepository SysModuleRepository { get; set; }

        public int UpdateRight(SysRightOperateModel model)
        {
            return m_Rep.UpdateRight(model);
        }

        public int UpdateDataRight(SysRightDataFilterModel model)
        {
            return m_Rep.UpdateDataRight(model);
        }

        public List<P_Sys_GetRightByRoleAndModule_Result> GetRightByRoleAndModule(string roleId, string moduleId)
        {
            return m_Rep.GetRightByRoleAndModule(roleId, moduleId);
        }

        public List<P_Sys_GetRightDataByRoleAndModule_Result> GetRightDataByRoleAndModule(string roleId, string moduleId)
        {
            return m_Rep.GetRightDataByRoleAndModule(roleId, moduleId);
        }

    }
}
