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
    public  partial class WMS_ReturnInspectionBLL
    {

        public override List<WMS_ReturnInspectionModel> CreateModelList(ref IQueryable<WMS_ReturnInspection> queryData)
        {

            List<WMS_ReturnInspectionModel> modelList = (from r in queryData
                                              select new WMS_ReturnInspectionModel
                                              {
                                                  Attr1 = r.Attr1,
                                                  Attr2 = r.Attr2,
                                                  Attr3 = r.Attr3,
                                                  Attr4 = r.Attr4,
                                                  Attr5 = r.Attr5,
                                                  CheckOutResult = r.CheckOutResult,
                                                  ConfirmDate = r.ConfirmDate,
                                                  ConfirmMan = r.ConfirmMan,
                                                  ConfirmRemark = r.ConfirmRemark,
                                                  ConfirmStatus = r.ConfirmStatus,
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  PartCustomerCode = r.PartCustomerCode,
                                                  PartCustomerCodeName = r.PartCustomerCodeName,
                                                  CustomerId = r.CustomerId,
                                                  Id = r.Id,
                                                  InspectDate = r.InspectDate,
                                                  InspectMan = r.InspectMan,
                                                  InspectStatus = r.InspectStatus,
                                                  InvId = r.InvId,
                                                  Lot = r.Lot,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  NoQualifyQty = r.NoQualifyQty,
                                                  PartID = r.PartID,
                                                  PCS = r.PCS,
                                                  PrintDate = r.PrintDate,
                                                  PrintMan = r.PrintMan,
                                                  PrintStatus = r.PrintStatus,
                                                  Qty = r.Qty,
                                                  QualifyQty = r.QualifyQty,
                                                  Remark = r.Remark,
                                                  ReturnInspectionNum = r.ReturnInspectionNum,
                                                  SubInvId = r.SubInvId,
                                                  SupplierId = r.SupplierId,
                                                  Volume = r.Volume,

                                                  PartCode = r.WMS_Part.PartCode,
                                                  PartName = r.WMS_Part.PartName,
                                                  CustomerShortName = r.WMS_Customer.CustomerShortName,
                                                  SupplierShortName = r.WMS_Supplier.SupplierShortName,
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
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ReturnInspectionNum, "退货送检单号");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PartCustomerCode, "客户图号");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PartCustomerCodeName, "零件名称");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PartID, "新电图号");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Qty, "数量");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.CustomerId, "客户");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.SupplierId, "供应商");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PCS, "箱数");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Volume, "体积");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.InvId, "库房");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.SubInvId, "子库房");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PrintStatus, "打印状态");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PrintDate, "打印日期");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.PrintMan, "打印人");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Remark, "备注");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.InspectMan, "检验人");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.InspectDate, "检验日期");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.InspectStatus, "检验状态");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.CheckOutResult, "检验结果");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.QualifyQty, "合格数量");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.NoQualifyQty, "不合格数量");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Lot, "批次");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ConfirmStatus, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ConfirmMan, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ConfirmDate, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ConfirmRemark, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Attr1, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Attr2, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Attr3, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Attr4, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.Attr5, "");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_ReturnInspectionModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_ReturnInspectionModel>(0);

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
								var model = new WMS_ReturnInspectionModel();
								model.Id = row.Id;
								model.ReturnInspectionNum = row.ReturnInspectionNum;
								model.PartCustomerCode = row.PartCustomerCode;
								model.PartCustomerCodeName = row.PartCustomerCodeName;
								model.PartID = row.PartID;
								model.Qty = row.Qty;
								model.CustomerId = row.CustomerId;
								model.SupplierId = row.SupplierId;
								model.PCS = row.PCS;
								model.Volume = row.Volume;
								model.InvId = row.InvId;
								model.SubInvId = row.SubInvId;
								model.PrintStatus = row.PrintStatus;
								model.PrintDate = row.PrintDate;
								model.PrintMan = row.PrintMan;
								model.Remark = row.Remark;
								model.InspectMan = row.InspectMan;
								model.InspectDate = row.InspectDate;
								model.InspectStatus = row.InspectStatus;
								model.CheckOutResult = row.CheckOutResult;
								model.QualifyQty = row.QualifyQty;
								model.NoQualifyQty = row.NoQualifyQty;
								model.Lot = row.Lot;
								model.ConfirmStatus = row.ConfirmStatus;
								model.ConfirmMan = row.ConfirmMan;
								model.ConfirmDate = row.ConfirmDate;
								model.ConfirmRemark = row.ConfirmRemark;
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
									WMS_ReturnInspection entity = new WMS_ReturnInspection();
									entity.Id = model.Id;
									entity.ReturnInspectionNum = model.ReturnInspectionNum;
									entity.PartCustomerCode = model.PartCustomerCode;
									entity.PartCustomerCodeName = model.PartCustomerCodeName;
									entity.PartID = model.PartID;
									entity.Qty = model.Qty;
									entity.CustomerId = model.CustomerId;
									entity.SupplierId = model.SupplierId;
									entity.PCS = model.PCS;
									entity.Volume = model.Volume;
									entity.InvId = model.InvId;
									entity.SubInvId = model.SubInvId;
									entity.PrintStatus = model.PrintStatus;
									entity.PrintDate = model.PrintDate;
									entity.PrintMan = model.PrintMan;
									entity.Remark = model.Remark;
									entity.InspectMan = model.InspectMan;
									entity.InspectDate = model.InspectDate;
									entity.InspectStatus = model.InspectStatus;
									entity.CheckOutResult = model.CheckOutResult;
									entity.QualifyQty = model.QualifyQty;
									entity.NoQualifyQty = model.NoQualifyQty;
									entity.Lot = model.Lot;
									entity.ConfirmStatus = model.ConfirmStatus;
									entity.ConfirmMan = model.ConfirmMan;
									entity.ConfirmDate = model.ConfirmDate;
									entity.ConfirmRemark = model.ConfirmRemark;
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

									db.WMS_ReturnInspection.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_ReturnInspectionModel model)
		{
		}

		public List<WMS_ReturnInspectionModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_ReturnInspection> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}

        public string CreateBatchReturnInspection(ref ValidationErrors errors, string opt, string jsonReturnInspection)
        {
            string result = String.Empty;
            try
            {
                result = m_Rep.CreateBatchReturnInspection(opt, jsonReturnInspection);
                if (!String.IsNullOrEmpty(result))
                {
                    return result;
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

        public bool ProcessReturnInspectBill(ref ValidationErrors errors, string opt, string jsonReturnInspectBill)
        {
            string result = String.Empty;
            try
            {
                result = m_Rep.ProcessReturnInspectBill(opt, jsonReturnInspectBill);
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
    }
}

