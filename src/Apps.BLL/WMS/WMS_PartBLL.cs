using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System;
using Apps.Models.WMS;
using System.IO;
using LinqToExcel;
using ClosedXML.Excel;
using System.Linq.Expressions;
using Apps.IDAL.Sys;
using Unity.Attributes;

namespace Apps.BLL.WMS
{
    public  partial class WMS_PartBLL
    {
        [Dependency]
        public ISysParamRepository m_SysParamRep { get; set; }

        [Dependency]
        public IDAL.WMS.IWMS_PartRepository m_PartRep { get; set; }

        public override List<WMS_PartModel> CreateModelList(ref IQueryable<WMS_Part> queryData)
        {

            List<WMS_PartModel> modelList = (from r in queryData
                                              select new WMS_PartModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  CustomerCode = r.CustomerCode,
                                                  Id = r.Id,
                                                  LogisticsCode = r.LogisticsCode,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  OtherCode = r.OtherCode,
                                                  PartCode = r.PartCode,
                                                  PartName = r.PartName,
                                                  PartType = r.PartType,
                                                  PCS = r.PCS,
                                                  Status = r.Status,
                                                  StoreMan = r.StoreMan,
                                                  Volume = r.Volume,
                                                  Unit = r.Unit,
                                                  Remark = r.Remark,
                                                  SafeStock = r.SafeStock,

                                                  FullPartName = r.PartCode + ":" + r.PartName,
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
                    excelFile.AddMapping<WMS_PartModel>(x => x.PartCode, "物料编码(必输)");
                    excelFile.AddMapping<WMS_PartModel>(x => x.PartName, "物料名称(必输)");
                    excelFile.AddMapping<WMS_PartModel>(x => x.PartType, "物料类型(必输)");
                    excelFile.AddMapping<WMS_PartModel>(x => x.CustomerCode, "主机厂编码");
                    excelFile.AddMapping<WMS_PartModel>(x => x.LogisticsCode, "物流号");
                    excelFile.AddMapping<WMS_PartModel>(x => x.OtherCode, "额外信息编码");
                    excelFile.AddMapping<WMS_PartModel>(x => x.PCS, "每箱数量");
                    excelFile.AddMapping<WMS_PartModel>(x => x.StoreMan, "保管员(必输)");
                    excelFile.AddMapping<WMS_PartModel>(x => x.Unit, "单位");
                    excelFile.AddMapping<WMS_PartModel>(x => x.Volume, "每箱体积");
                    excelFile.AddMapping<WMS_PartModel>(x => x.Remark, "说明");


                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_PartModel>(0);

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
                            var model = new WMS_PartModel();
                            model.Id = row.Id;
                            model.PartCode = row.PartCode;
                            model.PartName = row.PartName;
                            model.PartType = row.PartType;
                            model.CustomerCode = row.CustomerCode;
                            model.LogisticsCode = row.LogisticsCode;
                            model.OtherCode = row.OtherCode;
                            model.PCS = row.PCS;
                            model.StoreMan = row.StoreMan;
                            model.Unit = row.Unit;
                            model.Volume = row.Volume;
                            model.Remark = row.Remark;

                            //model.CreatePerson = row.oper;
                            //model.CreateTime = row.CreateTime;
                            //model.ModifyPerson = row.oper;
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
                                AdditionalCheckExcelData(db,ref model);
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
                            WMS_Part entity = new WMS_Part();
                            entity.Id = model.Id;
                            entity.PartCode = model.PartCode;
                            entity.PartName = model.PartName;
                            entity.PartType = model.PartType;
                            entity.CustomerCode = model.CustomerCode;
                            entity.LogisticsCode = model.LogisticsCode;
                            entity.OtherCode = model.OtherCode;
                            entity.PCS = model.PCS;
                            entity.StoreMan = model.StoreMan;
                            entity.Status = "有效";
                            entity.CreatePerson = oper;
                            entity.CreateTime = DateTime.Now;
                            entity.ModifyPerson = oper;
                            entity.ModifyTime = DateTime.Now;
                            entity.Unit = model.Unit;
                            entity.Volume = model.Volume;
                            entity.Remark = model.Remark;

                            db.WMS_Part.Add(entity);
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
        public bool ImportSafeStock(string oper, string filePath, ref ValidationErrors errors)
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
                    excelFile.AddMapping<WMS_PartModel>(x => x.PartCode, "物料编码(必输)");
                    excelFile.AddMapping<WMS_PartModel>(x => x.SafeStock, "安全库存(必输)");
                    

                    //SheetName，第一个Sheet
                    var excelContent = excelFile.Worksheet<WMS_PartModel>(0);

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
                            var model = new WMS_PartModel();
                            model.Id = row.Id;
                            model.PartCode = row.PartCode;
                            model.SafeStock = row.SafeStock;

                            //model.CreatePerson = row.oper;
                            //model.CreateTime = row.CreateTime;
                            //model.ModifyPerson = row.oper;
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
                                AdditionalCheckSafeStock(db, ref model);
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
                            WMS_Part entity = m_Rep.GetById(model.Id);
                            entity.Id = model.Id;
                            //entity.PartCode = model.PartCode;
                            entity.SafeStock = model.SafeStock;
                            entity.ModifyPerson = oper;
                            entity.ModifyTime = DateTime.Now;
                            m_Rep.Edit(entity);

                            //db.WMS_Part.Add(entity);
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

        public void AdditionalCheckExcelData(DBContainer db, ref WMS_PartModel model)
        {
            //获取物料ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var partCode = model.PartCode;
                Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

                //var part = m_PartRep.GetSingleWhere(exp);
                var part = db.WMS_Part.FirstOrDefault(exp);
                if (part != null)
                {
                    throw new Exception("物料编码重复！");
                }

            }
            else
            {
                throw new Exception("物料编码不能为空！");
            }
            //获取物料类型
            if (!String.IsNullOrEmpty(model.PartType))
            {
                var partType = model.PartType;                
                Expression<Func<SysParam, bool>> exp = x => x.ParamName == partType && x.TypeCode == "PartType";

                var part = m_SysParamRep.GetSingleWhere(exp);
                if (part == null)
                {
                    throw new Exception("物料类型不存在！");
                }
            }
            //物料编码不能为空
            if (String.IsNullOrEmpty(model.PartCode))
            {
                throw new Exception("物料编码不能为空！");
            }
            //物料名称不能为空
            if (String.IsNullOrEmpty(model.PartName))
            {
                throw new Exception("物料名称不能为空！");
            }
            //保管员不能为空
            if (String.IsNullOrEmpty(model.StoreMan))
            {
                throw new Exception("保管员不能为空！");
            }

        }
        public void AdditionalCheckSafeStock(DBContainer db, ref WMS_PartModel model)
        {
            //验证物料编码
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
                else { model.Id = part.Id; }

            }
            else
            {
                throw new Exception("物料编码不能为空！");
            }
            //安全库存不能为空
            if (model.SafeStock < 0)
            {
                throw new Exception("安全库存不能小于0！");
            }
            

        }

        public List<WMS_PartModel> GetList(ref GridPager pager, string partCode, string partName)
        {
            IQueryable<WMS_Part> queryData = null;
            string code = partCode ?? "";
            string name = partName ?? "";
            queryData = m_Rep.GetList().Where(x => x.PartCode.Contains(code) && x.PartName.Contains(name));
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public List<WMS_PartModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_Part> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
    }
}

