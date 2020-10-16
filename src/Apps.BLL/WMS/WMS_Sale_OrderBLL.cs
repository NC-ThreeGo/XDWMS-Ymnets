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
using System.Linq.Expressions;

namespace Apps.BLL.WMS
{
    public  partial class WMS_Sale_OrderBLL
    {

        public override List<WMS_Sale_OrderModel> CreateModelList(ref IQueryable<WMS_Sale_Order> queryData)
        {

            List<WMS_Sale_OrderModel> modelList = (from r in queryData
                                                   select new WMS_Sale_OrderModel
                                                   {
                                                       Attr1 = r.Attr1,
                                                       Attr2 = r.Attr2,
                                                       Attr3 = r.Attr3,
                                                       Attr4 = r.Attr4,
                                                       Attr5 = r.Attr5,
                                                       BoxQty = r.BoxQty,
                                                       ConfirmDate = r.ConfirmDate,
                                                       ConfirmMan = r.ConfirmMan,
                                                       ConfirmStatus = r.ConfirmStatus,
                                                       CreatePerson = r.CreatePerson,
                                                       CreateTime = r.CreateTime,
                                                       CustomerId = r.CustomerId,
                                                       Id = r.Id,
                                                       InvId = r.InvId,
                                                       Lot = r.Lot,
                                                       ModifyPerson = r.ModifyPerson,
                                                       ModifyTime = r.ModifyTime,
                                                       PartId = r.PartId,
                                                       PlanDeliveryDate = r.PlanDeliveryDate,
                                                       PrintDate = r.PrintDate,
                                                       PrintMan = r.PrintMan,
                                                       PrintStaus = r.PrintStaus,
                                                       Qty = r.Qty,
                                                       Remark = r.Remark,
                                                       SaleBillNum = r.SaleBillNum,
                                                       SellBillNum = r.SellBillNum,
                                                       SubInvId = r.SubInvId,
                                                       Volume =r.Volume,

                                                       PartCode = r.WMS_Part.PartCode,
                                                       PartName = r.WMS_Part.PartName,                                                       
                                                       InvCode = r.WMS_InvInfo.InvCode,
                                                       InvName = r.WMS_InvInfo.InvName,
                                                       CustomerShortName = r.WMS_Customer.CustomerShortName,
                                                       CustomerName = r.WMS_Customer.CustomerName,
                                                       //BoxVolume = Math.Ceiling(r.BoxQty.Value)*r.WMS_Part.Volume,
                                                       //由于基础信息物料里定义的是3位小数，这里要求是4位小数，就要求导入体积
                                                       BoxVolume = Math.Ceiling(r.BoxQty.Value) * r.Volume,
                                                       ConfirmMessage = r.ConfirmMessage,
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
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.SaleBillNum, "销售单号（业务）(必输)");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.SellBillNum, "销售单号（系统）");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PlanDeliveryDate, "要求到货日期");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.CustomerShortName, "客户名称(必输)");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PartCode, "物料编码(必输)");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Qty, "数量(必输)");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.BoxQty, "箱数");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.InvName, "库房");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.SubInvId, "子库存");
					excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Lot, "批次号(格式：YYYY-MM-DD)");
                    excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Volume, "体积");
                    excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Remark, "备注");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PrintStaus, "打印状态");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PrintDate, "打印日期");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PrintMan, "打印人");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ConfirmStatus, "确认状态");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ConfirmMan, "确认人");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ConfirmDate, "确认时间");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr1, "");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr2, "");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr3, "");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr4, "");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr5, "");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.CreatePerson, "创建人");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.CreateTime, "创建时间");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ModifyPerson, "修改人");
					//excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_Sale_OrderModel>(0);

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
								var model = new WMS_Sale_OrderModel();
								model.Id = row.Id;
								model.SaleBillNum = row.SaleBillNum;
								//model.SellBillNum = row.SellBillNum;
								model.PlanDeliveryDate = row.PlanDeliveryDate;
								model.CustomerShortName = row.CustomerShortName;
								model.PartCode = row.PartCode;
								model.Qty = row.Qty;
								model.BoxQty = row.BoxQty;
								model.InvName = row.InvName;
								//model.SubInvId = row.SubInvId;
								model.Lot = row.Lot;
                            model.Volume = row.Volume;
								model.Remark = row.Remark;
								//model.PrintStaus = row.PrintStaus;
								//model.PrintDate = row.PrintDate;
								//model.PrintMan = row.PrintMan;
								//model.ConfirmStatus = row.ConfirmStatus;
								//model.ConfirmMan = row.ConfirmMan;
								//model.ConfirmDate = row.ConfirmDate;
								//model.Attr1 = row.Attr1;
								//model.Attr2 = row.Attr2;
								//model.Attr3 = row.Attr3;
								//model.Attr4 = row.Attr4;
								//model.Attr5 = row.Attr5;
								//model.CreatePerson = row.CreatePerson;
								//model.CreateTime = row.CreateTime;
								//model.ModifyPerson = row.ModifyPerson;
								//model.ModifyTime = row.ModifyTime;

								if (!String.IsNullOrEmpty(errorMessage))
								{
									rtn = false;
									errors.Add(string.Format("第 {0} 列发现错误：{1}{2}", rowIndex, errorMessage, "<br/>"));
									wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
									continue;								}
								
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
									wws.Cell(rowIndex + 1, excelFile.GetColumnNames("Sheet1").Count()).Value = errorMessage;
									continue;
								}
								
									//写入数据库
									WMS_Sale_Order entity = new WMS_Sale_Order();
									entity.Id = model.Id;
									entity.SaleBillNum = model.SaleBillNum;
                            //entity.SellBillNum = "XS" + DateTime.Now.ToString("yyyyMMddHHmmssff");打印时生成
                            entity.PlanDeliveryDate = model.PlanDeliveryDate;
									entity.CustomerId = model.CustomerId;
									entity.PartId = model.PartId;
									entity.Qty = model.Qty;
									entity.BoxQty = model.BoxQty;
									entity.InvId = model.InvId;
									entity.SubInvId = model.SubInvId;
									entity.Lot = model.Lot;
									entity.Remark = model.Remark;
                                    entity.Volume = model.Volume;
									entity.PrintStaus = "未打印";
									//entity.PrintDate = model.PrintDate;
									//entity.PrintMan = model.PrintMan;
									entity.ConfirmStatus = "未确认";
									//entity.ConfirmMan = model.ConfirmMan;
									//entity.ConfirmDate = model.ConfirmDate;
									//entity.Attr1 = model.Attr1;
									//entity.Attr2 = model.Attr2;
									//entity.Attr3 = model.Attr3;
									//entity.Attr4 = model.Attr4;
									//entity.Attr5 = model.Attr5;
									//entity.CreatePerson = model.CreatePerson;
									//entity.CreateTime = model.CreateTime;
									//entity.ModifyPerson = model.ModifyPerson;
									//entity.ModifyTime = model.ModifyTime;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.WMS_Sale_Order.Add(entity);
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

		public void AdditionalCheckExcelData(DBContainer db, ref WMS_Sale_OrderModel model)
		{
            //获取物料ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var partCode = model.PartCode;
                Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

                //var part = m_PartRep.GetSingleWhere(exp);
                var part = db.WMS_Part.FirstOrDefault(exp);
                if (part == null)
                {
                    throw new Exception("物料编码不存在！");
                }
                else
                {
                    model.PartId = part.Id;
                    int partId = part.Id;
                    if (!String.IsNullOrEmpty(model.SaleBillNum))
                    {
                        var saleBillNum = model.SaleBillNum;
                        Expression<Func<WMS_Sale_Order, bool>> exp1 = x => x.PartId == partId && x.SaleBillNum == saleBillNum;
                        var part1 = db.WMS_Sale_Order.FirstOrDefault(exp1);
                        if (part1 != null)
                        {
                            throw new Exception("销售单号与物料编码重复！");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("物料编码不能为空！");
            }
            //设定箱数的默认值
            //if (model.BoxQty == null) { model.BoxQty = 0; }

            //获取客户ID
            if (!String.IsNullOrEmpty(model.CustomerShortName))
            {
                var customerShortName = model.CustomerShortName;
                Expression<Func<WMS_Customer, bool>> exp = x => x.CustomerName == customerShortName;

                //var part = m_PartRep.GetSingleWhere(exp);
                var customer = db.WMS_Customer.FirstOrDefault(exp);
                if (customer == null)
                {
                    throw new Exception("客户不存在！");
                }
                else
                {
                    model.CustomerId = customer.Id;
                }
            }
            else
            {
                throw new Exception("客户简称不能为空！");
            }

            //获取库房ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var invName = "主仓库";
                Expression<Func<WMS_InvInfo, bool>> exp = x => x.InvName == invName;

                //var supplier = m_SupplierRep.GetSingleWhere(exp);
                var invInfo = db.WMS_InvInfo.FirstOrDefault(exp);
                if (invInfo == null)
                {
                    throw new Exception("库房不存在！");
                }
                else
                {
                    model.InvId = invInfo.Id;
                }
            }
            //else
            //{
            //    throw new Exception("库房不能为空！");
            //}
            //校验批次号,没有批次号自动赋值为当前月
            if (!String.IsNullOrEmpty(model.Lot))
            {
                if (!DateTimeHelper.CheckYearMonth(model.Lot))
                {
                    throw new Exception("批次号不合符规范！");
                }
            }
            //销售单号不能为空
            if (String.IsNullOrEmpty(model.SaleBillNum))
            {
                throw new Exception("销售单号不能为空！");
            }
            //数量不能为空
            if (model.Qty == 0)
            {
                throw new Exception("数量不能为空！");
            }
        }

		public List<WMS_Sale_OrderModel> GetListByWhere(ref GridPager pager, string where)
		{
            IQueryable<WMS_Sale_Order> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}

        public List<WMS_Sale_OrderModel> GetListByWhereAndGroupBy(ref GridPager pager, string where)
        {
            IQueryable<WMS_Sale_Order> queryData = null;
            queryData = m_Rep.GetList().Where(where)
               .GroupBy(p => new { p.SaleBillNum })
               .Select(g => g.FirstOrDefault())
               .OrderBy(p => p.SaleBillNum);

            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public string PrintSaleOrder(ref ValidationErrors errors, string opt, string saleBillNum, int id)
        {
            try
            {
                string sellBillNum = null;
                var rtn = m_Rep.PrintSaleOrder(opt, saleBillNum, id, ref sellBillNum);
                if (!String.IsNullOrEmpty(rtn))
                {
                    errors.Add(rtn);
                    return null;
                }
                else
                {
                    return sellBillNum;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return null;
            }
        }
        public decimal GetSumByWhere(string where, string sumField)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(WMS_Sale_Order), "p");
            var expression = Expression.Lambda<Func<WMS_Sale_Order, decimal>>(Expression.Property(parameter, sumField), parameter);
            try
            {
                decimal total = m_Rep.GetList().Where(where).Sum(expression);
                return total;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public bool UnPrintSaleOrder(ref ValidationErrors errors, string opt, string sellBillNum, int id)
        {
            try
            {
                m_Rep.UnPrintSaleOrder(opt, sellBillNum, id);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }

        public bool ConfirmSaleOrder(ref ValidationErrors errors, string opt, string sellBillNum)
        {
            try
            {
                var rtn = m_Rep.ConfirmSaleOrder(opt, sellBillNum);
                if (String.IsNullOrEmpty(rtn))
                {
                    return true;
                }
                else
                {
                    errors.Add(rtn);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }
    }
}

