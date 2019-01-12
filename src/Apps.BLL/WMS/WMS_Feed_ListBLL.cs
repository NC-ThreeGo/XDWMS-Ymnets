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
    public  partial class WMS_Feed_ListBLL
    {

        public override List<WMS_Feed_ListModel> CreateModelList(ref IQueryable<WMS_Feed_List> queryData)
        {

            List<WMS_Feed_ListModel> modelList = (from r in queryData
                                              select new WMS_Feed_ListModel
                                              {
                                                  AssemblyPartId = r.AssemblyPartId,
                                                  Attr1 = r.Attr1,
                                                  Attr2 = r.Attr2,
                                                  Attr3 = r.Attr3,
                                                  Attr4 = r.Attr4,
                                                  Attr5 = r.Attr5,
                                                  BoxQty = r.BoxQty,
                                                  Capacity = r.Capacity,
                                                  ConfirmDate = r.ConfirmDate,
                                                  ConfirmMan = r.ConfirmMan,
                                                  ConfirmStatus = r.ConfirmStatus,
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Department = r.Department,
                                                  FeedBillNum = r.FeedBillNum,
                                                  FeedQty = r.FeedQty,
                                                  Id = r.Id,
                                                  InvId = r.InvId,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  PrintDate = r.PrintDate,
                                                  PrintMan = r.PrintMan,
                                                  PrintStaus = r.PrintStaus,
                                                  ReleaseBillNum = r.ReleaseBillNum,
                                                  Remark = r.Remark,
                                                  SubAssemblyPartId = r.SubAssemblyPartId,
                                                  SubInvId = r.SubInvId,
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
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.FeedBillNum, "投料单号（业务）");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ReleaseBillNum, "投料单号（系统）");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Department, "投料部门");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.AssemblyPartId, "总成物料");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.SubAssemblyPartId, "投料物料");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.FeedQty, "投料数量");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.BoxQty, "箱数");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Capacity, "体积");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.InvId, "库存");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.SubInvId, "子库存");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Remark, "备注");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.PrintStaus, "打印状态");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.PrintDate, "打印时间");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.PrintMan, "打印人");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ConfirmStatus, "确认状态");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ConfirmMan, "确认人");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ConfirmDate, "确认时间");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr1, "");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr2, "");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr3, "");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr4, "");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr5, "");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_Feed_ListModel>(0);

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
								var model = new WMS_Feed_ListModel();
								model.Id = row.Id;
								model.FeedBillNum = row.FeedBillNum;
								model.ReleaseBillNum = row.ReleaseBillNum;
								model.Department = row.Department;
								model.AssemblyPartId = row.AssemblyPartId;
								model.SubAssemblyPartId = row.SubAssemblyPartId;
								model.FeedQty = row.FeedQty;
								model.BoxQty = row.BoxQty;
								model.Capacity = row.Capacity;
								model.InvId = row.InvId;
								model.SubInvId = row.SubInvId;
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
									WMS_Feed_List entity = new WMS_Feed_List();
									entity.Id = model.Id;
									entity.FeedBillNum = model.FeedBillNum;
									entity.ReleaseBillNum = model.ReleaseBillNum;
									entity.Department = model.Department;
									entity.AssemblyPartId = model.AssemblyPartId;
									entity.SubAssemblyPartId = model.SubAssemblyPartId;
									entity.FeedQty = model.FeedQty;
									entity.BoxQty = model.BoxQty;
									entity.Capacity = model.Capacity;
									entity.InvId = model.InvId;
									entity.SubInvId = model.SubInvId;
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

									db.WMS_Feed_List.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_Feed_ListModel model)
		{
		}

		public List<WMS_Feed_ListModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_Feed_List> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}
    }
 }

