using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.IDAL;
using Apps.Models;
using System.Data;
using System.Data.SqlClient;

namespace Apps.DAL.Sys
{
    public partial class SysUserRepository
    {
        public int GetUserCountByDepId(string depId)
        {
            return Context.P_Sys_GetUserCountByDepId(depId).Cast<int>().First();
        }

        public List<string> GetHouseList(string sysUserId)
        {
            SqlParameter[] para = new SqlParameter[]
            {
                   new SqlParameter("@UserId",sysUserId),

            };
            //return  Context.Database.SqlQuery<string>(@"select distinct b.Id from dbo.SysRoleSpl_Warehouse a join Spl_Warehouse b on a.Spl_WarehouseId=b.Id
            //                                            where a.SysRoleId in (select Id from SysRole a join
            //                                            SysRoleSysUser b
            //                                            on a.Id = b.SysRoleId
            //                                            and b.SysUserId = @UserId)",para).ToList();
            //返回所有仓库，不受权限控制
            return Context.Database.SqlQuery<string>(@"SELECT Id FROM Spl_Warehouse").ToList();
        }
        public IQueryable<SysRole> GetRefSysRole(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return from m in Context.SysUser
                       from f in m.SysRole
                       where m.Id == id
                       select f;
            }
            return null;
        }

        public IQueryable<P_Sys_GetRoleByUserId_Result> GetRoleByUserId(string userId)
        {
            return Context.P_Sys_GetRoleByUserId(userId).AsQueryable();
        }
        public IQueryable<SysUser> GetListByPosId(string posId)
        {
            return Context.SysUser.Where(a => a.PosId == posId);
        }
        public IQueryable<P_Sys_GetUserByDepId_Result> GetUserByDepId(string depId)
        {
            return Context.P_Sys_GetUserByDepId(depId).AsQueryable();
        }

        public void UpdateSysRoleSysUser(string userId, string[] roleIds)
        {
            Context.P_Sys_DeleteSysRoleSysUserByUserId(userId);
            foreach (string roleid in roleIds)
            {
                if (!string.IsNullOrWhiteSpace(roleid))
                {
                        Context.P_Sys_UpdateSysRoleSysUser(roleid,userId);
                 }
            }
            this.SaveChanges();
        }

       public IQueryable<P_Sys_GetAllUsers_Result> GetAllUsers()
       {
           return Context.P_Sys_GetAllUsers().AsQueryable();
       }



       /// <summary>
        /// 根据ID获取一个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameById(string id)
        {
           SysUser user = this.GetById(id);
           if (user != null)
           {
               return user.TrueName;
            }
            return "";
        }

    }
}
