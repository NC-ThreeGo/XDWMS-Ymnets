using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.IBLL;
using Unity.Attributes;
using Apps.IDAL;
using Apps.Models;
using Apps.BLL.Core;
using Apps.Common;
using System.Transactions;
using Apps.Models.Sys;
using Apps.Locale;
using Apps.IDAL.Sys;

namespace Apps.BLL.Sys
{
    public partial class SysModuleBLL
    {

        [Dependency]
        public ISysModuleOperateRepository m_OperateRep { get; set; }
      

        public List<SysModuleModel> GetList(string parentId)
        {
            IQueryable<SysModule> queryData = null;
            queryData = m_Rep.GetList(a => a.ParentId == parentId).OrderBy(a => a.Sort);
            return CreateModelList(ref queryData);
        }
      
        public List<SysModule> GetModuleBySystem(string parentId)
        {

            return m_Rep.GetModuleBySystem(parentId).ToList();
        }

        public override bool Create(ref ValidationErrors errors, SysModuleModel model)
        {
            try
            {
                SysModule entity = m_Rep.GetById(model.Id);
                if (entity != null)
                {
                    errors.Add(Resource.PrimaryRepeat);
                    return false;
                }
                entity = new SysModule();
                entity.Id = model.Id;
                entity.Name = model.Name;
                entity.EnglishName = model.EnglishName;
                entity.ParentId = model.ParentId;
                entity.Url = model.Url;
                entity.Iconic = model.Iconic;
                entity.Sort = model.Sort;
                entity.Remark = model.Remark;
                entity.Enable = model.Enable;
                entity.CreatePerson = model.CreatePerson;
                entity.CreateTime = model.CreateTime;
                entity.IsLast = model.IsLast;
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
        public bool DeleteAndClear(ref ValidationErrors errors, string id)
        {
            try
            {
                //检查是否有下级
                if (m_Rep.GetChildrenCount(id)> 0)
                {
                    errors.Add("有下属关联，请先删除下属！");
                    return false;
                }
                
                if (m_Rep.Delete(id) > 0)
                {
                    //清理无用的项
                    m_Rep.P_Sys_ClearUnusedRightOperate();
                    return true;
                }
                else
                {
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
     }
}
