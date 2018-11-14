using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using Apps.Models.Spl;
using Apps.Models.Sys;
using System;
using Apps.BLL.Core;
using Apps.IDAL.Sys;
using Unity.Attributes;

namespace Apps.BLL.Spl
{
    public  partial class Spl_WarehouseBLL
    {
        [Dependency]
        public ISysUserRepository userBLL { get; set; }

        public List<Spl_WarehouseModel> GetList(ref GridPager pager, string queryStr,string sysUserId)
        {
            List<string> houseList = userBLL.GetHouseList(sysUserId);
            IQueryable<Spl_Warehouse> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                                a => (a.Id.Contains(queryStr)
                                || a.Name.Contains(queryStr)
                                || a.Code.Contains(queryStr)

                                || a.ContactPerson.Contains(queryStr)
                                || a.ContactPhone.Contains(queryStr)
                                || a.Address.Contains(queryStr)
                                || a.Remark.Contains(queryStr)


                                || a.WarehouseCategoryId.Contains(queryStr))&& houseList.Contains(a.Id)
                                );
            }
            else
            {
                queryData = m_Rep.GetList(a=>houseList.Contains(a.Id));
            }
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<Spl_WarehouseModel> CreateModelList(ref IQueryable<Spl_Warehouse> queryData)
        {

            List<Spl_WarehouseModel> modelList = (from r in queryData
                                              select new Spl_WarehouseModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  Code = r.Code,
                                                  IsDefault = r.IsDefault,
                                                  ContactPerson = r.ContactPerson,
                                                  ContactPhone = r.ContactPhone,
                                                  Address = r.Address,
                                                  Remark = r.Remark,
                                                  Enable = r.Enable,
                                                  CreateTime = r.CreateTime,
                                                 
                                                  WarehouseCategoryName = r.Spl_WarehouseCategory.Name,
                                              }).ToList();

            foreach (var r in modelList)
            {
                r.RoleName = GetRefSysRole(r.Id);
            }
            return modelList;
        }

        public string GetRefSysRole(string userId)
        {
            string RoleName = "";
            var roleList = m_Rep.GetRefSysRole(userId);
            if (roleList != null)
            {
                foreach (var role in roleList)
                {
                    RoleName += "[" + role.Name + "] ";
                }
            }
            return RoleName;
        }

        public IQueryable<GetRoleByUserIdResultModel> GetRoleByUserId(ref GridPager pager, string userId)
        {
            IQueryable<GetRoleByUserIdResultModel> queryData = m_Rep.GetRoleByUserId(userId);
            pager.totalRows = queryData.Count();
            queryData = m_Rep.GetRoleByUserId(userId);
            return queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
        }

        public bool UpdateSysRoleSpl_Warehouse(string userId, string[] roleIds)
        {
            try
            {
                m_Rep.UpdateSysRoleSpl_Warehouse(userId, roleIds);
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

