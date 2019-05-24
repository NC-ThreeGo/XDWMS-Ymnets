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
using Unity.Attributes;
using Apps.IDAL.WMS;
using Apps.Models.V;

namespace Apps.BLL.WMS
{
    public partial class WMS_Inv_History_DBLL
    {
        [Dependency]
        public IWMS_Inv_History_HRepository m_HeaderRep { get; set; }

        public override List<WMS_Inv_History_DModel> GetListByParentId(ref GridPager pager, string queryStr, object parentId)
        {
            IQueryable<WMS_Inv_History_D> queryData = null;
            int pid = Convert.ToInt32(parentId);
            if (pid != 0)
            {
                queryData = m_Rep.GetList(a => a.HeadId == pid);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                            a => (
                                    a.Remark.Contains(queryStr)
                                   || a.Attr1.Contains(queryStr)
                                   || a.Attr2.Contains(queryStr)
                                   || a.Attr3.Contains(queryStr)
                                   || a.Attr4.Contains(queryStr)
                                   || a.Attr5.Contains(queryStr)
                                   || a.CreatePerson.Contains(queryStr)
                                   || a.ModifyPerson.Contains(queryStr)
                                 )
                            );
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<WMS_Inv_History_DModel> CreateModelList(ref IQueryable<WMS_Inv_History_D> queryData)
        {
            List<WMS_Inv_History_DModel> modelList = (from r in queryData
                                                      select new WMS_Inv_History_DModel
                                                      {
                                                          Attr1 = r.Attr1,
                                                          Attr2 = r.Attr2,
                                                          Attr3 = r.Attr3,
                                                          Attr4 = r.Attr4,
                                                          Attr5 = r.Attr5,
                                                          CreatePerson = r.CreatePerson,
                                                          CreateTime = r.CreateTime,
                                                          HeadId = r.HeadId,
                                                          Id = r.Id,
                                                          InvId = r.InvId,
                                                          ModifyPerson = r.ModifyPerson,
                                                          ModifyTime = r.ModifyTime,
                                                          PartId = r.PartId,
                                                          Remark = r.Remark,
                                                          SnapshootQty = r.SnapshootQty,
                                                          SubInvId = r.SubInvId,

                                                          InvHistoryTitle = r.WMS_Inv_History_H.InvHistoryTitle,
                                                          PartCode = r.WMS_Part.PartCode,
                                                          PartName = r.WMS_Part.PartName,
                                                          InvCode = r.WMS_InvInfo.InvCode,
                                                          InvName = r.WMS_InvInfo.InvName,
                                                          SubInvName = r.WMS_SubInvInfo.SubInvName,
                                                          StoreMan = r.WMS_Part.StoreMan,
                                                      }).ToList();
            return modelList;
        }

        public List<WMS_Inv_History_HModel> CreateModelListParent(ref IQueryable<WMS_Inv_History_H> queryData)
        {
            List<WMS_Inv_History_HModel> modelList = (from r in queryData
                                                      select new WMS_Inv_History_HModel
                                                      {
                                                          Attr1 = r.Attr1,
                                                          Attr2 = r.Attr2,
                                                          Attr3 = r.Attr3,
                                                          Attr4 = r.Attr4,
                                                          Attr5 = r.Attr5,
                                                          CreatePerson = r.CreatePerson,
                                                          CreateTime = r.CreateTime,
                                                          Id = r.Id,
                                                          ModifyPerson = r.ModifyPerson,
                                                          ModifyTime = r.ModifyTime,
                                                          Remark = r.Remark,
                                                          InvHistoryTitle = r.InvHistoryTitle,
                                                          InvHistoryStatus = r.InvHistoryStatus,
                                                      }).ToList();
            return modelList;
        }

        public bool ImportExcelData(string oper, string filePath, ref ValidationErrors errors)
        {
            bool rtn = true;

            var targetFile = new FileInfo(filePath);

            if (!targetFile.Exists)
            {
                errors.Add("导入的数据文件不存在");
                return false;
            }

            var excelFile = new ExcelQueryFactory(filePath);

            using (XLWorkbook wb = new XLWorkbook(filePath))
            {
                //第一个Sheet
                using (IXLWorksheet wws = wb.Worksheets.First())
                {
                    //对应列头
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.HeadId, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.PartId, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.SnapshootQty, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.InvId, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.SubInvId, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.Remark, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.Attr1, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.Attr2, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.Attr3, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.Attr4, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.Attr5, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.CreatePerson, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.CreateTime, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.ModifyPerson, "");
                    excelFile.AddMapping<WMS_Inv_History_DModel>(x => x.ModifyTime, "");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_Inv_History_DModel>(0);

                    //开启事务
                    using (DBContainer db = new DBContainer())
                    {
                        var tran = db.Database.BeginTransaction();  //开启事务
                        int rowIndex = 0;

                        //检查数据正确性
                        foreach (var row in excelContent)
                        {
                            rowIndex += 1;
                            string errorMessage = String.Empty;
                            var model = new WMS_Inv_History_DModel();
                            model.Id = row.Id;
                            model.HeadId = row.HeadId;
                            model.PartId = row.PartId;
                            model.SnapshootQty = row.SnapshootQty;
                            model.InvId = row.InvId;
                            model.SubInvId = row.SubInvId;
                            model.Remark = row.Remark;
                            model.Attr1 = row.Attr1;
                            model.Attr2 = row.Attr2;
                            model.Attr3 = row.Attr3;
                            model.Attr4 = row.Attr4;
                            model.Attr5 = row.Attr5;
                            model.CreatePerson = row.CreatePerson;
                            model.CreateTime = row.CreateTime;
                            model.ModifyPerson = row.ModifyPerson;
                            model.ModifyTime = row.ModifyTime;

                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                rtn = false;
                                errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                                wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
                                continue;
                            }

                            //执行额外的数据校验
                            try
                            {
                                AdditionalCheckExcelData(ref model);
                            }
                            catch (Exception ex)
                            {
                                rtn = false;
                                errorMessage = ex.Message;
                                errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                                wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
                                continue;
                            }

                            //写入数据库
                            WMS_Inv_History_D entity = new WMS_Inv_History_D();
                            entity.Id = model.Id;
                            entity.HeadId = model.HeadId;
                            entity.PartId = model.PartId;
                            entity.SnapshootQty = model.SnapshootQty;
                            entity.InvId = model.InvId;
                            entity.SubInvId = model.SubInvId;
                            entity.Remark = model.Remark;
                            entity.Attr1 = model.Attr1;
                            entity.Attr2 = model.Attr2;
                            entity.Attr3 = model.Attr3;
                            entity.Attr4 = model.Attr4;
                            entity.Attr5 = model.Attr5;
                            entity.CreatePerson = model.CreatePerson;
                            entity.CreateTime = model.CreateTime;
                            entity.ModifyPerson = model.ModifyPerson;
                            entity.ModifyTime = model.ModifyTime;
                            entity.CreatePerson = oper;
                            entity.CreateTime = DateTime.Now;
                            entity.ModifyPerson = oper;
                            entity.ModifyTime = DateTime.Now;

                            db.WMS_Inv_History_D.Add(entity);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                rtn = false;
                                //将当前报错的entity状态改为分离，类似EF的回滚（忽略之前的Add操作）
                                db.Entry(entity).State = System.Data.Entity.EntityState.Detached;
                                errorMessage = ex.InnerException.InnerException.Message;
                                errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                                wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
                            }
                        }

                        if (rtn)
                        {
                            tran.Commit();  //必须调用Commit()，不然数据不会保存
                        }
                        else
                        {
                            tran.Rollback();    //出错就回滚       
                        }
                    }
                }
                wb.Save();
            }

            return rtn;
        }

        public void AdditionalCheckExcelData(ref WMS_Inv_History_DModel model)
        {
        }

        public List<WMS_Inv_History_DModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_Inv_History_D> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public List<WMS_Inv_History_HModel> GetListParent(ref GridPager pager, string where)
        {
            IQueryable<WMS_Inv_History_H> queryData = null;
            queryData = m_HeaderRep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelListParent(ref queryData);
        }

        public bool Create(ref ValidationErrors errors, string oper, string title, string status, string remark)
        {
            try
            {
                var rtn = m_HeaderRep.Create(oper, title, status, remark);
                if (!String.IsNullOrEmpty(rtn))
                {
                    errors.Add(rtn);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }

        public List<WMS_InvHistoryAvg> GetInvHistoryAvg(ref GridPager pager, string where)
        {
            using (DBContainer db = new DBContainer())
            {
                IQueryable<V_WMS_InvHistoryAvg> queryData = db.V_WMS_InvHistoryAvg.Where(where);
                var queryData1 = from d in queryData
                                 select new WMS_InvHistoryAvg()
                                 {
                                     Id = d.Id,
                                     PartId = d.PartId,
                                     InvId = d.InvId,
                                     PartCode = d.PartCode,
                                     PartName = d.PartName,
                                     InvCode = d.InvCode,
                                     InvName = d.InvName,
                                     AvgQty = d.AvgQty,
                                     InvQty = d.InvQty,
                                     BalanceQty = d.InvQty - d.AvgQty,
                                 };
                pager.totalRows = queryData1.Count();
                //排序
                queryData1 = LinqHelper.SortingAndPaging(queryData1, pager.sort, pager.order, pager.page, pager.rows);

                return queryData1.ToList();
            }
        }
    }
}

