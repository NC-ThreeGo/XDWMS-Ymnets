using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;

namespace Apps.IDAL.Sys
{
    public partial interface ISysUserRepository
    {
        List<string> GetHouseList(string sysUserId);
        int GetUserCountByDepId(string depId);
      
        IQueryable<SysUser> GetListByPosId(string posId);
        IQueryable<SysRole> GetRefSysRole(string id);
        IQueryable<P_Sys_GetRoleByUserId_Result> GetRoleByUserId( string userId);
        IQueryable<P_Sys_GetUserByDepId_Result> GetUserByDepId(string DepId);
        void UpdateSysRoleSysUser(string userId, string[] roleIds);
        string GetNameById(string id);
        /// <summary>
        /// 获取二级架构下的所有用户
        /// </summary>
        /// <returns></returns>
        IQueryable<P_Sys_GetAllUsers_Result> GetAllUsers();
    }
}
