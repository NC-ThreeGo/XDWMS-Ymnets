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
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using Apps.IDAL.WMS;
using Unity.Attributes;

namespace Apps.BLL.WMS
{
    public partial class WMS_Product_EntryBLL
    {
        [Dependency]
        public IWMS_PartRepository m_PartRep { get; set; }

        [Dependency]
        public IWMS_InvInfoRepository m_InvInfoRep { get; set; }

        [Dependency]
        public IWMS_Product_EntryRepository m_ProductRep { get; set; }

        public override List<WMS_Product_EntryModel> CreateModelList(ref IQueryable<WMS_Product_Entry> queryData)
        {

            List<WMS_Product_EntryModel> modelList = (from r in queryData
                                                      select new WMS_Product_EntryModel
                                                      {
                                                          Attr1 = r.Attr1,
                                                          Attr2 = r.Attr2,
                                                          Attr3 = r.Attr3,
                                                          Attr4 = r.Attr4,
                                                          Attr5 = r.Attr5,
                                                          CreatePerson = r.CreatePerson,
                                                          CreateTime = r.CreateTime,
                                                          Department = r.Department,
                                                          EntryBillNum = r.EntryBillNum,
                                                          Id = r.Id,
                                                          InvId = r.InvId,
                                                          ModifyPerson = r.ModifyPerson,
                                                          ModifyTime = r.ModifyTime,
                                                          Partid = r.Partid,
                                                          ProductBillNum = r.ProductBillNum,
                                                          ProductQty = r.ProductQty,
                                                          Remark = r.Remark,
                                                          SubInvId = r.SubInvId,
                                                          Lot = r.Lot,

                                                          PartCode = r.WMS_Part.PartCode,
                                                          PartName = r.WMS_Part.PartName,
                                                          InvName = r.WMS_InvInfo.InvName,
                                                          InvCode = r.WMS_InvInfo.InvCode
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
                    excelFile.AddMapping<WMS_Product_EntryModel>(x => x.ProductBillNum, "入库单号（业务）(必输)");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.EntryBillNum, "入库单号（系统）");
                    excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Department, "本货部门");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Partid, "物料");
                    excelFile.AddMapping<WMS_Product_EntryModel>(x => x.PartCode, "物料编码(必输)");
                    excelFile.AddMapping<WMS_Product_EntryModel>(x => x.ProductQty, "数量(必输)");
                    excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Lot, "批次(格式：YYYY-MM-DD)");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.InvId, "库存");
                    excelFile.AddMapping<WMS_Product_EntryModel>(x => x.InvName, "库房");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.SubInvId, "子库存");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Remark, "备注");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Attr1, "");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Attr2, "");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Attr3, "");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Attr4, "");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.Attr5, "");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.CreatePerson, "创建人");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.CreateTime, "创建时间");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.ModifyPerson, "修改人");
                    //excelFile.AddMapping<WMS_Product_EntryModel>(x => x.ModifyTime, "修改时间");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_Product_EntryModel>(0);

                    //开启事务
                    using (DBContainer db = new DBContainer())
                    {
                        var tran = db.Database.BeginTransaction();  //开启事务
                        int rowIndex = 0;
                        string productBillNum = "Z" + DateTime.Now.ToString("yyyyMMddHHmmssff");

                        //检查数据正确性
                        foreach (var row in excelContent)
                        {
                            rowIndex += 1;
                            string errorMessage = String.Empty;
                            var model = new WMS_Product_EntryModel();
                            model.Id = row.Id;
                            model.ProductBillNum = productBillNum;
                            //productBillNum = row.ProductBillNum;
                            model.EntryBillNum = row.EntryBillNum;
                            model.Department = row.Department;
                            //model.Partid = row.Partid;
                            model.PartCode = row.PartCode;
                            model.ProductQty = row.ProductQty;
                            model.Lot = row.Lot;
                            //model.InvId = row.InvId;
                            model.InvName = row.InvName;
                            //model.SubInvId = row.SubInvId;
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
                            WMS_Product_Entry entity = new WMS_Product_Entry();
                            entity.Id = model.Id;
                            entity.ProductBillNum = model.ProductBillNum;
                            //entity.ProductBillNum = "ZZJ" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                            entity.EntryBillNum = model.EntryBillNum;
                            //entity.EntryBillNum = "ZZJ" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                            entity.Department = "总装车间";
                            entity.Partid = model.Partid;
                            entity.ProductQty = model.ProductQty;
                            entity.InvId = model.InvId;
                            entity.Lot = model.Lot;
                            //entity.SubInvId = model.SubInvId;
                            entity.Remark = model.Remark;
                            //entity.Attr1 = model.Attr1;
                            //entity.Attr2 = model.Attr2;
                            //entity.Attr3 = model.Attr3;
                            //entity.Attr4 = model.Attr4;
                            //entity.Attr5 = model.Attr5;
                            entity.CreatePerson = oper;
                            entity.CreateTime = DateTime.Now;
                            entity.ModifyPerson = oper;
                            entity.ModifyTime = DateTime.Now;

                            db.WMS_Product_Entry.Add(entity);
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
                            //全部保存成功后，调用存储过程“P_WMS_ProcessProductEntry”入库。
                            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
                            db.P_WMS_ProcessProductEntry(oper, productBillNum, returnValue);
                            if (returnValue.Value == DBNull.Value)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                                errors.Add((string)returnValue.Value);
                            }

                            //tran.Commit();  //必须调用Commit()，不然数据不会保存
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

        private void AdditionalCheckExcelData(DBContainer db, ref WMS_Product_EntryModel model)
        {
            //获取物料ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var partCode = model.PartCode;
                Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

                var part = db.WMS_Part.FirstOrDefault(exp);
                if (part == null)
                {
                    throw new Exception("物料编码不存在！");
                }
                else
                {
                    model.Partid = part.Id;
                }
            }
            else
            {
                throw new Exception("物料编码不能为空！");
            }

            //获取仓库ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var invName = "主仓库";
                Expression<Func<WMS_InvInfo, bool>> exp = x => x.InvName == invName;

                var inv = db.WMS_InvInfo.FirstOrDefault(exp);
                if (inv == null)
                {
                    throw new Exception("库房不存在！");
                }
                else
                {
                    model.InvId = inv.Id;
                }
            }
            //else
            //{
            //    throw new Exception("库房不能为空！");
            //}

            //校验入库单号与物料
            if (!String.IsNullOrEmpty(model.ProductBillNum))
            {
                var partcode = model.PartCode;
                var productBillNum = model.ProductBillNum;
                Expression<Func<WMS_Product_Entry, bool>> exp = x => x.WMS_Part.PartCode == partcode && x.ProductBillNum == productBillNum;

                var part = db.WMS_Product_Entry.FirstOrDefault(exp);
                if (part != null)
                {
                    throw new Exception("入库单号与物料编码重复！");
                }
            }
            //else
            //{
            //    throw new Exception("入库单号不能为空！");
            //}

            //校验批次号,没有批次号自动赋值为当前月
            if (!String.IsNullOrEmpty(model.Lot))
            {
                if (!DateTimeHelper.CheckYearMonth(model.Lot))
                {
                    throw new Exception("批次号不合符规范！");
                }
            }
            else {
                //model.Lot = DateTime.Now.ToString("yyyyMM");
                model.Lot = DateTimeHelper.GetLot(DateTime.Now);
            }

            if (model.ProductQty == 0)
            {
                throw new Exception("数量不能为空！");
            }

        }

        public List<WMS_Product_EntryModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_Product_Entry> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public decimal GetSumByWhere(string where, string sumField)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(WMS_Product_Entry), "p");
            var expression = Expression.Lambda<Func<WMS_Product_Entry, decimal>>(Expression.Property(parameter, sumField), parameter);
            try
            {
                decimal total = m_Rep.GetList().Where(where).Sum(expression);
                return total;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public override bool Create(ref ValidationErrors errors, WMS_Product_EntryModel model)
        {
            try
            {
                //开启事务
                using (DBContainer db = new DBContainer())
                {
                    var tran = db.Database.BeginTransaction();  //开启事务

                    WMS_Product_Entry entity = new WMS_Product_Entry();
                    entity.ProductBillNum = model.ProductBillNum;
                    entity.Department = model.Department;
                    entity.Partid = model.Partid;
                    entity.ProductQty = model.ProductQty;
                    entity.InvId = model.InvId;
                    entity.SubInvId = model.SubInvId;
                    entity.Remark = model.Remark;
                    entity.CreatePerson = model.CreatePerson;
                    entity.CreateTime = model.CreateTime;
                    entity.Lot = model.Lot;

                    db.WMS_Product_Entry.Add(entity);

                    if (db.SaveChanges() == 1)
                    {
                        ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
                        db.P_WMS_ProcessProductEntry(model.CreatePerson, model.ProductBillNum, returnValue);
                        if (returnValue.Value == DBNull.Value)
                        {
                            tran.Commit();
                            return true;
                        }
                        else
                        {
                            tran.Rollback();
                            errors.Add((string)returnValue.Value);
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }
    }
}

