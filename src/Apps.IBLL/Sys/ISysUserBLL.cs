using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models.Sys;
using Apps.Common;
using Apps.Models;

namespace Apps.IBLL.Sys
{
   public partial interface ISysUserBLL
    {
        List<SysUserModel> GetList(ref GridPager pager, string queryStr, string depId);
        List<permModel> GetPermission(string accountid, string controller);
        List<SysUserModel> GetListByPosId(string posId);
        string GetRefSysRole(string userId);
        IQueryable<P_Sys_GetRoleByUserId_Result> GetRoleByUserId(ref GridPager pager, string roleId);
        List<SysUserModel> GetUserByDepId(ref GridPager pager, string depId, string queryStr);
        bool UpdateSysRoleSysUser(string userId, string[] roleIds);
        /// <summary>
        /// 创建新用户 controller必须初始化
        ///         model.Id = ResultHelper.NewId;
        ///         model.CreateTime = ResultHelper.NowTime;
        ///         model.Password = ValueConvert.MD5(model.Password);
        ///         model.CreatePerson = GetUserTrueName();
        ///         model.State = true;
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Edit(ref ValidationErrors errors, SysUserEditModel model);
        bool EditPwd(ref ValidationErrors errors, SysUserEditModel model);
        string GetNameById(string id);
        string GetTrueNames(string ids);
        List<SysUser> GetListBySelName(string name);
        List<SysUser> GetListByDepId(string id);
         
       /// <summary>
       /// 获取二级架构下的用户
       /// </summary>
       /// <returns></returns>
        List<SysOnlineUserModel> GetAllUsers();
        
    }
}
