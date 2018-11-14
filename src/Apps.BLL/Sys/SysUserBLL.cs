using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.BLL.Core;
using Apps.IBLL;
using Unity.Attributes;
using Apps.IDAL;
using Apps.Models.Sys;
using Apps.Common;
using Apps.Models;
using System.Transactions;
using Apps.Locale;
using Apps.IDAL.Sys;

namespace Apps.BLL.Sys
{
    public partial class SysUserBLL
    {
        [Dependency]
        public ISysRightRepository sysRightRep { get; set; }
        [Dependency]
        public ISysStructRepository structRep { get; set; }
        [Dependency]
        public ISysPositionRepository posRep { get; set; }
        public List<permModel> GetPermission(string accountid, string controller)
        {
            return sysRightRep.GetPermission(accountid,controller);
        }

        public  List<SysUserModel> GetList(ref GridPager pager,string queryStr,string depId)
        {

            List<SysUser> query = null;
            IQueryable<SysUser> list = m_Rep.GetList();
            pager.totalRows = list.Count();
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                list = list.Where(a => a.UserName.Contains(queryStr) || a.TrueName.Contains(queryStr));
            }
            //根据部门来查询
            if (!string.IsNullOrWhiteSpace(depId) && depId!="root")
            {
                list = list.Where(a => a.DepId== depId);
               
            }
            if (pager.order == "desc")
            {
                if (pager.order == "UserName")
                {
                    query = list.OrderBy(c => c.UserName).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                }
                else//createtime
                {
                    query = list.OrderBy(c => c.CreateTime).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                }
            }
            else
            {
                if (pager.order == "UserName")
                {
                    query = list.OrderByDescending(c => c.UserName).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                }
                else//createtime
                {
                    query = list.OrderByDescending(c => c.CreateTime).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                }
            }
           
            List<SysUserModel> userInfoList = new List<SysUserModel>();
            List<SysUser> dataList = query.ToList();
            foreach (var user in dataList)
            {
                SysUserModel userModel = new SysUserModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Password = user.Password,
                    TrueName = user.TrueName,
                    MobileNumber = user.MobileNumber,
                    PhoneNumber = user.PhoneNumber,
                    QQ = user.QQ,
                    EmailAddress = user.EmailAddress,
                    OtherContact = user.OtherContact,
                    Province = user.Province,
                    City = user.City,
                    Village = user.Village,
                    Address = user.Address,
                    State = user.State,
                    CreateTime = user.CreateTime,
                    CreatePerson = user.CreatePerson,
                    RoleName = GetRefSysRole(user.Id),
                    PosName = user.SysPosition.Name,
                    DepName = user.SysStruct.Name
                };
                userInfoList.Add(userModel);
            }

            return userInfoList;
        }

        public List<SysUserModel> GetListByPosId(string posId)
        {
            IQueryable<SysUser> list = m_Rep.GetListByPosId(posId);
            List<SysUserModel> modelList = (from r in list
                                            select new SysUserModel
                                            {
                                                Id = r.Id,
                                                UserName = r.UserName,
                                                Password = r.Password,
                                                TrueName = r.TrueName,
                                                Card = r.Card,
                                                MobileNumber = r.MobileNumber,
                                                PhoneNumber = r.PhoneNumber,
                                                QQ = r.QQ,
                                                EmailAddress = r.EmailAddress,
                                                OtherContact = r.OtherContact,
                                                Province = r.Province,
                                                City = r.City,
                                                Village = r.Village,
                                                Address = r.Address,
                                                State = r.State,
                                                CreateTime = r.CreateTime,
                                                CreatePerson = r.CreatePerson,
                                                Sex = r.Sex,
                                                Birthday = r.Birthday,
                                                JoinDate = r.JoinDate,
                                                Marital = r.Marital,
                                                Political = r.Political,
                                                Nationality = r.Nationality,
                                                Native = r.Native,
                                                School = r.School,
                                                Professional = r.Professional,
                                                Degree = r.Degree,
                                                DepId = r.DepId,
                                                PosId = r.PosId,
                                                Expertise = r.Expertise,
                                                JobState = r.JobState,
                                                Photo = r.Photo,
                                                Attach = r.Attach,
                                                Lead = r.Lead,
                                                LeadName = r.LeadName,
                                                IsSelLead = r.IsSelLead,
                                                IsReportCalendar = r.IsReportCalendar,
                                                IsSecretary = r.IsSecretary
                                            }).ToList();
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
        public string GetTrueNames(string ids)
        {
            string[] keys = ids.Split(',');
            using (DBContainer db = new DBContainer())
            {
                List<string> list = db.SysUser.Where(a => keys.Contains(a.Id)).Select(a => a.TrueName).ToList();
                return GetArrayStr(list, ",");
            }
        }                                                                                                                                                                                                                                                                                                                                          
        public string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }
        public IQueryable<P_Sys_GetRoleByUserId_Result> GetRoleByUserId(ref GridPager pager, string userId)
        {
            IQueryable<P_Sys_GetRoleByUserId_Result> queryData = m_Rep.GetRoleByUserId(userId);
            pager.totalRows = queryData.Count();
            queryData = m_Rep.GetRoleByUserId(userId);
            return queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
        }

        public List<SysUserModel> GetUserByDepId(ref GridPager pager, string depId, string queryStr)
        {
            IQueryable<P_Sys_GetUserByDepId_Result> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetUserByDepId(depId).Where(a => a.TrueName.Contains(queryStr));
                pager.totalRows = queryData.Count();
                queryData = m_Rep.GetUserByDepId(depId).Where(a => a.TrueName.Contains(queryStr));
            }
            else
            {
                queryData = m_Rep.GetUserByDepId(depId);
                pager.totalRows = queryData.Count();
                queryData = m_Rep.GetUserByDepId(depId);
            }
           
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);

        }
        private List<SysUserModel> CreateModelList(ref IQueryable<P_Sys_GetUserByDepId_Result> queryData)
        {
            List<SysUserModel> modelList = (from r in queryData
                                            select new SysUserModel
                                            {
                                                Id = r.Id,
                                                UserName = r.UserName,
                                                Password = r.Password,
                                                TrueName = r.TrueName,
                                                Card = r.Card,
                                                MobileNumber = r.MobileNumber,
                                                PhoneNumber = r.PhoneNumber,
                                                QQ = r.QQ,
                                                EmailAddress = r.EmailAddress,
                                                OtherContact = r.OtherContact,
                                                Province = r.Province,
                                                City = r.City,
                                                Village = r.Village,
                                                Address = r.Address,
                                                State = r.State,
                                                CreateTime = r.CreateTime,
                                                CreatePerson = r.CreatePerson,
                                                Sex = r.Sex,
                                                Birthday = r.Birthday,
                                                JoinDate = r.JoinDate,
                                                Marital = r.Marital,
                                                Political = r.Political,
                                                Nationality = r.Nationality,
                                                Native = r.Native,
                                                School = r.School,
                                                Professional = r.Professional,
                                                Degree = r.Degree,
                                                DepId = r.DepId,
                                                PosId = r.PosId,
                                                Expertise = r.Expertise,
                                                JobState = r.JobState,
                                                Photo = r.Photo,
                                                Attach = r.Attach,
                                                Lead = r.Lead,
                                                LeadName = r.LeadName,
                                                IsSelLead = r.IsSelLead,
                                                IsReportCalendar = r.IsReportCalendar,
                                                IsSecretary = r.IsSecretary
                                            }).ToList();
            foreach (var v in modelList)
            {
                v.DepName = structRep.GetById(v.DepId).Name;
                v.PosName = posRep.GetById(v.PosId).Name;
            }
            return modelList;
        }
        public bool UpdateSysRoleSysUser(string userId, string[] roleIds)
        {
            try
            {
                m_Rep.UpdateSysRoleSysUser(userId, roleIds);
                return true;

            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return false;
            }
            
        }


        public override bool Create(ref ValidationErrors errors, SysUserModel model)
        {
            try
            {
                if (m_Rep.GetList(a => a.UserName == model.UserName).Count()>0)
                {
                    errors.Add("用户名已经存在！");
                    return false;
                }
                SysUser entity = new SysUser();
                entity.Id = model.Id;
                entity.UserName = model.UserName;
                entity.Password = model.Password;
                entity.TrueName = model.TrueName;
                entity.Card = model.Card;
                entity.MobileNumber = model.MobileNumber;
                entity.PhoneNumber = model.PhoneNumber;
                entity.QQ = model.QQ;
                entity.EmailAddress = model.EmailAddress;
                entity.OtherContact = model.OtherContact;
                entity.Province = model.Province;
                entity.City = model.City;
                entity.Village = model.Village;
                entity.Address = model.Address;
                entity.State = model.State;
                entity.CreateTime = model.CreateTime;
                entity.CreatePerson = model.CreatePerson;
                entity.Sex = model.Sex;
                entity.Birthday = model.Birthday;
                entity.JoinDate = model.JoinDate;
                entity.Marital = model.Marital;
                entity.Political = model.Political;
                entity.Nationality = model.Nationality;
                entity.Native = model.Native;
                entity.School = model.School;
                entity.Professional = model.Professional;
                entity.Degree = model.Degree;
                entity.DepId = model.DepId;
                entity.PosId = model.PosId;
                entity.Expertise = model.Expertise;
                entity.JobState = model.JobState;
                entity.Photo = model.Photo;
                entity.Attach = model.Attach;
                entity.Lead = model.Lead;
                entity.LeadName = model.LeadName;
                entity.IsSelLead = model.IsSelLead;
                entity.IsReportCalendar = model.IsReportCalendar;
                entity.IsSecretary = model.IsSecretary;


                if (m_Rep.Create(entity))
                {
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

        public bool Edit(ref ValidationErrors errors, SysUserEditModel model)
        {
            try
            {
                SysUser entity = m_Rep.GetById(model.Id);
                if (entity == null)
                {
                    errors.Add(Resource.Disable);
                    return false;
                }
                entity.TrueName = model.TrueName;
                entity.Card = model.Card;
                entity.MobileNumber = model.MobileNumber;
                entity.PhoneNumber = model.PhoneNumber;
                entity.QQ = model.QQ;
                entity.EmailAddress = model.EmailAddress;
                entity.OtherContact = model.OtherContact;
                entity.Province = model.Province;
                entity.City = model.City;
                entity.Village = model.Village;
                entity.Address = model.Address;
                entity.State = model.State;
                entity.CreateTime = model.CreateTime;
                entity.CreatePerson = model.CreatePerson;
                entity.Sex = model.Sex;
                entity.Birthday = ResultHelper.StringConvertDatetime(model.Birthday);
                entity.JoinDate = ResultHelper.StringConvertDatetime(model.JoinDate);
                entity.Marital = model.Marital;
                entity.Political = model.Political;
                entity.Nationality = model.Nationality;
                entity.Native = model.Native;
                entity.School = model.School;
                entity.Professional = model.Professional;
                entity.Degree = model.Degree;
                entity.DepId = model.DepId;
                entity.PosId = model.PosId;
                entity.Expertise = model.Expertise;
                entity.JobState = model.JobState;
                entity.Photo = model.Photo;
                entity.Attach = model.Attach;
                entity.Lead = model.Lead;
                entity.LeadName = model.LeadName;
                entity.IsSelLead = model.IsSelLead;
                entity.IsReportCalendar = model.IsReportCalendar;
                entity.IsSecretary = model.IsSecretary;

                if (m_Rep.Edit(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.NoDataChange);
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

        public bool EditPwd(ref ValidationErrors errors, SysUserEditModel model)
        {
            try
            {
                SysUser entity = m_Rep.GetById(model.Id);
                if (entity == null)
                {
                    errors.Add(Resource.Disable);
                    return false;
                }
              
                entity.Password = model.Password;

                if (m_Rep.Edit(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.NoDataChange);
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
        public override SysUserModel GetById(object id)
        {
            
                SysUser entity = m_Rep.GetById(id);
                SysUserModel model = new SysUserModel();
                model.Id = entity.Id;
                model.UserName = entity.UserName;
                model.Password = entity.Password;
                model.TrueName = entity.TrueName;
                model.Card = entity.Card;
                model.MobileNumber = entity.MobileNumber;
                model.PhoneNumber = entity.PhoneNumber;
                model.QQ = entity.QQ;
                model.EmailAddress = entity.EmailAddress;
                model.OtherContact = entity.OtherContact;
                model.Province = entity.Province;
                model.City = entity.City;
                model.Village = entity.Village;
                model.Address = entity.Address;
                model.State = entity.State;
                model.CreateTime = entity.CreateTime;
                model.CreatePerson = entity.CreatePerson;
                model.Sex = entity.Sex;
                model.Birthday = entity.Birthday;
                model.JoinDate = entity.JoinDate;
                model.Marital = entity.Marital;
                model.Political = entity.Political;
                model.Nationality = entity.Nationality;
                model.Native = entity.Native;
                model.School = entity.School;
                model.Professional = entity.Professional;
                model.Degree = entity.Degree;
                model.DepId = entity.DepId;
                model.DepName = entity.SysStruct.Name; 
                model.PosId = entity.PosId;
                model.PosName = entity.SysPosition.Name;
                model.Expertise = entity.Expertise;
                model.JobState = entity.JobState;
                model.Photo = entity.Photo;
                model.Attach = entity.Attach;
                model.Lead = entity.Lead;
                model.LeadName = entity.LeadName;
                model.IsSelLead = entity.IsSelLead;
                model.IsReportCalendar = entity.IsReportCalendar;
                model.IsSecretary = entity.IsSecretary;
                return model;
          
        }
        public string GetNameById(string id)
        {
            return m_Rep.GetById(id).TrueName;
        }
        //模糊搜索name
        public List<SysUser> GetListBySelName(string name)
        {
            return m_Rep.GetList(a => a.TrueName.Contains(name)).ToList();
        }
        public List<SysUser> GetListByDepId(string id)
        {
            return m_Rep.GetList(a => a.DepId==id).ToList();
        }

        /// <summary>
        /// 获取二级架构下的用户
        /// </summary>
        /// <returns></returns>
        public List<SysOnlineUserModel> GetAllUsers()
        {
            IQueryable<P_Sys_GetAllUsers_Result> queryData = m_Rep.GetAllUsers();
            List<SysOnlineUserModel> modelList = (from r in queryData
                                                  select new SysOnlineUserModel
                                            {
                                                UserId = r.UserId,
                                                TrueName = r.TrueName,
                                                Email = r.EmailAddress,
                                                PhoneNumber = r.PhoneNumber,
                                                Photo = r.Photo,
                                                PosName = r.PosName,
                                                Sort = r.Sort,
                                                StructId = r.StructId,
                                                StructName = r.StructName,
                                                ContextId = "",
                                                Status = 0,//0离线状态1在线2忙碌3离开
                                            }).ToList();
            return modelList;
        }
    }
}
