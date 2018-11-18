using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.WMS;

namespace Apps.BLL.WMS
{
    public  partial class WMS_PartBLL
    {

        public override List<WMS_PartModel> CreateModelList(ref IQueryable<WMS_Part> queryData)
        {

            List<WMS_PartModel> modelList = (from r in queryData
                                              select new WMS_PartModel
                                              {
                                                  created_by = r.created_by,
                                                  creation_date = r.creation_date,
                                                  customer_code = r.customer_code,
                                                  logistics_code = r.logistics_code,
                                                  other_code = r.other_code,
                                                  part_code = r.part_code,
                                                  id = r.id,
                                                  part_name = r.part_name,
                                                  part_type = r.part_type,
                                                  pcs = r.pcs,
                                                  status = r.status,
                                                  storeman = r.storeman,
                                                  update_date = r.update_date,
                                                  updated_by = r.updated_by,
                                              }).ToList();
            return modelList;
        }
    }
 }

