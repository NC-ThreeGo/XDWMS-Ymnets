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
using System.Linq.Expressions;
using Apps.IDAL.Sys;
using Unity.Attributes;
using System.Linq.Dynamic.Core;
using System.Data.Entity.Infrastructure;

namespace Apps.BLL.WMS
{
    public  partial class WMS_CustomerBLL
    {

        [Dependency]
        public ISysParamRepository m_SysParamRep { get; set; }

        [Dependency]
        public IDAL.WMS.IWMS_CustomerRepository m_CustomerRep { get; set; }

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
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerCode, "客户编码(必输)");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerShortName, "客户简称(必输)");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerName, "客户名称(必输)");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.CustomerType, "客户类型");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.LinkMan, "联系人");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.LinkManTel, "联系电话");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.LinkManAddress, "联系地址");
					//excelFile.AddMapping<WMS_CustomerModel>(x => x.Status, "状态");
					excelFile.AddMapping<WMS_CustomerModel>(x => x.Remark, "说明");
					//excelFile.AddMapping<WMS_CustomerModel>(x => x.CreatePerson, "创建人");
					//excelFile.AddMapping<WMS_CustomerModel>(x => x.CreateTime, "创建时间");
					//excelFile.AddMapping<WMS_CustomerModel>(x => x.ModifyPerson, "修改人");
					//excelFile.AddMapping<WMS_CustomerModel>(x => x.ModifyTime, "修改时间");

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
                               if (row.CustomerCode != null)
                               { model.CustomerCode = row.CustomerCode.Replace(" ", ""); }
                               if (row.CustomerShortName != null)
                               { model.CustomerShortName = row.CustomerShortName.Replace(" ", ""); }
                               if (row.CustomerName != null)
                               { model.CustomerName = row.CustomerName.Replace(" ", ""); }
								model.CustomerType = row.CustomerType;
								model.LinkMan = row.LinkMan;
								model.LinkManTel = row.LinkManTel;
								model.LinkManAddress = row.LinkManAddress;
								//model.Status = row.Status;
								model.Remark = row.Remark;
								//model.CreatePerson = row.CreatePerson;
								//model.CreateTime = row.CreateTime;
								//model.ModifyPerson = row.ModifyPerson;
								//model.ModifyTime = row.ModifyTime;

								if (!String.IsNullOrEmpty(errorMessage))
								{
                                    rtn = false;
                                    errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                                    wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
                                    continue;
                                 //rtn = false;
                                //errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                                //wws.Cell(rowIndex + 1, 15).Value = errorMessage;
                                //continue;								
                                }
								
								//执行额外的数据校验
								try
								{
									AdditionalCheckExcelData(db, ref model);
								}
								catch (Exception ex)
								{
									rtn = false;
									errorMessage = ex.Message;
									errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
                                    //wws.Cell(rowIndex + 1, 15).Value = errorMessage;
                                    wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
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
									entity.Status = "有效";
									entity.Remark = model.Remark;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

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
                                        //wws.Cell(rowIndex + 1, 15).Value = errorMessage;
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

		public void AdditionalCheckExcelData(DBContainer db, ref WMS_CustomerModel model)
		{
            //获取客户编码
            if (!String.IsNullOrEmpty(model.CustomerCode))
            {
                var customerCode = model.CustomerCode;
                Expression<Func<WMS_Customer, bool>> exp = x => x.CustomerCode == customerCode;

                //var customer = m_CustomerRep.GetSingleWhere(exp);
                var customer = db.WMS_Customer.FirstOrDefault(exp);
                if (customer != null)
                {
                    throw new Exception("客户编码重复！");
                }

            }
            else
            {
                throw new Exception("客户编码不能为空！");
            }
            //获取客户类型
            if (!String.IsNullOrEmpty(model.CustomerType))
            {
                var customer = model.CustomerType;
                Expression<Func<SysParam, bool>> exp = x => x.ParamName == customer && x.TypeCode == "CustomerType";

                var part = m_SysParamRep.GetSingleWhere(exp);
                if (part == null)
                {
                    throw new Exception("客户类型不存在！");
                }
            }
            ////客户类型不能为空
            //if (String.IsNullOrEmpty(model.CustomerType))
            //{
            //    throw new Exception("客户类型不能为空！");
            //}
            //客户名称不能为空
            if (String.IsNullOrEmpty(model.CustomerName))
            {
                throw new Exception("客户名称不能为空！");
            }
            //客户简称不能为空
            if (String.IsNullOrEmpty(model.CustomerShortName))
            {
                throw new Exception("客户简称不能为空！");
            }
        }

        public List<WMS_CustomerModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_Customer> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public List<WMS_CustomerModel> GetListByBelong(ref GridPager pager, string codes)
        {
            using (DBContainer db = new DBContainer())
            {
                DbRawSqlQuery<WMS_CustomerModel> query = db.Database.SqlQuery<WMS_CustomerModel>(@"SELECT  * from WMS_Customer where '" + codes + "' like '%;' + CustomerCode + ';%'");

                //启用通用列头过滤
                pager.totalRows = query.Count();

                try
                {
                    //排序
                    IQueryable<WMS_CustomerModel> queryData = LinqHelper.SortingAndPaging(query.AsQueryable(), pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

