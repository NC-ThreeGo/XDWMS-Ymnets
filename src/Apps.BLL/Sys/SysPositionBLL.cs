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
using Apps.IDAL.Sys;

namespace Apps.BLL.Sys
{
    public partial class SysPositionBLL
    {
     
        [Dependency]
        public ISysStructRepository depRep { get; set; }
    
        public List<SysPositionModel> GetPosListByDepId(ref GridPager pager,string depId)
        {

            IQueryable<SysPosition> queryData = null;
            if (!string.IsNullOrWhiteSpace(depId))
            {
                if(depId=="root")
                    queryData = m_Rep.GetList();
                else
                queryData = m_Rep.GetList(a => a.DepId == depId);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            pager.totalRows = queryData.Count();
            //queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public override List<SysPositionModel> GetList(ref GridPager pager, string queryStr)
        {

            IQueryable<SysPosition> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                                a => a.Id.Contains(queryStr)
                                || a.Name.Contains(queryStr)
                                || a.Remark.Contains(queryStr)
                                || a.DepId.Contains(queryStr)
                                );
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<SysPositionModel> CreateModelList(ref IQueryable<SysPosition> queryData)
        {

            List<SysPositionModel> modelList = (from r in queryData
                                                select new SysPositionModel
                                                {
                                                    Id = r.Id,
                                                    Name = r.Name,
                                                    Remark = r.Remark,
                                                    Sort = r.Sort,
                                                    CreateTime = r.CreateTime,
                                                    Enable = r.Enable,
                                                    MemberCount = r.MemberCount,
                                                    DepId = r.DepId,
                                                    DepName = r.SysStruct.Name
                                                }).ToList();

            return modelList;
        }

        public override SysPositionModel GetById(object id)
        {
            if (IsExists(id))
            {
                SysPosition entity = m_Rep.GetById(id);
                SysPositionModel model = new SysPositionModel();
                model.Id = entity.Id;
                model.Name = entity.Name;
                model.Remark = entity.Remark;
                model.Sort = entity.Sort;
                model.CreateTime = entity.CreateTime;
                model.Enable = entity.Enable;
                model.MemberCount = entity.MemberCount;
                model.DepId = entity.DepId;
                model.DepName = entity.SysStruct.Name;
                return model;
            }
            else
            {
                return null;
            }
        }
    }
}
