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
    }
 }

