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
    public  partial class WMS_InvRecordBLL
    {

        public override List<WMS_InvRecordModel> CreateModelList(ref IQueryable<WMS_InvRecord> queryData)
        {

            List<WMS_InvRecordModel> modelList = (from r in queryData
                                              select new WMS_InvRecordModel
                                              {
                                                  BillId = r.BillId,
                                                  Id = r.Id,
                                                  InvId = r.InvId,
                                                  Lot = r.Lot,
                                                  OperateDate = r.OperateDate,
                                                  OperateMan = r.OperateMan,
                                                  PartId = r.PartId,
                                                  QTY = r.QTY,
                                                  SourceBill = r.SourceBill,
                                                  Stock_InvId = r.Stock_InvId,
                                                  StockStatus = r.StockStatus,
                                                  SubInvId = r.SubInvId,
                                                  Type = r.Type,
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
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.PartId, "物料编码");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.QTY, "数量");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.InvId, "库房编码");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.SubInvId, "");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.BillId, "单据ID");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.SourceBill, "单据来源");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.OperateDate, "操作时间");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.Lot, "");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.Type, "出入库类型");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.OperateMan, "操作人");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.Stock_InvId, "备料库存");
					excelFile.AddMapping<WMS_InvRecordModel>(x => x.StockStatus, "备料状态：1-不适用（直接修改库存现有量）；2-已备料；3-无效备料（取消备料后将2改成3）；4-取消备料（当前操作是取消备料）");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_InvRecordModel>(0);

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
								var model = new WMS_InvRecordModel();
								model.Id = row.Id;
								model.PartId = row.PartId;
								model.QTY = row.QTY;
								model.InvId = row.InvId;
								model.SubInvId = row.SubInvId;
								model.BillId = row.BillId;
								model.SourceBill = row.SourceBill;
								model.OperateDate = row.OperateDate;
								model.Lot = row.Lot;
								model.Type = row.Type;
								model.OperateMan = row.OperateMan;
								model.Stock_InvId = row.Stock_InvId;
								model.StockStatus = row.StockStatus;

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
									WMS_InvRecord entity = new WMS_InvRecord();
									entity.Id = model.Id;
									entity.PartId = model.PartId;
									entity.QTY = model.QTY;
									entity.InvId = model.InvId;
									entity.SubInvId = model.SubInvId;
									entity.BillId = model.BillId;
									entity.SourceBill = model.SourceBill;
									entity.OperateDate = model.OperateDate;
									entity.Lot = model.Lot;
									entity.Type = model.Type;
									entity.OperateMan = model.OperateMan;
									entity.Stock_InvId = model.Stock_InvId;
									entity.StockStatus = model.StockStatus;


									db.WMS_InvRecord.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_InvRecordModel model)
		{
		}

		public List<WMS_InvRecordModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_InvRecord> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}
    }
 }

