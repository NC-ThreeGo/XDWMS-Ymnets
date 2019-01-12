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

namespace Apps.BLL.WMS
{
    public  partial class WMS_ReInspectBLL
    {

        public override List<WMS_ReInspectModel> CreateModelList(ref IQueryable<WMS_ReInspect> queryData)
        {

            List<WMS_ReInspectModel> modelList = (from r in queryData
                                                  select new WMS_ReInspectModel
                                                  {
                                                      AdjustDate = r.AdjustDate,
                                                      AdjustMan = r.AdjustMan,
                                                      AIId = r.AIId,
                                                      Attr1 = r.Attr1,
                                                      Attr2 = r.Attr2,
                                                      Attr3 = r.Attr3,
                                                      Attr4 = r.Attr4,
                                                      Attr5 = r.Attr5,
                                                      CreatePerson = r.CreatePerson,
                                                      CreateTime = r.CreateTime,
                                                      Id = r.Id,
                                                      ModifyPerson = r.ModifyPerson,
                                                      ModifyTime = r.ModifyTime,
                                                      NCheckOutDate = r.NCheckOutDate,
                                                      NCheckOutRemark = r.NCheckOutRemark,
                                                      NCheckOutResult = r.NCheckOutResult,
                                                      NNoQualifyQty = r.NNoQualifyQty,
                                                      NQualifyQty = r.NQualifyQty,
                                                      OCheckOutDate = r.OCheckOutDate,
                                                      OCheckOutRemark = r.OCheckOutRemark,
                                                      OCheckOutResult = r.OCheckOutResult,
                                                      ONoQualifyQty = r.ONoQualifyQty,
                                                      OQualifyQty = r.OQualifyQty,
                                                      Remark = r.Remark,
                                                      InspectBillNum = r.WMS_AI.InspectBillNum,
                                                      PO = r.WMS_AI.WMS_PO.PO,
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
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.AIId, "到货送检单ID");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.OCheckOutResult, "原送检单结果");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.OQualifyQty, "原送检单合格数量");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.ONoQualifyQty, "原送检单不合格数量");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.OCheckOutRemark, "原送检单说明");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.OCheckOutDate, "原送检单检验日期");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.NCheckOutResult, "新送检单结果");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.NQualifyQty, "新送检单合格数量");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.NNoQualifyQty, "新送检单不合格数量");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.NCheckOutRemark, "新送检单检验结果");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.NCheckOutDate, "新送检单检验日期");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.Remark, "调整说明");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.AdjustMan, "调整人");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.AdjustDate, "调整时间");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.Attr1, "");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.Attr2, "");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.Attr3, "");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.Attr4, "");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.Attr5, "");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.CreatePerson, "创建时间");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.CreateTime, "创建人");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_ReInspectModel>(x => x.ModifyTime, "修改人");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_ReInspectModel>(0);

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
								var model = new WMS_ReInspectModel();
								model.Id = row.Id;
								model.AIId = row.AIId;
								model.OCheckOutResult = row.OCheckOutResult;
								model.OQualifyQty = row.OQualifyQty;
								model.ONoQualifyQty = row.ONoQualifyQty;
								model.OCheckOutRemark = row.OCheckOutRemark;
								model.OCheckOutDate = row.OCheckOutDate;
								model.NCheckOutResult = row.NCheckOutResult;
								model.NQualifyQty = row.NQualifyQty;
								model.NNoQualifyQty = row.NNoQualifyQty;
								model.NCheckOutRemark = row.NCheckOutRemark;
								model.NCheckOutDate = row.NCheckOutDate;
								model.Remark = row.Remark;
								model.AdjustMan = row.AdjustMan;
								model.AdjustDate = row.AdjustDate;
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
									WMS_ReInspect entity = new WMS_ReInspect();
									entity.Id = model.Id;
									entity.AIId = model.AIId;
									entity.OCheckOutResult = model.OCheckOutResult;
									entity.OQualifyQty = model.OQualifyQty;
									entity.ONoQualifyQty = model.ONoQualifyQty;
									entity.OCheckOutRemark = model.OCheckOutRemark;
									entity.OCheckOutDate = model.OCheckOutDate;
									entity.NCheckOutResult = model.NCheckOutResult;
									entity.NQualifyQty = model.NQualifyQty;
									entity.NNoQualifyQty = model.NNoQualifyQty;
									entity.NCheckOutRemark = model.NCheckOutRemark;
									entity.NCheckOutDate = model.NCheckOutDate;
									entity.Remark = model.Remark;
									entity.AdjustMan = model.AdjustMan;
									entity.AdjustDate = model.AdjustDate;
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

									db.WMS_ReInspect.Add(entity);
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

        public void AdditionalCheckExcelData(ref WMS_ReInspectModel model)
		{
		}

		public List<WMS_ReInspectModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_ReInspect> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}
        public override bool Create(ref ValidationErrors errors, WMS_ReInspectModel model)
        {
            //ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            var returnValue = m_Rep.CreateReInspect(model.CreatePerson,model.AIId,model.NCheckOutResult,model.NQualifyQty,model.NNoQualifyQty,model.NCheckOutRemark,model.NCheckOutDate,model.Remark);
            if (returnValue == null)
            {
                return true;
            }
            else
            {
                //tran.Rollback();
                errors.Add((string)returnValue);
                return false;
            }

        }
    }
 }

