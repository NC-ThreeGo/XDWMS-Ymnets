using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.WMS;
using System.IO;
using LinqToExcel;
using System.Text;
using ClosedXML.Excel;
using Apps.Common.ExcelHelper;

namespace Apps.BLL.WMS
{
    public partial class WMS_SupplierBLL
    {

        public override List<WMS_SupplierModel> CreateModelList(ref IQueryable<WMS_Supplier> queryData)
        {

            List<WMS_SupplierModel> modelList = (from r in queryData
                                                 select new WMS_SupplierModel
                                                 {
                                                     CreatePerson = r.CreatePerson,
                                                     CreateTime = r.CreateTime,
                                                     Id = r.Id,
                                                     LinkMan = r.LinkMan,
                                                     LinkManAddress = r.LinkManAddress,
                                                     LinkManTel = r.LinkManTel,
                                                     ModifyPerson = r.ModifyPerson,
                                                     ModifyTime = r.ModifyTime,
                                                     Remark = r.Remark,
                                                     Status = r.Status,
                                                     SupplierCode = r.SupplierCode,
                                                     SupplierName = r.SupplierName,
                                                     SupplierShortName = r.SupplierShortName,
                                                     SupplierType = r.SupplierType,
                                                 }).ToList();
            return modelList;
        }

        public bool ImportExcelData(string filePath, ref ValidationErrors errors)
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
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierCode, "供应商编码");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierShortName, "供应商简称");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierName, "供应商名称");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierType, "供应商类型");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.LinkMan, "联系人");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.LinkManTel, "联系人电话");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.LinkManAddress, "联系人地址");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.Status, "状态");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.Remark, "说明");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.CreatePerson, "创建人");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.CreateTime, "创建时间");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.ModifyPerson, "修改人");
                    excelFile.AddMapping<WMS_SupplierModel>(x => x.ModifyTime, "修改时间");

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_SupplierModel>(0);

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
                            var model = new WMS_SupplierModel();
                            model.Id = row.Id;
                            model.SupplierCode = row.SupplierCode;
                            model.SupplierShortName = row.SupplierShortName;
                            model.SupplierName = row.SupplierName;
                            model.SupplierType = row.SupplierType;
                            model.LinkMan = row.LinkMan;
                            model.LinkManTel = row.LinkManTel;
                            model.LinkManAddress = row.LinkManAddress;
                            model.Status = row.Status;
                            model.Remark = row.Remark;
                            model.CreatePerson = row.CreatePerson;
                            model.CreateTime = row.CreateTime;
                            model.ModifyPerson = row.ModifyPerson;
                            model.ModifyTime = row.ModifyTime;

                            //=============================================================================
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                rtn = false;

                                errors.Add(string.Format(
                                    "第 {0} 列发现错误：{1}{2}",
                                    rowIndex,
                                    errorMessage,
                                    "<br/>"));

                                wws.Cell(rowIndex + 1, 15).Value = errorMessage;
                            }
                            else
                            {
                                //写入数据库
                                WMS_Supplier entity = new WMS_Supplier();
                                entity.Id = 0;
                                entity.SupplierCode = model.SupplierCode;
                                entity.SupplierShortName = model.SupplierShortName;
                                entity.SupplierName = model.SupplierName;
                                entity.SupplierType = model.SupplierType;
                                entity.LinkMan = model.LinkMan;
                                entity.LinkManTel = model.LinkManTel;
                                entity.LinkManAddress = model.LinkManAddress;
                                entity.Status = model.Status;
                                entity.Remark = model.Remark;
                                entity.CreatePerson = model.CreatePerson;
                                entity.CreateTime = ResultHelper.NowTime;
                                entity.ModifyPerson = model.ModifyPerson;
                                entity.ModifyTime = model.ModifyTime;

                                db.WMS_Supplier.Add(entity);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    rtn = false;
                                    db.Entry(entity).State = System.Data.Entity.EntityState.Detached;
                                    errorMessage = ex.InnerException.InnerException.Message;
                                    errors.Add(string.Format(
                                       "第 {0} 列发现错误：{1}{2}",
                                       rowIndex,
                                       errorMessage,
                                       "<br/>"));
                                    wws.Cell(rowIndex + 1, 15).Value = errorMessage;
                                }
                            }
                        }

                        if (rtn)
                        {
                            tran.Commit();  //必须调用Commit()，不然数据不会保存
                        }
                        else
                        {
                            tran.Rollback();    //出错就回滚                        }
                        }
                    }
                    wb.Save();
                }

                return rtn;
            }
        }
    }
}

