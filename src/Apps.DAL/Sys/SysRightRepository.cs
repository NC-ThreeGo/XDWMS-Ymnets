using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.IDAL;
using Apps.Models;
using Apps.Models.Sys;

namespace Apps.DAL.Sys
{
    public partial class SysRightRepository
    {
        public int UpdateRight(SysRightOperateModel model)
        {
            //转换
            SysRightOperate rightOperate = new SysRightOperate();
            rightOperate.Id = model.Id;
            rightOperate.RightId = model.RightId;
            rightOperate.KeyCode = model.KeyCode;
            rightOperate.IsValid = model.IsValid;
            //判断rightOperate是否存在，如果存在就更新rightOperate,否则就添加一条
            SysRightOperate right = Context.SysRightOperate.Where(a => a.Id == rightOperate.Id).FirstOrDefault();
                if (right != null)
                {
                    right.IsValid = rightOperate.IsValid;
                }
                else
                {
                    Context.SysRightOperate.Add(rightOperate);
                }
                if (Context.SaveChanges() > 0)
                {
                    //更新角色--模块的有效标志RightFlag
                    var sysRight = (from r in Context.SysRight
                                    where r.Id == rightOperate.RightId
                                    select r).First();
                    Context.P_Sys_UpdateSysRightRightFlag(sysRight.ModuleId, sysRight.RoleId);
                    return 1;
                }
            return 0;
        }

        public int UpdateDataRight(SysRightDataFilterModel model)
        {
            //转换
            SysRightDataFilter dataFilter = new SysRightDataFilter();
            dataFilter.Id = model.Id;
            dataFilter.RightId = model.RightId;
            dataFilter.KeyCode = model.KeyCode;
            dataFilter.IsValid = model.IsValid;
            //判断rightOperate是否存在，如果存在就更新rightOperate,否则就添加一条
            SysRightDataFilter right = Context.SysRightDataFilter.Where(a => a.Id == dataFilter.Id).FirstOrDefault();
            if (right != null)
            {
                right.IsValid = dataFilter.IsValid;
            }
            else
            {
                Context.SysRightDataFilter.Add(dataFilter);
            }
            if (Context.SaveChanges() > 0)
            {
                //更新角色--模块的有效标志RightFlag
                var sysRight = (from r in Context.SysRight
                                where r.Id == dataFilter.RightId
                                select r).First();
                Context.P_Sys_UpdateSysRightRightFlag(sysRight.ModuleId, sysRight.RoleId);
                return 1;
            }
            return 0;
        }

        //按选择的角色及模块加载模块的权限项
        public List<P_Sys_GetRightByRoleAndModule_Result> GetRightByRoleAndModule(string roleId, string moduleId)
        {
            List<P_Sys_GetRightByRoleAndModule_Result> result = null;
            result = Context.P_Sys_GetRightByRoleAndModule(roleId, moduleId).ToList();
            return result;
        }

        public List<P_Sys_GetRightDataByRoleAndModule_Result> GetRightDataByRoleAndModule(string roleId, string moduleId)
        {
            List<P_Sys_GetRightDataByRoleAndModule_Result> result = null;
            result = Context.P_Sys_GetRightDataByRoleAndModule(roleId, moduleId).ToList();
            return result;
        }
        /// <summary>
        /// 取角色模块的操作权限，用于权限控制
        /// </summary>
        /// <param name="accountid">acount Id</param>
        /// <param name="controller">url</param>
        /// <returns></returns>
        public List<permModel> GetPermission(string accountid, string controller) 
        {
                List<permModel> rights = (from r in Context.P_Sys_GetRightOperate(accountid, controller)
                                         select new permModel
                                         {
                                             KeyCode = r.KeyCode,
                                             IsValid = r.IsValid,
                                             Category = r.Category
                                         }).ToList();
                return rights;
            }
     
    }
}
