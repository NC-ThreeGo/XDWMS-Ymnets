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
using System.Linq.Expressions;

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

        private List<WMS_AIModel> CreateModelListForInspect(ref IQueryable<WMS_AI> queryData)
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
                                               CheckOutDate = DateTime.Now,
                                               CheckOutRemark = r.CheckOutRemark,
                                               CheckOutResult = "合格",
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
                                               NoQualifyQty = 0,
                                               POId = r.POId,
                                               PartId = r.PartId,
                                               Lot = r.Lot,
                                               QualifyQty = r.ArrivalQty,
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
                    excelFile.AddMapping<WMS_AIModel>(x => x.ArrivalBillNum, "到货单据号(必输)");
                    excelFile.AddMapping<WMS_AIModel>(x => x.PO, "采购订单号(必输)");
                    excelFile.AddMapping<WMS_AIModel>(x => x.PartCode, "物料编码(必输)");
                    excelFile.AddMapping<WMS_AIModel>(x => x.BoxQty, "到货箱数");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ArrivalQty, "到货数量(必输)");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ArrivalDate, "到货日期(格式:年-月-日)(必输)");
                    excelFile.AddMapping<WMS_AIModel>(x => x.ReceiveMan, "接收人");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.ReceiveStatus, "到货状态");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InspectBillNum, "送检单号");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InspectMan, "送检人");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InspectDate, "送检日期");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InspectStatus, "送检状态");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.CheckOutDate, "检验日期");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.CheckOutResult, "检验结果");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.QualifyQty, "合格数量");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.NoQualifyQty, "不合格数量");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.CheckOutRemark, "检验说明");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.ReInspectBillNum, "重新送检单");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InStoreBillNum, "入库单号");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InStoreMan, "");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InvId, "入库仓库");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.SubInvId, "子库");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.InStoreStatus, "入库状态");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.Attr1, "");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.Attr2, "");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.Attr3, "");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.Attr4, "");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.Attr5, "");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.CreatePerson, "创建人");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.CreateTime, "创建时间");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.ModifyPerson, "修改人");
                    //excelFile.AddMapping<WMS_AIModel>(x => x.ModifyTime, "修改时间");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_AIModel>(0);

                    //开启事务
                    using (DBContainer db = new DBContainer())
                    {
                        var tran = db.Database.BeginTransaction();  //开启事务
                        int rowIndex = 1;
                        string errorMessage = String.Empty;

                        try
                        {
                            //检查数据正确性
                            foreach (var row in excelContent)
                            {
                                var model = new WMS_AIModel();
                                model.Id = row.Id;
                                model.ArrivalBillNum = row.ArrivalBillNum;
                                //model.POId = row.POId;
                                model.PO = row.PO;
                                //model.PartId = row.PartId;
                                model.PartCode = row.PartCode;
                                model.BoxQty = row.BoxQty;
                                model.ArrivalQty = row.ArrivalQty;
                                model.ArrivalDate = row.ArrivalDate;
                                model.ReceiveMan = row.ReceiveMan;
                                //model.Lot = row.ArrivalDate.ToString("yyyyMM");
                                model.ReceiveStatus = "已到货";
                                //model.InspectBillNum = row.InspectBillNum;
                                //model.InspectMan = row.InspectMan;
                                //model.InspectDate = row.InspectDate;
                                model.InspectStatus = "未送检";
                                //model.CheckOutDate = row.CheckOutDate;
                                //model.CheckOutResult = row.CheckOutResult;
                                //model.QualifyQty = row.QualifyQty;
                                //model.NoQualifyQty = row.NoQualifyQty;
                                //model.CheckOutRemark = row.CheckOutRemark;
                                //model.ReInspectBillNum = row.ReInspectBillNum;
                                //model.InStoreBillNum = row.InStoreBillNum;
                                //model.InStoreMan = row.InStoreMan;
                                //model.InvId = row.InvId;
                                //model.SubInvId = row.SubInvId;
                                model.InStoreStatus = "未入库";
                                //model.Attr1 = row.Attr1;
                                //model.Attr2 = row.Attr2;
                                //model.Attr3 = row.Attr3;
                                //model.Attr4 = row.Attr4;
                                //model.Attr5 = row.Attr5;
                                //model.CreatePerson = row.CreatePerson;
                                //model.CreateTime = row.CreateTime;
                                //model.ModifyPerson = row.ModifyPerson;
                                //model.ModifyTime = row.ModifyTime;

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
                                    AdditionalCheckExcelData(db, ref model);
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
                                //entity.ArrivalBillNum = "DH" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                                entity.POId = model.POId;
                                entity.PartId = model.PartId;
                                entity.BoxQty = model.BoxQty;
                                entity.ArrivalQty = model.ArrivalQty;
                                entity.ArrivalDate = model.ArrivalDate.Value;
                                entity.ReceiveMan = model.ReceiveMan;
                                entity.Lot = model.Lot;
                                entity.ReceiveStatus = model.ReceiveStatus;
                                //entity.InspectBillNum = model.InspectBillNum;
                                //entity.InspectMan = model.InspectMan;
                                //entity.InspectDate = model.InspectDate;
                                entity.InspectStatus = model.InspectStatus;
                                //entity.CheckOutDate = model.CheckOutDate;
                                //entity.CheckOutResult = model.CheckOutResult;
                                //entity.QualifyQty = model.QualifyQty;
                                //entity.NoQualifyQty = model.NoQualifyQty;
                                //entity.CheckOutRemark = model.CheckOutRemark;
                                //entity.ReInspectBillNum = model.ReInspectBillNum;
                                //entity.InStoreBillNum = model.InStoreBillNum;
                                //entity.InStoreMan = model.InStoreMan;
                                //entity.InvId = model.InvId;
                                //entity.SubInvId = model.SubInvId;
                                entity.InStoreStatus = model.InStoreStatus;
                                //entity.Attr1 = model.Attr1;
                                //entity.Attr2 = model.Attr2;
                                //entity.Attr3 = model.Attr3;
                                //entity.Attr4 = model.Attr4;
                                //entity.Attr5 = model.Attr5;
                                //entity.CreatePerson = model.CreatePerson;
                                //entity.CreateTime = model.CreateTime;
                                //entity.ModifyPerson = model.ModifyPerson;
                                //entity.ModifyTime = model.ModifyTime;
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

                                rowIndex += 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            rtn = false;
                            //将当前报错的entity状态改为分离，类似EF的回滚（忽略之前的Add操作）
                            errorMessage = ex.Message;
                            errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                            wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
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

        public void AdditionalCheckExcelData(DBContainer db, ref WMS_AIModel model)
        {
            //获取物料ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var partCode = model.PartCode;
                Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

                //var part = m_PartRep.GetSingleWhere(exp);
                var part = db.WMS_Part.FirstOrDefault(exp);
                if (part == null)
                {
                    throw new Exception("物料编码不存在！");
                }
                else
                {
                    model.PartId = part.Id;
                }
            }
            else
            {
                throw new Exception("物料编码不能为空！");
            }
            //采购订单号
            if (!String.IsNullOrEmpty(model.PO))
            {
                var po = model.PO;
                var partid = model.PartId;
                Expression<Func<WMS_PO, bool>> exp = x => x.PO == po && x.PartId == partid && x.Status == "有效";

                var result = db.WMS_PO.FirstOrDefault(exp);
                if (result != null)
                {
                    model.POId = result.Id;
                }
                else { throw new Exception("无此订单或者订单无此物料！"); }
            }
            else
            {
                throw new Exception("采购订单号不能为空！");
            }
            //校验入库日期
            if (!String.IsNullOrEmpty(model.ArrivalDate.ToString()))
            {
                if (!DateTimeHelper.CheckYearMonth(model.ArrivalDate.Value.ToString("yyyyMM")))
                {
                    throw new Exception("批次号不合符规范！");
                }
                else { model.Lot = model.ArrivalDate.Value.ToString("yyyyMM"); }
            }
            else
            {
                throw new Exception("到货日期不能为空！");
            }
            //校验到货单号
            if (!String.IsNullOrEmpty(model.ArrivalBillNum))
            {
                var partId = model.PartId;
                var arrivalBillNum = model.ArrivalBillNum;
                Expression<Func<WMS_AI, bool>> exp = x => x.PartId == partId && x.ArrivalBillNum == arrivalBillNum;

                //var part = m_PORep.GetSingleWhere(exp);
                var part = db.WMS_AI.FirstOrDefault(exp);
                if (part != null)
                {
                    throw new Exception("到货单号与物料编码重复！");
                }
            }
            else
            {
                throw new Exception("到货单号不能为空！");
            }
            //不允许超量接受

            //投料数量不能为空
            if (model.ArrivalQty > 0)
            {
                throw new Exception("到货数量必须大于0！");
            }
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

        public List<WMS_AIModel> GetInspectBillList(ref GridPager pager, string where)
        {
            try
            {
                IQueryable<WMS_AI> queryData = m_Rep.GetList().Where(where);
                pager.totalRows = queryData.Count();
                //排序
                queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                return CreateModelListForInspect(ref queryData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

