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
using Apps.Locale;
using Apps.BLL.Core;

namespace Apps.BLL.WMS
{
    public  partial class WMS_ReturnOrderBLL
    {
        public override List<WMS_ReturnOrderModel> CreateModelList(ref IQueryable<WMS_ReturnOrder> queryData)
        {
            List<WMS_ReturnOrderModel> modelList = (from r in queryData
                                              select new WMS_ReturnOrderModel
                                              {
                                                  AdjustQty = r.AdjustQty,
                                                  AIID = r.AIID,
                                                  Attr1 = r.Attr1,
                                                  Attr2 = r.Attr2,
                                                  Attr3 = r.Attr3,
                                                  Attr4 = r.Attr4,
                                                  Attr5 = r.Attr5,
                                                  //ConfirmDate = r.ConfirmDate,
                                                  //ConfirmMan = r.ConfirmMan,
                                                  //ConfirmStatus = r.ConfirmStatus,
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Id = r.Id,
                                                  InvId = r.InvId,
                                                  Lot = r.Lot,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  PartID = r.PartID,
                                                  //PrintDate = r.PrintDate,
                                                  //PrintMan = r.PrintMan,
                                                  //PrintStaus = r.PrintStaus,
                                                  //Remark = r.Remark,
                                                  //ReturnOrderNum = r.ReturnOrderNum,
                                                  ReturnQty = r.ReturnQty,
                                                  SubInvId = r.SubInvId,
                                                  SupplierId = r.SupplierId,
                                                  Status = r.Status,

                                                  ArrivalBillNum = r.WMS_AI.ArrivalBillNum,
                                                  InspectBillNum = r.WMS_AI.InspectBillNum,
                                                  SupplierShortName = r.WMS_Supplier.SupplierShortName,
                                                  PartCode = r.WMS_Part.PartCode,
                                                  PartName =  r.WMS_Part.PartName,
                                                  InvCode = r.WMS_InvInfo.InvCode,
                                                  InvName = r.WMS_InvInfo.InvName,
                                                  ArrivalQty = r.WMS_AI.ArrivalQty,
                                                  QualifyNum = r.WMS_AI.QualifyQty,
                                                  NoQualifyNum = r.WMS_AI.NoQualifyQty,
                                                  SupplierCode = r.WMS_Supplier.SupplierCode,
                                                  SupplierName = r.WMS_Supplier.SupplierName
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
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ReturnOrderNum, "退货单号");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.AIID, "到货检验单ID");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.PartID, "物料编码");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.SupplierId, "代理商编码");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.InvId, "库存编码");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.SubInvId, "");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ReturnQty, "应退货数量");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.AdjustQty, "实际退货数量");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.Remark, "退货说明");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.PrintStaus, "打印状态");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.PrintDate, "打印时间");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.PrintMan, "打印人");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ConfirmStatus, "确认状态");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ConfirmMan, "确认人");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ConfirmDate, "确认时间");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.Attr1, "");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.Attr2, "");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.Attr3, "");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.Attr4, "");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.Attr5, "");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_ReturnOrderModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_ReturnOrderModel>(0);

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
								var model = new WMS_ReturnOrderModel();
								model.Id = row.Id;
								model.ReturnOrderNum = row.ReturnOrderNum;
								model.AIID = row.AIID;
								model.PartID = row.PartID;
								model.SupplierId = row.SupplierId;
								model.InvId = row.InvId;
								model.SubInvId = row.SubInvId;
								model.ReturnQty = row.ReturnQty;
								model.AdjustQty = row.AdjustQty;
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

								if (!String.IsNullOrEmpty(errorMessage))
								{
									rtn = false;
									errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
									wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
									continue;								}
								
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
									WMS_ReturnOrder entity = new WMS_ReturnOrder();
									entity.Id = model.Id;
									entity.ReturnOrderNum = model.ReturnOrderNum;
									entity.AIID = model.AIID;
									entity.PartID = model.PartID;
									entity.SupplierId = model.SupplierId;
									entity.InvId = model.InvId;
									entity.SubInvId = model.SubInvId;
									entity.ReturnQty = model.ReturnQty;
									entity.AdjustQty = model.AdjustQty;
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
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.WMS_ReturnOrder.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_ReturnOrderModel model)
		{
		}

		public List<WMS_ReturnOrderModel> GetListByWhere(ref GridPager pager, string where)
		{
            try
            {
                IQueryable<WMS_ReturnOrder> queryData = null;
                queryData = m_Rep.GetList().Where(where);

                //var a = queryData.Select(p => p.WMS_Part);

                pager.totalRows = queryData.Count();
                //排序
                queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                return CreateModelList(ref queryData);
            }
            catch(Exception ex)
            {
                throw ex;
            }
		}

        public List<WMS_ReturnOrderModel> GetListByWhereAndGroupBy(ref GridPager pager, string where)
        {
            try
            {
                IQueryable<WMS_ReturnOrder> queryData = null;
                queryData = m_Rep.GetList().Where(where)
                    .GroupBy(p => new { p.ReturnOrderNum, p.SupplierId })
                    .Select(g => g.FirstOrDefault())
                    .OrderBy(p => p.ReturnOrderNum);
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

        public List<WMS_ReturnOrderModel> GetListByWhereAndGroupBySupplierId(ref GridPager pager, string where)
        {
            try
            {
                IQueryable<WMS_ReturnOrder> queryData = null;
                queryData = m_Rep.GetList().Where(where).Where(p => Math.Abs(p.AdjustQty) < Math.Abs(p.ReturnQty))
                .GroupBy(p => new { p.SupplierId })
                    .Select(g => g.FirstOrDefault())
                    .OrderBy(p => p.SupplierId);
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

        public bool CreateBatchReturnOrder(ref ValidationErrors errors, string opt, string jsonReturnOrder)
        {
            string result = String.Empty;
            try
            {
                result = m_Rep.CreateBatchReturnOrder(opt, jsonReturnOrder);
                if (String.IsNullOrEmpty(result))
                {
                    return true;
                }
                else
                {
                    errors.Add(result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }

        public bool CreateReturnOrder(ref ValidationErrors errors, string opt, WMS_ReturnOrderModel model)
        {
            try
            {
                var message = m_Rep.CreateReturnOrder(opt, model.PartID, model.SupplierId, model.InvId, model.Lot, model.AdjustQty, model.Remark);
                if (String.IsNullOrEmpty(message))
                    return true;
                else
                {
                    errors.Add(message);
                    return false;
                }
            }
            catch(Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }

        //public string PrintReturnOrder(ref ValidationErrors errors, string opt, string jsonReturnOrderNum)
        //{
        //    try
        //    {
        //        var rtn = m_Rep.PrintReturnOrder(opt, jsonReturnOrderNum);
        //        if (!String.IsNullOrEmpty(rtn))
        //        {
        //            return rtn;
        //        }
        //        else
        //            return null;
        //    }
        //    catch(Exception ex)
        //    {
        //        errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //        return null;
        //    }
        //}

        //public bool ConfirmReturnOrder(ref ValidationErrors errors, string opt, string returnOrderNum)
        //{
        //    try
        //    {
        //        m_Rep.ConfirmReturnOrder(opt, returnOrderNum);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //        return false;
        //    }
        //}

        public virtual bool CancelReturnOrder(ref ValidationErrors errors, string opt, int id)
        {
            try
            {
                //Expression<Func<WMS_ReturnOrder, bool>> exp = x => x.Id == aiId && x.ConfirmStatus == "未确认";
                Expression<Func<WMS_ReturnOrder, bool>> exp = x => x.Id == id;
                WMS_ReturnOrder entity = m_Rep.GetSingleWhere(exp);
                if (entity == null)
                {
                    //errors.Add(Resource.Disable);
                    errors.Add(" :单据不存在");
                    return false;
                }
                //entity.PrintStaus = "已失效";
                entity.Status = "无效";
                entity.ModifyPerson = opt;
                entity.ModifyTime = DateTime.Now;

                if (m_Rep.Edit(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.NoDataChange);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
    }
}

