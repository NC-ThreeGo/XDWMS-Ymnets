
using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using Apps.Models.Spl;

namespace Apps.BLL.Spl
{
    public  partial class Spl_WareCategoryBLL
    {

        public List<Spl_WareCategoryModel> GetList(string parentId)
        {
            IQueryable<Spl_WareCategory> queryData = null;
            queryData = m_Rep.GetList(a => a.ParentId == parentId).OrderBy(a => a.CreateTime);
            return CreateModelList(ref queryData);
        }

        public override List<Spl_WareCategoryModel> CreateModelList(ref IQueryable<Spl_WareCategory> queryData)
        {

            List<Spl_WareCategoryModel> modelList = (from r in queryData
                                              select new Spl_WareCategoryModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  Code = r.Code,
                                                  ParentId = r.ParentId,
                                                  Remark = r.Remark,
                                                  CreateTime = r.CreateTime,
                                                  Enable = r.Enable,
                                              }).ToList();
            return modelList;
        }
    }
 }

