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
    public  partial class WMS_ReportParamBLL
    {

    public override List<WMS_ReportParamModel> GetListByParentId(ref GridPager pager, string queryStr, object parentId)
    {
        IQueryable<WMS_ReportParam> queryData = null;
        int pid = Convert.ToInt32(parentId);
        if (pid != 0)
        {
        queryData = m_Rep.GetList(a => a.ReportId == pid);
        }
        else
        {
        queryData = m_Rep.GetList();
        }
        if (!string.IsNullOrWhiteSpace(queryStr))
        {
            queryData = m_Rep.GetList(
                        a => (
                                a.ParamCode.Contains(queryStr)
                               || a.InputNo.Contains(queryStr)
                               || a.ParamName.Contains(queryStr)
                               || a.ShowName.Contains(queryStr)
                               || a.ParamType.Contains(queryStr)
                               || a.ParamData.Contains(queryStr)
                               || a.DefaultValue.Contains(queryStr)
                               || a.ParamElement.Contains(queryStr)
                               || a.Remark.Contains(queryStr)
                               || a.CreatePerson.Contains(queryStr)
                               || a.ModifyPerson.Contains(queryStr)
                             )
                        );
        }
        pager.totalRows = queryData.Count();
        queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
        return CreateModelList(ref queryData);
    }
        public override List<WMS_ReportParamModel> CreateModelList(ref IQueryable<WMS_ReportParam> queryData)
        {

            List<WMS_ReportParamModel> modelList = (from r in queryData
                                              select new WMS_ReportParamModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  DefaultValue = r.DefaultValue,
                                                  Id = r.Id,
                                                  InputNo = r.InputNo,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  ParamCode = r.ParamCode,
                                                  ParamData = r.ParamData,
                                                  ParamElement = r.ParamElement,
                                                  ParamName = r.ParamName,
                                                  ParamType = r.ParamType,
                                                  Remark = r.Remark,
                                                  ReportId = r.ReportId,
                                                  ShowName = r.ShowName,
                                                  ReportName = r.WMS_Report.ReportName,
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
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ParamCode, "参数代码");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ReportId, "报表ID");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.InputNo, "");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ParamName, "参数名");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ShowName, "显示名称");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ParamType, "参数类型：varchar、int、datetime");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ParamData, "可选值");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.DefaultValue, "默认值");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ParamElement, "显示元素：文本框、下拉框、日期框等");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.Remark, "备注");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_ReportParamModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_ReportParamModel>(0);

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
								var model = new WMS_ReportParamModel();
								model.Id = row.Id;
								model.ParamCode = row.ParamCode;
								model.ReportId = row.ReportId;
								model.InputNo = row.InputNo;
								model.ParamName = row.ParamName;
								model.ShowName = row.ShowName;
								model.ParamType = row.ParamType;
								model.ParamData = row.ParamData;
								model.DefaultValue = row.DefaultValue;
								model.ParamElement = row.ParamElement;
								model.Remark = row.Remark;
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
									WMS_ReportParam entity = new WMS_ReportParam();
									entity.Id = model.Id;
									entity.ParamCode = model.ParamCode;
									entity.ReportId = model.ReportId;
									entity.InputNo = model.InputNo;
									entity.ParamName = model.ParamName;
									entity.ShowName = model.ShowName;
									entity.ParamType = model.ParamType;
									entity.ParamData = model.ParamData;
									entity.DefaultValue = model.DefaultValue;
									entity.ParamElement = model.ParamElement;
									entity.Remark = model.Remark;
									entity.CreatePerson = model.CreatePerson;
									entity.CreateTime = model.CreateTime;
									entity.ModifyPerson = model.ModifyPerson;
									entity.ModifyTime = model.ModifyTime;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.WMS_ReportParam.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_ReportParamModel model)
		{
		}

		public List<WMS_ReportParamModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_ReportParam> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}
    }
 }

