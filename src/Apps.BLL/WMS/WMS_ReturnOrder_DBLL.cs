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

namespace Apps.BLL.WMS
{
    public partial class WMS_ReturnOrder_DBLL
    {

        public override List<WMS_ReturnOrder_DModel> GetListByParentId(ref GridPager pager, string queryStr, object parentId)
        {
            IQueryable<WMS_ReturnOrder_D> queryData = null;
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
                                    a.ReturnOrderDNum.Contains(queryStr)
                                   || a.Remark.Contains(queryStr)
                                   || a.PrintStaus.Contains(queryStr)
                                   || a.PrintMan.Contains(queryStr)
                                   || a.ConfirmStatus.Contains(queryStr)
                                   || a.ConfirmMan.Contains(queryStr)
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
        public override List<WMS_ReturnOrder_DModel> CreateModelList(ref IQueryable<WMS_ReturnOrder_D> queryData)
        {

            List<WMS_ReturnOrder_DModel> modelList = (from r in queryData
                                                      select new WMS_ReturnOrder_DModel
                                                      {
                                                          Attr1 = r.Attr1,
                                                          Attr2 = r.Attr2,
                                                          Attr3 = r.Attr3,
                                                          Attr4 = r.Attr4,
                                                          Attr5 = r.Attr5,
                                                          BatchId = r.BatchId,
                                                          ConfirmDate = r.ConfirmDate,
                                                          ConfirmMan = r.ConfirmMan,
                                                          ConfirmStatus = r.ConfirmStatus,
                                                          CreatePerson = r.CreatePerson,
                                                          CreateTime = r.CreateTime,
                                                          HeadId = r.HeadId,
                                                          Id = r.Id,
                                                          ModifyPerson = r.ModifyPerson,
                                                          ModifyTime = r.ModifyTime,
                                                          PrintDate = r.PrintDate,
                                                          PrintMan = r.PrintMan,
                                                          PrintStaus = r.PrintStaus,
                                                          Remark = r.Remark,
                                                          ReturnOrderDNum = r.ReturnOrderDNum,
                                                          ReturnQty = r.ReturnQty,

                                                          Lot = r.WMS_ReturnOrder.Lot,
                                                          ArrivalBillNum = r.WMS_ReturnOrder.WMS_AI.ArrivalBillNum,
                                                          InspectBillNum = r.WMS_ReturnOrder.WMS_AI.InspectBillNum,
                                                          PartCode = r.WMS_ReturnOrder.WMS_Part.PartCode,
                                                          PartName = r.WMS_ReturnOrder.WMS_Part.PartName,
                                                          SupplierCode = r.WMS_ReturnOrder.WMS_Supplier.SupplierCode,
                                                          SupplierShortName = r.WMS_ReturnOrder.WMS_Supplier.SupplierShortName,
                                                          SupplierName = r.WMS_ReturnOrder.WMS_Supplier.SupplierName,
                                                          InvCode = r.WMS_ReturnOrder.WMS_InvInfo.InvCode,
                                                          InvName = r.WMS_ReturnOrder.WMS_InvInfo.InvName,
                                                          ArrivalQty = r.WMS_ReturnOrder.WMS_AI.ArrivalQty,
                                                          QualifyNum = r.WMS_ReturnOrder.WMS_AI.QualifyQty,
                                                          NoQualifyNum = r.WMS_ReturnOrder.WMS_AI.NoQualifyQty,
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
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ReturnOrderDNum, "退货单号");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.HeadId, "头表ID");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ReturnQty, "退货数量");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.Remark, "备注");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.PrintStaus, "打印状态");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.PrintDate, "打印日期");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.PrintMan, "打印人");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ConfirmStatus, "确认状态");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ConfirmMan, "确认人");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ConfirmDate, "确认时间");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.Attr1, "");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.Attr2, "");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.Attr3, "");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.Attr4, "");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.Attr5, "");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.CreatePerson, "创建人");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.CreateTime, "创建时间");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ModifyPerson, "修改人");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.ModifyTime, "修改时间");
                    excelFile.AddMapping<WMS_ReturnOrder_DModel>(x => x.BatchId, "");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_ReturnOrder_DModel>(0);

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
                            var model = new WMS_ReturnOrder_DModel();
                            model.Id = row.Id;
                            model.ReturnOrderDNum = row.ReturnOrderDNum;
                            model.HeadId = row.HeadId;
                            model.ReturnQty = row.ReturnQty;
                            model.Remark = row.Remark;
                            model.PrintStaus = row.PrintStaus;
                            model.PrintDate = row.PrintDate;
                            model.PrintMan = row.PrintMan;
                            model.ConfirmStatus = row.ConfirmStatus;
                            model.ConfirmMan = row.ConfirmMan;
                            model.ConfirmDate = row.ConfirmDate;
                            model.Attr1 = row.Attr1;
                            model.Attr2 = row.Attr2;
                            model.Attr3 = row.Attr3;
                            model.Attr4 = row.Attr4;
                            model.Attr5 = row.Attr5;
                            model.CreatePerson = row.CreatePerson;
                            model.CreateTime = row.CreateTime;
                            model.ModifyPerson = row.ModifyPerson;
                            model.ModifyTime = row.ModifyTime;
                            model.BatchId = row.BatchId;

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
                            WMS_ReturnOrder_D entity = new WMS_ReturnOrder_D();
                            entity.Id = model.Id;
                            entity.ReturnOrderDNum = model.ReturnOrderDNum;
                            entity.HeadId = model.HeadId;
                            entity.ReturnQty = model.ReturnQty;
                            entity.Remark = model.Remark;
                            entity.PrintStaus = model.PrintStaus;
                            entity.PrintDate = model.PrintDate;
                            entity.PrintMan = model.PrintMan;
                            entity.ConfirmStatus = model.ConfirmStatus;
                            entity.ConfirmMan = model.ConfirmMan;
                            entity.ConfirmDate = model.ConfirmDate;
                            entity.Attr1 = model.Attr1;
                            entity.Attr2 = model.Attr2;
                            entity.Attr3 = model.Attr3;
                            entity.Attr4 = model.Attr4;
                            entity.Attr5 = model.Attr5;
                            entity.CreatePerson = model.CreatePerson;
                            entity.CreateTime = model.CreateTime;
                            entity.ModifyPerson = model.ModifyPerson;
                            entity.ModifyTime = model.ModifyTime;
                            entity.BatchId = model.BatchId;
                            entity.CreatePerson = oper;
                            entity.CreateTime = DateTime.Now;
                            entity.ModifyPerson = oper;
                            entity.ModifyTime = DateTime.Now;

                            db.WMS_ReturnOrder_D.Add(entity);
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

        public void AdditionalCheckExcelData(ref WMS_ReturnOrder_DModel model)
        {
        }

        public List<WMS_ReturnOrder_DModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_ReturnOrder_D> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public string PrintReturnOrder(ref ValidationErrors errors, string opt, string jsonReturnOrderNum)
        {
            try
            {
                var rtn = m_Rep.PrintReturnOrder(opt, jsonReturnOrderNum);
                if (!String.IsNullOrEmpty(rtn))
                {
                    return rtn;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return null;
            }
        }

        public bool ConfirmReturnOrder(ref ValidationErrors errors, string opt, string returnOrderNum)
        {
            try
            {
                m_Rep.ConfirmReturnOrder(opt, returnOrderNum);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }
    }
}

