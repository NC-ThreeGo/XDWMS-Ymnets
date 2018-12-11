using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System;
using System.IO;
using LinqToExcel;
using ClosedXML.Excel;
using Apps.Models.WMS;

namespace Apps.BLL.WMS
{
    public  partial class WMS_InvInfoBLL
    {

        public override List<WMS_InvInfoModel> CreateModelList(ref IQueryable<WMS_InvInfo> queryData)
        {

            List<WMS_InvInfoModel> modelList = (from r in queryData
                                              select new WMS_InvInfoModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Id = r.Id,
                                                  InvCode = r.InvCode,
                                                  InvName = r.InvName,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  Remark = r.Remark,
                                                  Status = r.Status,
                                              }).ToList();
            return modelList;
        }

		public bool ImportExcelData(string filePath, ref ValidationErrors errors)
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
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.InvCode, "库房编码");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.InvName, "库房名称");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.Remark, "说明");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.Status, "状态");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_InvInfoModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_InvInfoModel>(0);

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
								var model = new WMS_InvInfoModel();
								model.Id = row.Id;
								model.InvCode = row.InvCode;
								model.InvName = row.InvName;
								model.Remark = row.Remark;
								model.Status = row.Status;
								model.CreatePerson = row.CreatePerson;
								model.CreateTime = row.CreateTime;
								model.ModifyPerson = row.ModifyPerson;
								model.ModifyTime = row.ModifyTime;

								if (!String.IsNullOrEmpty(errorMessage))
								{
									rtn = false;
									errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
									wws.Cell(rowIndex + 1, 10).Value = errorMessage;
									continue;								}
								
								//执行额外的数据校验
								try
								{
									AdditionalCheckExcelData(model);
								}
								catch (Exception ex)
								{
									rtn = false;
									errorMessage = ex.Message;
									errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
									wws.Cell(rowIndex + 1, 10).Value = errorMessage;
									continue;
								}
								
									//写入数据库
									WMS_InvInfo entity = new WMS_InvInfo();
									entity.Id = model.Id;
									entity.InvCode = model.InvCode;
									entity.InvName = model.InvName;
									entity.Remark = model.Remark;
									entity.Status = model.Status;
									entity.CreatePerson = model.CreatePerson;
									entity.CreateTime = model.CreateTime;
									entity.ModifyPerson = model.ModifyPerson;
									entity.ModifyTime = model.ModifyTime;

									db.WMS_InvInfo.Add(entity);
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
										wws.Cell(rowIndex + 1, 10).Value = errorMessage;
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

		public void AdditionalCheckExcelData(WMS_InvInfoModel model)
		{
		}

        public List<WMS_InvInfoModel> GetListByWhere(string where)
        {
            IQueryable<WMS_InvInfo> queryData = null;
            queryData = m_Rep.GetList().Where(where).OrderBy(p => p.InvCode);
            return CreateModelList(ref queryData);
        }
    }
}

