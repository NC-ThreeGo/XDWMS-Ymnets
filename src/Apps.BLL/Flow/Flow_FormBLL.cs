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
    public partial class Flow_FormBLL
    {

        [Dependency]
        public IFlow_TypeRepository typeRep { get; set; }

        public List<Flow_FormModel> GetListByTypeId(string typeId)
        {
            IQueryable<Flow_Form> queryData = m_Rep.GetList(a => a.TypeId == typeId);
            return CreateModelList(ref queryData);

        }
        public override List<Flow_FormModel> CreateModelList(ref IQueryable<Flow_Form> queryData)
        {

            List<Flow_FormModel> modelList = (from r in queryData
                                              select new Flow_FormModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  Remark = r.Remark,
                                                  UsingDep = r.UsingDep,
                                                  TypeId = r.TypeId,
                                                  TypeName = r.Flow_Type.Name,
                                                  State = r.State,
                                                  CreateTime = r.CreateTime,
                                                  AttrA = r.AttrA,
                                                  IsExternal = r.IsExternal,
                                                  ExternalURL = r.ExternalURL
                                              }).ToList();

            return modelList;
        }
    }
}
