using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using LinqToExcel;
using ClosedXML.Excel;
using Apps.Models.WMS;

namespace Apps.BLL.WMS
{
    public  partial class WMS_CustomerBLL
    {

        public override List<WMS_CustomerModel> CreateModelList(ref IQueryable<WMS_Customer> queryData)
        {

            List<WMS_CustomerModel> modelList = (from r in queryData
                                              select new WMS_CustomerModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  CustomerCode = r.CustomerCode,
                                                  CustomerName = r.CustomerName,
                                                  CustomerShortName = r.CustomerShortName,
                                                  CustomerType = r.CustomerType,
                                                  Id = r.Id,
                                                  LinkMan = r.LinkMan,
                                                  LinkManAddress = r.LinkManAddress,
                                                  LinkManTel = r.LinkManTel,
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
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerCode, "客户编码");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerShortName, "客户简称");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerName, "客户名称");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerType, "客户类型");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.LinkMan, "联系人");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.LinkManTel, "联系电话");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.LinkManAddress, "联系地址");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.Status, "状态");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.Remark, "说明");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CreatePerson, "创建人");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CreateTime, "创建时间");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.ModifyPerson, "修改人");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_CustomerModel>(0);

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
								var model = new WMS_CustomerModel();
								model.Id = row.Id;
								model.CustomerCode = row.CustomerCode;
								model.CustomerShortName = row.CustomerShortName;
								model.CustomerName = row.CustomerName;
								model.CustomerType = row.CustomerType;
								model.LinkMan = row.LinkMan;
								model.LinkManTel = row.LinkManTel;
								model.LinkManAddress = row.LinkManAddress;
								model.Status = row.Status;
								model.Remark = row.Remark;
								model.CreatePerson = row.CreatePerson;
								model.CreateTime = row.CreateTime;
								model.ModifyPerson = row.ModifyPerson;
								model.ModifyTime = row.ModifyTime;

								if (!String.IsNullOrEmpty(errorMessage))
								{
									rtn = false;
									errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
									wws.Cell(rowIndex + 1, 15).Value = errorMessage;
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
									wws.Cell(rowIndex + 1, 15).Value = errorMessage;
									continue;
								}
								
									//写入数据库
									WMS_Customer entity = new WMS_Customer();
									entity.Id = model.Id;
									entity.CustomerCode = model.CustomerCode;
									entity.CustomerShortName = model.CustomerShortName;
									entity.CustomerName = model.CustomerName;
									entity.CustomerType = model.CustomerType;
									entity.LinkMan = model.LinkMan;
									entity.LinkManTel = model.LinkManTel;
									entity.LinkManAddress = model.LinkManAddress;
									entity.Status = model.Status;
									entity.Remark = model.Remark;
									entity.CreatePerson = model.CreatePerson;
									entity.CreateTime = model.CreateTime;
									entity.ModifyPerson = model.ModifyPerson;
									entity.ModifyTime = model.ModifyTime;

									db.WMS_Customer.Add(entity);
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
										wws.Cell(rowIndex + 1, 15).Value = errorMessage;
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

		public void AdditionalCheckExcelData(WMS_CustomerModel model)
		{
		}
    }
 }

