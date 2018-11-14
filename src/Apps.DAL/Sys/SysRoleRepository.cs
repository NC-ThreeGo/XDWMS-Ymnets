using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.IDAL;
using System.Data;

namespace Apps.DAL.Sys
{
    public partial class SysRoleRepository
    {


        public IQueryable<SysUser> GetRefSysUser( string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return from m in Context.SysRole
                       from f in m.SysUser
                       where m.Id == id
                       select f;
            }
            return null;
        }

        public IQueryable<P_Sys_GetUserByRoleId_Result> GetUserByRoleId(string roleId,string depId)
        {
            return Context.P_Sys_GetUserByRoleId(roleId, depId).AsQueryable();
        }
        
        public void UpdateSysRoleSysUser(string roleId,string[] userIds)
        { 
            using(DBContainer db = new DBContainer())
            {
                db.P_Sys_DeleteSysRoleSysUserByRoleId(roleId);
                foreach (string userid in userIds)
                {
                    if (!string.IsNullOrWhiteSpace(userid))
                    {
                        db.P_Sys_UpdateSysRoleSysUser(roleId, userid);
                    }
                }
                db.SaveChanges();
            }
        }

        public void P_Sys_InsertSysRight()
        {
            Context.P_Sys_InsertSysRight();
        }
        //清理无用的项
        public void P_Sys_ClearUnusedRightOperate() {
            Context.P_Sys_ClearUnusedRightOperate();
        }

    }
}
