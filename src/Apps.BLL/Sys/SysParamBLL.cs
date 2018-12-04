using Apps.Common;
using Apps.Models;
using Apps.Models.Sys;
using ClosedXML.Excel;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web.Mvc;

namespace Apps.BLL.Sys
{
    public  partial class SysParamBLL
    {

        public override List<SysParamModel> CreateModelList(ref IQueryable<SysParam> queryData)
        {

            List<SysParamModel> modelList = (from r in queryData
                                              select new SysParamModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Id = r.Id,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  ParamCode = r.ParamCode,
                                                  ParamName = r.ParamName,
                                                  TypeCode = r.TypeCode,
                                                  TypeName = r.TypeName,
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
					excelFile.AddMapping<SysParamModel>(x => x.TypeCode, "");
					excelFile.AddMapping<SysParamModel>(x => x.TypeName, "");
					excelFile.AddMapping<SysParamModel>(x => x.ParamCode, "");
					excelFile.AddMapping<SysParamModel>(x => x.ParamName, "");
					excelFile.AddMapping<SysParamModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<SysParamModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<SysParamModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<SysParamModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<SysParamModel>(0);

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
								var model = new SysParamModel();
								model.Id = row.Id;
								model.TypeCode = row.TypeCode;
								model.TypeName = row.TypeName;
								model.ParamCode = row.ParamCode;
								model.ParamName = row.ParamName;
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
									SysParam entity = new SysParam();
									entity.Id = model.Id;
									entity.TypeCode = model.TypeCode;
									entity.TypeName = model.TypeName;
									entity.ParamCode = model.ParamCode;
									entity.ParamName = model.ParamName;
									entity.CreatePerson = model.CreatePerson;
									entity.CreateTime = model.CreateTime;
									entity.ModifyPerson = model.ModifyPerson;
									entity.ModifyTime = model.ModifyTime;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.SysParam.Add(entity);
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

		public void AdditionalCheckExcelData(ref SysParamModel model)
		{
		}

		public List<SysParamModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<SysParam> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}

        public static SelectList GetSysParamByType(string typeCode)
        {
            using (DBContainer db = new DBContainer())
            {
                var list = db.SysParam.Where(x => x.TypeCode == typeCode).OrderBy(x => x.ParamCode).ToList();
                return new SelectList(list, "ParamCode", "ParamName");
            }
        }
    }
}

