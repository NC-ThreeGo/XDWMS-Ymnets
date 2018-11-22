using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.WMS;

namespace Apps.BLL.WMS
{
    public  partial class WMS_LineBLL
    {

    public override List<WMS_LineModel> GetListByParentId(ref GridPager pager, string queryStr, object parentId)
    {
        IQueryable<WMS_Line> queryData = null;
        int pid = Convert.ToInt32(parentId);
        if (pid != 0)
        {
        queryData = m_Rep.GetList(a => a.HeaderId == pid);
        }
        else
        {
        queryData = m_Rep.GetList();
        }
        if (!string.IsNullOrWhiteSpace(queryStr))
        {
            queryData = m_Rep.GetList(
                        a => (
                                a.LineName.Contains(queryStr)
                             )
                        );
        }
        pager.totalRows = queryData.Count();
        queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
        return CreateModelList(ref queryData);
    }
        public override List<WMS_LineModel> CreateModelList(ref IQueryable<WMS_Line> queryData)
        {

            List<WMS_LineModel> modelList = (from r in queryData
                                              select new WMS_LineModel
                                              {
                                                  HeaderId = r.HeaderId,
                                                  Id = r.Id,
                                                  LineName = r.LineName,
                                                  HeaderName = r.WMS_Header.Name,
                                              }).ToList();
            return modelList;
        }
    }
 }

