using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.IDAL;
using Apps.Models;
using Apps.IDAL.Sys;

namespace Apps.DAL.Sys
{
    public class SysRightGetModuleRightRepository :BaseRepository<SysModuleOperate>, ISysRightGetModuleRightRepository,IDisposable
    {
        public SysRightGetModuleRightRepository(DBContainer db)
            : base(db)
        {
        
        }

        public List<P_Sys_GetModule_RoleRight_Result> GetModuleRoleRight(string moduleId)
        {
                var result = Context.P_Sys_GetModule_RoleRight(moduleId).ToList();
                return result;
        }
        public List<P_Sys_GetModule_UserRight_Result> GetModuleUserRight(string moduleId)
        {
            var result = Context.P_Sys_GetModule_UserRight(moduleId).ToList();
                return result;
        }
    }
  
}
