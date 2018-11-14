using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.Models;
using Apps.Common;
using System.Transactions;
using Apps.Models.Sys;
using Apps.IBLL;
using Apps.IDAL;
using Apps.BLL.Core;
using Apps.Locale;

namespace Apps.BLL.Sys
{
    public partial class SysRoleBLL
    {
     
        public override  List<SysRoleModel> CreateModelList(ref IQueryable<SysRole> queryData)
        {
            List<SysRoleModel> modelList = new List<SysRoleModel>();
            foreach (var r in queryData)
            {
                modelList.Add(new SysRoleModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreateTime = r.CreateTime,
                    CreatePerson = r.CreatePerson,
                    UserName = GetRefSysUser(r.Id)
                });
            }
            return modelList;
        }

        public override bool Create(ref ValidationErrors errors, SysRoleModel model)
        {
            try
            {
                SysRole entity = m_Rep.GetById(model.Id);
                if (entity != null)
                {
                    errors.Add(Resource.PrimaryRepeat);
                    return false;
                }
                entity = new SysRole();
                entity.Id = model.Id;
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.CreateTime = model.CreateTime;
                entity.CreatePerson = model.CreatePerson;
                if (m_Rep.Create(entity))
                {
                    //分配给角色
                    m_Rep.P_Sys_InsertSysRight();
                    //清理无用的项
                    m_Rep.P_Sys_ClearUnusedRightOperate();
                    return true;
                }
                else
                {
                    errors.Add(Resource.InsertFail);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }

         
        /// <summary>
        /// 获取角色对应的所有用户
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        public string GetRefSysUser(string roleId)
        {
            string UserName = "";
            var userList = m_Rep.GetRefSysUser(roleId);
            if (userList != null)
            {
                foreach (var user in userList)
                {
                    UserName += "[" + user.UserName + "] ";
                }
            }
            return UserName;
        }

        public IQueryable<P_Sys_GetUserByRoleId_Result> GetUserByRoleId(ref GridPager pager, string roleId, string depId, string queryStr)
        {
            IQueryable<P_Sys_GetUserByRoleId_Result> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetUserByRoleId(roleId, depId).Where(a => a.TrueName.Contains(queryStr));
                pager.totalRows = queryData.Count();
                queryData = m_Rep.GetUserByRoleId(roleId, depId).Where(a => a.TrueName.Contains(queryStr));
            }
            else
            {
                queryData = m_Rep.GetUserByRoleId(roleId, depId);
                pager.totalRows = queryData.Count();
                queryData = m_Rep.GetUserByRoleId(roleId, depId);
            }
            
            return queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
        }
        public bool UpdateSysRoleSysUser(string roleId, string[] userIds)
        {
            try
            {
                m_Rep.UpdateSysRoleSysUser(roleId, userIds);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
    }
}




