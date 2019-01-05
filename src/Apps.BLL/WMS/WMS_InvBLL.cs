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
    public  partial class WMS_InvBLL
    {

        public override List<WMS_InvModel> CreateModelList(ref IQueryable<WMS_Inv> queryData)
        {

            List<WMS_InvModel> modelList = (from r in queryData
                                              select new WMS_InvModel
                                              {
                                                  Id = r.Id,
                                                  InvId = r.InvId,
                                                  PartId = r.PartId,
                                                  Qty = r.Qty,
                                                  SubInvId = r.SubInvId,

                                                  PartCode = r.WMS_Part.PartCode,
                                                  PartName = r.WMS_Part.PartName,
                                                  InvCode = r.WMS_InvInfo.InvCode,
                                                  InvName = r.WMS_InvInfo.InvName,
                                                  SubInvName = r.WMS_SubInvInfo.SubInvName,
                                              }).OrderBy("InvId asc, SubInvId asc, PartId asc")
                                              .ToList();
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
					excelFile.AddMapping<WMS_InvModel>(x => x.InvId, "");
					excelFile.AddMapping<WMS_InvModel>(x => x.SubInvId, "");
					excelFile.AddMapping<WMS_InvModel>(x => x.PartId, "");
					excelFile.AddMapping<WMS_InvModel>(x => x.Qty, "");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_InvModel>(0);

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
								var model = new WMS_InvModel();
								model.Id = row.Id;
								model.InvId = row.InvId;
								model.SubInvId = row.SubInvId;
								model.PartId = row.PartId;
								model.Qty = row.Qty;

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
									WMS_Inv entity = new WMS_Inv();
									entity.Id = model.Id;
									entity.InvId = model.InvId;
									entity.SubInvId = model.SubInvId;
									entity.PartId = model.PartId;
									entity.Qty = model.Qty;

									db.WMS_Inv.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_InvModel model)
		{
		}

		public List<WMS_InvModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_Inv> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}
    }
 }

