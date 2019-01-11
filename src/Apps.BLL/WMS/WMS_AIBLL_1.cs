using Apps.Common;
using Apps.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.IO;
using LinqToExcel;
using ClosedXML.Excel;
using Apps.Models.WMS;
using System.Dynamic;
using Apps.Locale;
using Apps.BLL.Core;
using System.Linq.Expressions;

namespace Apps.BLL.WMS
{
    public partial class WMS_AIBLL
    {
        public IQueryable<WMS_POForAIModel> GetPOListForAI(ref GridPager pager, string poNo)
        {
            IQueryable<WMS_POForAIModel> queryData = null;
            queryData = m_Rep.GetPOListForAI(poNo);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return queryData;
        }

        public IQueryable<WMS_POForAIModel> GetPOListForAI(ref GridPager pager, string poNo, int partId)
        {
            IQueryable<WMS_POForAIModel> queryData = null;
            queryData = m_Rep.GetPOListForAI(poNo, partId);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return queryData;
        }

        public IQueryable<WMS_POForAIModel> GetArrivalBillListForNum(ref GridPager pager, string arrivalBillNum)
        {
            //IQueryable<WMS_POForAIModel> queryData = null;
            //queryData = m_Rep.GetList().Where(arrivalBillNum);
            //pager.totalRows = queryData.Count();
            ////排序
            //queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            //return queryData;
            return null;
        }

        public string CreateInspectBill(string opt, string arrivalBillNum)
        {
            return m_Rep.CreateInspectBill(opt, arrivalBillNum);
        }

        public virtual bool CancelInspectBill(ref ValidationErrors errors, string opt, int aiId)
        {
            try
            {
                //WMS_AI entity = m_Rep.GetById(aiId);
                //var customer = model.CustomerType;
                Expression<Func<WMS_AI, bool>> exp = x => x.Id == aiId && x.InStoreStatus == "未入库";
                WMS_AI entity = m_Rep.GetSingleWhere(exp);
                if (entity == null)
                {
                    //errors.Add(Resource.Disable);
                    errors.Add(" :单据已入库不能删除");
                    return false;
                }
                //entity.Id = aiId;               
                entity.InspectBillNum = "";
                entity.InspectMan = "";
                entity.InspectDate = null;
                entity.InspectStatus = "未送检";
                entity.ModifyPerson = opt;
                entity.ModifyTime = DateTime.Now;

                if (m_Rep.Edit(entity))
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

        public bool ProcessInspectBill(ref ValidationErrors errors, string opt, string jsonInspectBill)
        {
            string result = String.Empty;
            try
            {
                result = m_Rep.ProcessInspectBill(opt, jsonInspectBill);
                if (String.IsNullOrEmpty(result))
                {
                    return true;
                }
                else
                {
                    errors.Add(result);
                    return false;
                }
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }
        }
    }
}

