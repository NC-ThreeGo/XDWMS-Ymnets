using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.Models;
using Apps.Common;
using System.Transactions;
using Apps.Models.Flow;
using Apps.IBLL.Flow;
using Apps.IDAL.Flow;
using Apps.BLL.Core;
using Apps.Locale;


namespace Apps.BLL.Flow
{
    public partial class Flow_FormAttrBLL
    {
       
        [Dependency]
        public IFlow_TypeRepository typeRep { get; set; }
        public override List<Flow_FormAttrModel> GetList(ref GridPager pager, string typeId)
        {

            IQueryable<Flow_FormAttr> queryData = null;
            if (!string.IsNullOrWhiteSpace(typeId) && typeId != "0")
            {

                queryData = m_Rep.GetList(a => a.Name.Contains(typeId) || a.TypeId == typeId);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<Flow_FormAttrModel> CreateModelList(ref IQueryable<Flow_FormAttr> queryData)
        {

            List<Flow_FormAttrModel> modelList = (from r in queryData
                                                  select new Flow_FormAttrModel
                                                  {
                                                      Id = r.Id,
                                                      Title = r.Title,
                                                      Name = r.Name,
                                                      AttrType = r.AttrType,
                                                      CheckJS = r.CheckJS,
                                                      TypeId = r.TypeId,
                                                      TypeName = r.Flow_Type.Name,
                                                      CreateTime = r.CreateTime,
                                                      OptionList = r.OptionList,
                                                      CheckRule = r.CheckRule,

                                                  }).ToList();

            return modelList;
        }

        public override bool Create(ref ValidationErrors errors, Flow_FormAttrModel model)
        { 
            try
            {
                if (m_Rep.GetAttrCountByName(model.Name)>0)
                {
                    errors.Add("英文名称被使用过，请重新输入！");
                    return false;
                }
                Flow_FormAttr entity = m_Rep.GetById(model.Id);
                if (entity != null)
                {
                    errors.Add(Resource.PrimaryRepeat);
                    return false;
                }
                
                entity = new Flow_FormAttr();
                entity.Id = model.Id;
                entity.Title = model.Title;
                entity.Name = model.Name;
                entity.AttrType = model.AttrType;
                entity.CheckRule = model.CheckRule;
                entity.CheckJS = model.CheckJS;
                entity.TypeId = model.TypeId;
                entity.CreateTime = model.CreateTime;
                entity.OptionList = model.OptionList;
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

        public override bool Edit(ref ValidationErrors errors, Flow_FormAttrModel model)
        {
            try
            {
                if (m_Rep.GetAttrCountByName(model.Name) > 1)
                {
                    errors.Add("英文名称被使用过，请重新输入！");
                    return false;
                }
                Flow_FormAttr entity = m_Rep.GetById(model.Id);
                if (entity == null)
                {
                    errors.Add(Resource.Disable);
                    return false;
                }
                
                entity.Id = model.Id;
                entity.Title = model.Title;
                entity.Name = model.Name;
                entity.AttrType = model.AttrType;
                entity.CheckJS = model.CheckJS;
                entity.CheckRule = model.CheckRule;
                entity.TypeId = model.TypeId;
                entity.CreateTime = model.CreateTime;
                entity.OptionList = model.OptionList;
                if (m_Rep.Edit(entity) )
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
    }
}
