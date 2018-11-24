using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.WMS;

namespace Apps.BLL.WMS
{
    public  partial class WMS_SupplierBLL
    {

        public override List<WMS_SupplierModel> CreateModelList(ref IQueryable<WMS_Supplier> queryData)
        {

            List<WMS_SupplierModel> modelList = (from r in queryData
                                              select new WMS_SupplierModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Id = r.Id,
                                                  LinkMan = r.LinkMan,
                                                  LinkManAddress = r.LinkManAddress,
                                                  LinkManTel = r.LinkManTel,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  Remark = r.Remark,
                                                  Status = r.Status,
                                                  SupplierCode = r.SupplierCode,
                                                  SupplierName = r.SupplierName,
                                                  SupplierShortName = r.SupplierShortName,
                                                  SupplierType = r.SupplierType,
                                              }).ToList();
            return modelList;
        }
    }
 }

