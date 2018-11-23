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
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  CustomerCode = r.CustomerCode,
                                                  Id = r.Id,
                                                  LogisticsCode = r.LogisticsCode,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  OtherCode = r.OtherCode,
                                                  PartCode = r.PartCode,
                                                  PartName = r.PartName,
                                                  PartType = r.PartType,
                                                  PCS = r.PCS,
                                                  Status = r.Status,
                                                  StoreMan = r.StoreMan,
                                              }).ToList();
            return modelList;
        }
    }
 }

