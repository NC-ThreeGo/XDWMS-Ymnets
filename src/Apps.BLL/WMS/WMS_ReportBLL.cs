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
    public  partial class WMS_ReportBLL
    {

        public override List<WMS_ReportModel> CreateModelList(ref IQueryable<WMS_Report> queryData)
        {

            List<WMS_ReportModel> modelList = (from r in queryData
                                              select new WMS_ReportModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  DataSource = r.DataSource,
                                                  DataSourceType = r.DataSourceType,
                                                  FileName = r.FileName,
                                                  Id = r.Id,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  Remark = r.Remark,
                                                  ReportCode = r.ReportCode,
                                                  ReportName = r.ReportName,
                                                  ReportType = r.ReportType,
                                                  Status = r.Status,
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
					excelFile.AddMapping<WMS_ReportModel>(x => x.ReportCode, "报表编码");
					excelFile.AddMapping<WMS_ReportModel>(x => x.ReportName, "报表名称");
					excelFile.AddMapping<WMS_ReportModel>(x => x.ReportType, "报表类型：1-单据，2-报表");
					excelFile.AddMapping<WMS_ReportModel>(x => x.Remark, "备注");
					excelFile.AddMapping<WMS_ReportModel>(x => x.DataSource, "数据源");
					excelFile.AddMapping<WMS_ReportModel>(x => x.DataSourceType, "数据源类型：1-SQL语句；2-存储过程");
					excelFile.AddMapping<WMS_ReportModel>(x => x.FileName, "报表文件");
					excelFile.AddMapping<WMS_ReportModel>(x => x.Status, "状态");
					excelFile.AddMapping<WMS_ReportModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_ReportModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_ReportModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_ReportModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_ReportModel>(0);

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
								var model = new WMS_ReportModel();
								model.Id = row.Id;
								model.ReportCode = row.ReportCode;
								model.ReportName = row.ReportName;
								model.ReportType = row.ReportType;
								model.Remark = row.Remark;
								model.DataSource = row.DataSource;
								model.DataSourceType = row.DataSourceType;
								model.FileName = row.FileName;
								model.Status = row.Status;
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
									WMS_Report entity = new WMS_Report();
									entity.Id = model.Id;
									entity.ReportCode = model.ReportCode;
									entity.ReportName = model.ReportName;
									entity.ReportType = model.ReportType;
									entity.Remark = model.Remark;
									entity.DataSource = model.DataSource;
									entity.DataSourceType = model.DataSourceType;
									entity.FileName = model.FileName;
									entity.Status = model.Status;
									entity.CreatePerson = model.CreatePerson;
									entity.CreateTime = model.CreateTime;
									entity.ModifyPerson = model.ModifyPerson;
									entity.ModifyTime = model.ModifyTime;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.WMS_Report.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_ReportModel model)
		{
		}

		public List<WMS_ReportModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_Report> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}
    }
 }

