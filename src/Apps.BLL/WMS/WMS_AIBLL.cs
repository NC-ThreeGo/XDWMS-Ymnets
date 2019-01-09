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
    public partial class WMS_AIBLL
    {

        public override List<WMS_AIModel> CreateModelList(ref IQueryable<WMS_AI> queryData)
        {

            List<WMS_AIModel> modelList = (from r in queryData
                                           select new WMS_AIModel
                                           {
                                               ArrivalBillNum = r.ArrivalBillNum,
                                               ArrivalDate = r.ArrivalDate,
                                               ArrivalQty = r.ArrivalQty,
                                               Attr1 = r.Attr1,
                                               Attr2 = r.Attr2,
                                               Attr3 = r.Attr3,
                                               Attr4 = r.Attr4,
                                               Attr5 = r.Attr5,
                                               BoxQty = r.BoxQty,
                                               CheckOutDate = r.CheckOutDate,
                                               CheckOutRemark = r.CheckOutRemark,
                                               CheckOutResult = r.CheckOutResult,
                                               CreatePerson = r.CreatePerson,
                                               CreateTime = r.CreateTime,
                                               Id = r.Id,
                                               InspectBillNum = r.InspectBillNum,
                                               InspectDate = r.InspectDate,
                                               InspectMan = r.InspectMan,
                                               InspectStatus = r.InspectStatus,
                                               InStoreBillNum = r.InStoreBillNum,
                                               InStoreMan = r.InStoreMan,
                                               InStoreStatus = r.InStoreStatus,
                                               InvId = r.InvId,
                                               SubInvId = r.SubInvId,
                                               ModifyPerson = r.ModifyPerson,
                                               ModifyTime = r.ModifyTime,
                                               NoQualifyQty = r.NoQualifyQty,
                                               POId = r.POId,
                                               PartId = r.PartId,
                                               Lot = r.Lot,
                                               QualifyQty = r.QualifyQty,
                                               ReceiveMan = r.ReceiveMan,
                                               ReceiveStatus = r.ReceiveStatus,
                                               ReInspectBillNum = r.ReInspectBillNum,

                                               PartCode = r.WMS_PO.WMS_Part.PartCode,
                                               PartName = r.WMS_PO.WMS_Part.PartName,
                                               SupplierShortName = r.WMS_PO.WMS_Supplier.SupplierShortName,
                                               PO = r.WMS_PO.PO,
                                               PlanDate = r.WMS_PO.PlanDate,
                                               InvName = r.WMS_InvInfo.InvName,
                                               SubInvName = r.WMS_SubInvInfo.SubInvName,
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
                    excelFile.AddMapping<WMS_AIModel>(x => x.ArrivalBillNum, "到货单据号");
                    excelFile.AddMapping<WMS_AIModel>(x => x.POId, "采购订单ID");
                    excelFile.AddMapping<WMS_AIModel>(x => x.PartId, "物料ID");
                    excelFile.AddMapping<WMS_AIModel>(x => x.BoxQty, "到货箱数");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ArrivalQty, "到货数量");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ArrivalDate, "到货日期");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ReceiveMan, "接收人");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ReceiveStatus, "到货状态");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InspectBillNum, "送检单号");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InspectMan, "送检人");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InspectDate, "送检日期");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InspectStatus, "送检状体");
                    excelFile.AddMapping<WMS_AIModel>(x => x.CheckOutDate, "检验日期");
                    excelFile.AddMapping<WMS_AIModel>(x => x.CheckOutResult, "检验结果");
                    excelFile.AddMapping<WMS_AIModel>(x => x.QualifyQty, "合格数量");
                    excelFile.AddMapping<WMS_AIModel>(x => x.NoQualifyQty, "不合格数量");
                    excelFile.AddMapping<WMS_AIModel>(x => x.CheckOutRemark, "检验说明");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ReInspectBillNum, "重新送检单");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InStoreBillNum, "入库单号");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InStoreMan, "");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InvId, "入库仓库");
                    excelFile.AddMapping<WMS_AIModel>(x => x.SubInvId, "子库");
                    excelFile.AddMapping<WMS_AIModel>(x => x.InStoreStatus, "入库状态");
                    excelFile.AddMapping<WMS_AIModel>(x => x.Attr1, "");
                    excelFile.AddMapping<WMS_AIModel>(x => x.Attr2, "");
                    excelFile.AddMapping<WMS_AIModel>(x => x.Attr3, "");
                    excelFile.AddMapping<WMS_AIModel>(x => x.Attr4, "");
                    excelFile.AddMapping<WMS_AIModel>(x => x.Attr5, "");
                    excelFile.AddMapping<WMS_AIModel>(x => x.CreatePerson, "创建人");
                    excelFile.AddMapping<WMS_AIModel>(x => x.CreateTime, "创建时间");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ModifyPerson, "修改人");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ModifyTime, "修改时间");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_AIModel>(0);

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
                            var model = new WMS_AIModel();
                            model.Id = row.Id;
                            model.ArrivalBillNum = row.ArrivalBillNum;
                            model.POId = row.POId;
                            model.PartId = row.PartId;
                            model.BoxQty = row.BoxQty;
                            model.ArrivalQty = row.ArrivalQty;
                            model.ArrivalDate = row.ArrivalDate;
                            model.ReceiveMan = row.ReceiveMan;
                            model.ReceiveStatus = row.ReceiveStatus;
                            model.InspectBillNum = row.InspectBillNum;
                            model.InspectMan = row.InspectMan;
                            model.InspectDate = row.InspectDate;
                            model.InspectStatus = row.InspectStatus;
                            model.CheckOutDate = row.CheckOutDate;
                            model.CheckOutResult = row.CheckOutResult;
                            model.QualifyQty = row.QualifyQty;
                            model.NoQualifyQty = row.NoQualifyQty;
                            model.CheckOutRemark = row.CheckOutRemark;
                            model.ReInspectBillNum = row.ReInspectBillNum;
                            model.InStoreBillNum = row.InStoreBillNum;
                            model.InStoreMan = row.InStoreMan;
                            model.InvId = row.InvId;
                            model.SubInvId = row.SubInvId;
                            model.InStoreStatus = row.InStoreStatus;
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
                            WMS_AI entity = new WMS_AI();
                            entity.Id = model.Id;
                            entity.ArrivalBillNum = model.ArrivalBillNum;
                            entity.POId = model.POId;
                            entity.PartId = model.PartId;
                            entity.BoxQty = model.BoxQty;
                            entity.ArrivalQty = model.ArrivalQty;
                            entity.ArrivalDate = model.ArrivalDate;
                            entity.ReceiveMan = model.ReceiveMan;
                            entity.ReceiveStatus = model.ReceiveStatus;
                            entity.InspectBillNum = model.InspectBillNum;
                            entity.InspectMan = model.InspectMan;
                            entity.InspectDate = model.InspectDate;
                            entity.InspectStatus = model.InspectStatus;
                            entity.CheckOutDate = model.CheckOutDate;
                            entity.CheckOutResult = model.CheckOutResult;
                            entity.QualifyQty = model.QualifyQty;
                            entity.NoQualifyQty = model.NoQualifyQty;
                            entity.CheckOutRemark = model.CheckOutRemark;
                            entity.ReInspectBillNum = model.ReInspectBillNum;
                            entity.InStoreBillNum = model.InStoreBillNum;
                            entity.InStoreMan = model.InStoreMan;
                            entity.InvId = model.InvId;
                            entity.SubInvId = model.SubInvId;
                            entity.InStoreStatus = model.InStoreStatus;
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

                            db.WMS_AI.Add(entity);
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

        public void AdditionalCheckExcelData(ref WMS_AIModel model)
        {
        }

        public List<WMS_AIModel> GetListByWhere(ref GridPager pager, string where)
        {
            try
            {
                IQueryable<WMS_AI> queryData = null;
                queryData = m_Rep.GetList().Where(where);
                pager.totalRows = queryData.Count();
                //排序
                queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                return CreateModelList(ref queryData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

