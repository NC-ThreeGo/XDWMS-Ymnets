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
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Dynamic;

namespace Apps.BLL.WMS
{
    public  partial class WMS_ReportBLL
    {
        /// <summary>
        /// 获取当前报表的数据源
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSource(WMS_ReportModel report, List<WMS_ReportParamModel> listParam)
        {
			return m_Rep.GetDataSource(report, listParam);
		}

        public List<WMS_Feed_ListModel> GetFeedList(ref GridPager pager)
        {
            using (DBContainer db = new DBContainer())
            {
                DbRawSqlQuery<WMS_Feed_ListModel> query = db.Database.SqlQuery<WMS_Feed_ListModel>(@"SELECT  * from V_WMS_FeedList");
                
                //启用通用列头过滤
                pager.totalRows = query.Count();

                try
                {
                    //排序
                    IQueryable<WMS_Feed_ListModel> queryData = LinqHelper.SortingAndPaging(query.AsQueryable(), pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                    //return query.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        //库存现有量信息
        public List<WMS_InvModel> InvAmount(ref GridPager pager,string partcode,string partname)
        {
            using (DBContainer db = new DBContainer())
            {
                DbRawSqlQuery<WMS_InvModel> query = db.Database.SqlQuery<WMS_InvModel>("SELECT  * from V_WMS_Inv where PartCode like '%" + partcode + "%' and PartName like '%" + partname + "%'");

                //启用通用列头过滤
                pager.totalRows = query.Count();

                try
                {
                    //排序
                    IQueryable<WMS_InvModel> queryData = LinqHelper.SortingAndPaging(query.AsQueryable(), pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                    //return query.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        //供应商交付报表
        public List<WMS_AIModel> SupplierDelivery(ref GridPager pager, string po, string suppliername, string partcode, string partname, DateTime beginDate, DateTime endDate, string deliveryType)
        {
            using (DBContainer db = new DBContainer())
            {
                string s="";
                //显示到货日期不为空的
                if (deliveryType == "已到货")
                {
                    s = "SELECT* from V_WMS_Supplierdelivery where PO like '%" + po
                  + "%' and SupplierName like '%" + suppliername + "%' and PartCode like '%" + partcode
                  + "%' and PartName like '%" + partname + "%' and (ArrivalDate>=CONVERT(varchar(100), '" + beginDate
                  + "', 120) and ArrivalDate<=CONVERT(varchar(100), '" + endDate.AddDays(1) + "', 120))";


                }
                //显示到货日期为空的
                if (deliveryType == "全部")
                {
                    s = "SELECT* from V_WMS_Supplierdelivery where PO like '%" + po
                  + "%' and SupplierName like '%" + suppliername + "%' and PartCode like '%" + partcode
                  + "%' and PartName like '%" + partname + "%' and ((ArrivalDate>=CONVERT(varchar(100), '" + beginDate
                  + "', 120) and ArrivalDate<=CONVERT(varchar(100), '" + endDate.AddDays(1) + "', 120)) or ArrivalDate is null )";
                }
                DbRawSqlQuery<WMS_AIModel> query = db.Database.SqlQuery<WMS_AIModel>(s);

                //启用通用列头过滤
                pager.totalRows = query.Count();

                try
                {
                    //排序
                    IQueryable<WMS_AIModel> queryData = LinqHelper.SortingAndPaging(query.AsQueryable(), pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                    //return query.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        //退货率报表
        public List<WMS_Product_EntryModel> ReturnRate(ref GridPager pager, string partcode, string partname, DateTime beginDate, DateTime endDate,string returnRateType)
        {
            //自制件退货
            string ProductReturnRate = "select c.PartCode,c.PartName,ReturnQty/ProductQty ReturnRate from "
            + " (select partid, SUM(ReturnQty) ReturnQty from WMS_ReturnOrder a where a.AIID is null and CreateTime>=CONVERT(varchar(100), '" + beginDate + "', 120) and CreateTime<=CONVERT(varchar(100), '" + endDate.AddDays(1) + "', 120) group by partid) a,"
            + " (select partid, SUM(ProductQty) ProductQty from WMS_Product_Entry a where  CreateTime>=CONVERT(varchar(100), '" + beginDate + "', 120) and CreateTime<=CONVERT(varchar(100), '" + endDate.AddDays(1) + "', 120) group by partid ) b,WMS_Part c"
            + "  where a.partid = b.partid and a.PartID = c.Id and c.PartCode like '%" + partcode + "%' and c.PartName like '%" + partname + "%'";

            //外购件退货
            string POReturnRate = "select c.PartCode,c.PartName,ReturnQty/ArrivalQty ReturnRate from "
            + " (select partid, SUM(ReturnQty) ReturnQty from WMS_ReturnOrder a where a.AIID is null and CreateTime>=CONVERT(varchar(100), '" + beginDate + "', 120) and CreateTime<=CONVERT(varchar(100), '" + endDate.AddDays(1) + "', 120) group by partid) a,"
            + " (select partid, SUM(ArrivalQty) ArrivalQty from WMS_AI a where  ArrivalDate>=CONVERT(varchar(100), '" + beginDate + "', 120) and ArrivalDate<=CONVERT(varchar(100), '" + endDate.AddDays(1) + "', 120) group by partid ) b,WMS_Part c"
            + "  where a.partid = b.partid and a.PartID = c.Id and c.PartCode like '%" + partcode + "%' and c.PartName like '%" + partname + "%'";
            string ReturnRate;

            if (returnRateType == "自制件")
            { ReturnRate = ProductReturnRate; }
            else { ReturnRate = POReturnRate; }

            using (DBContainer db = new DBContainer())
            {
                DbRawSqlQuery<WMS_Product_EntryModel> query = db.Database.SqlQuery<WMS_Product_EntryModel>(ReturnRate);

                //启用通用列头过滤
                pager.totalRows = query.Count();

                try
                {
                    //排序
                    IQueryable<WMS_Product_EntryModel> queryData = LinqHelper.SortingAndPaging(query.AsQueryable(), pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                    //return query.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}

