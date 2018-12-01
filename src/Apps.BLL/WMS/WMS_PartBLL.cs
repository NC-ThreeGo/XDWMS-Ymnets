using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.WMS;
using System.IO;
using LinqToExcel;
using ClosedXML.Excel;

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

        public bool ImportExcelData(string filePath, ref ValidationErrors errors)
        {
            bool rtn = true;

            return rtn;
        }

        public void AdditionalCheckExcelData(WMS_PartModel model)
        {
        }

        public List<WMS_PartModel> GetList(ref GridPager pager, string partCode, string partName)
        {
            IQueryable<WMS_Part> queryData = null;
            string code = partCode ?? "";
            string name = partName ?? "";
            queryData = m_Rep.GetList().Where(x => x.PartCode.Contains(code) && x.PartName.Contains(name));
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
    }
 }

