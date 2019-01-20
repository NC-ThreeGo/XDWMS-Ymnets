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
    public partial class WMS_Inv_AdjustBLL
    {

        public override List<WMS_Inv_AdjustModel> CreateModelList(ref IQueryable<WMS_Inv_Adjust> queryData)
        {

            List<WMS_Inv_AdjustModel> modelList = (from r in queryData
                                                   select new WMS_Inv_AdjustModel
                                                   {
                                                       AdjustQty = r.AdjustQty,
                                                       AdjustType = r.AdjustType,
                                                       Attr1 = r.Attr1,
                                                       Attr2 = r.Attr2,
                                                       Attr3 = r.Attr3,
                                                       Attr4 = r.Attr4,
                                                       Attr5 = r.Attr5,
                                                       CreatePerson = r.CreatePerson,
                                                       CreateTime = r.CreateTime,
                                                       Id = r.Id,
                                                       InvAdjustBillNum = r.InvAdjustBillNum,
                                                       InvId = r.InvId,
                                                       ModifyPerson = r.ModifyPerson,
                                                       ModifyTime = r.ModifyTime,
                                                       PartId = r.PartId,
                                                       Remark = r.Remark,
                                                       SubInvId = r.SubInvId,

                                                       PartCode = r.WMS_Part.PartCode,
                                                       InvCode = r.WMS_InvInfo.InvCode,
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
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.InvAdjustBillNum, "调帐单据号");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.PartId, "物料");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.AdjustQty, "调整数量");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.AdjustType, "调整类型");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.InvId, "库存");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.SubInvId, "子库存");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.Remark, "备注");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.Attr1, "");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.Attr2, "");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.Attr3, "");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.Attr4, "");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.Attr5, "");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.CreatePerson, "创建人");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.CreateTime, "创建时间");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.ModifyPerson, "修改人");
                    excelFile.AddMapping<WMS_Inv_AdjustModel>(x => x.ModifyTime, "修改时间");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_Inv_AdjustModel>(0);

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
                            var model = new WMS_Inv_AdjustModel();
                            model.Id = row.Id;
                            model.InvAdjustBillNum = row.InvAdjustBillNum;
                            model.PartId = row.PartId;
                            model.AdjustQty = row.AdjustQty;
                            model.AdjustType = row.AdjustType;
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
                            WMS_Inv_Adjust entity = new WMS_Inv_Adjust();
                            entity.Id = model.Id;
                            entity.InvAdjustBillNum = model.InvAdjustBillNum;
                            entity.PartId = model.PartId;
                            entity.AdjustQty = model.AdjustQty;
                            entity.AdjustType = model.AdjustType;
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

                            db.WMS_Inv_Adjust.Add(entity);
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

        public void AdditionalCheckExcelData(ref WMS_Inv_AdjustModel model)
        {
        }

        public List<WMS_Inv_AdjustModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_Inv_Adjust> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public override bool Create(ref ValidationErrors errors, WMS_Inv_AdjustModel model)
        {
            string invAdjustBillNum = null;
            try
            {
                var rtn = m_Rep.CreateInvAdjust(model.CreatePerson, model.PartId, model.InvId, model.Lot, model.AdjustQty, model.AdjustType,
                        model.Remark, ref invAdjustBillNum);
                if (String.IsNullOrEmpty(rtn))
                {
                    model.InvAdjustBillNum = invAdjustBillNum;
                    return true;
                }
                else
                {
                    errors.Add(rtn);
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

