using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.WMS;

namespace Apps.BLL.WMS
{
    public  partial class WMS_HeaderBLL
    {

        public override List<WMS_HeaderModel> CreateModelList(ref IQueryable<WMS_Header> queryData)
        {

            List<WMS_HeaderModel> modelList = (from r in queryData
                                              select new WMS_HeaderModel
                                              {
                                                  Code = r.Code,
                                                  Id = r.Id,
                                                  Name = r.Name,
                                              }).ToList();
            return modelList;
        }
    }
 }

