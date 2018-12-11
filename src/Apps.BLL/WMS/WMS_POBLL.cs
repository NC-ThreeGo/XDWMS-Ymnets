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
using Unity.Attributes;
using Apps.IDAL.WMS;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;


namespace Apps.BLL.WMS
{
    public  partial class WMS_POBLL
    {
        [Dependency]
        public IWMS_PartRepository m_PartRep { get; set; }
        [Dependency]
        public IWMS_SupplierRepository m_SupplierRep { get; set; }
        [Dependency]
        public IWMS_PORepository m_PORep { get; set; }


        public override List<WMS_POModel> CreateModelList(ref IQueryable<WMS_PO> queryData)
        {

            List<WMS_POModel> modelList = (from r in queryData
                                              select new WMS_POModel
                                              {
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
                                                  PartId = r.PartId,
                                                  PlanDate = r.PlanDate,
                                                  PO = r.PO,
                                                  PODate = r.PODate,
                                                  POType = r.POType,
                                                  QTY = r.QTY,
                                                  Remark = r.Remark,
                                                  Status = r.Status,
                                                  SupplierId = r.SupplierId,
                                                  PartCode = r.WMS_Part.PartCode,
                                                  SupplierShortName = r.WMS_Supplier.SupplierShortName,
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
					excelFile.AddMapping<WMS_POModel>(x => x.PO, "采购订单");
					excelFile.AddMapping<WMS_POModel>(x => x.PODate, "采购日期");
					excelFile.AddMapping<WMS_POModel>(x => x.SupplierShortName, "供应商简称");
					excelFile.AddMapping<WMS_POModel>(x => x.PartCode, "物料编码");
					excelFile.AddMapping<WMS_POModel>(x => x.QTY, "数量");
					excelFile.AddMapping<WMS_POModel>(x => x.PlanDate, "计划到货日期");
					excelFile.AddMapping<WMS_POModel>(x => x.POType, "采购订单类型");
					//excelFile.AddMapping<WMS_POModel>(x => x.Status, "状态");
					excelFile.AddMapping<WMS_POModel>(x => x.Remark, "说明");
					//excelFile.AddMapping<WMS_POModel>(x => x.Attr1, "");
					//excelFile.AddMapping<WMS_POModel>(x => x.Attr2, "");
					//excelFile.AddMapping<WMS_POModel>(x => x.Attr3, "");
					//excelFile.AddMapping<WMS_POModel>(x => x.Attr4, "");
					//excelFile.AddMapping<WMS_POModel>(x => x.Attr5, "");
					//excelFile.AddMapping<WMS_POModel>(x => x.CreatePerson, "创建人");
					//excelFile.AddMapping<WMS_POModel>(x => x.CreateTime, "创建时间");
					//excelFile.AddMapping<WMS_POModel>(x => x.ModifyPerson, "修改人");
					//excelFile.AddMapping<WMS_POModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_POModel>(0);

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
								var model = new WMS_POModel();
								model.Id = row.Id;
								model.PO = row.PO;
								model.PODate = row.PODate;
								model.SupplierShortName = row.SupplierShortName;
								model.PartCode = row.PartCode;
								model.QTY = row.QTY;
								model.PlanDate = row.PlanDate;
								model.POType = row.POType;
								model.Status = "有效";
								model.Remark = row.Remark;
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
									WMS_PO entity = new WMS_PO();
									entity.Id = model.Id;
									entity.PO = model.PO;
									entity.PODate = model.PODate;
									entity.SupplierId = model.SupplierId;
									entity.PartId = model.PartId;
									entity.QTY = model.QTY;
									entity.PlanDate = model.PlanDate;
									entity.POType = model.POType;
									entity.Status = "有效";
									entity.Remark = model.Remark;
									entity.Attr1 = model.Attr1;
									entity.Attr2 = model.Attr2;
									entity.Attr3 = model.Attr3;
									entity.Attr4 = model.Attr4;
									entity.Attr5 = model.Attr5;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.WMS_PO.Add(entity);
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

		public void AdditionalCheckExcelData(ref WMS_POModel model)
		{
            //获取物料ID
            if (!String.IsNullOrEmpty(model.PartCode))
            {
                var partCode = model.PartCode;
                Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

                var part = m_PartRep.GetSingleWhere(exp);
                if (part == null)
                {
                    throw new Exception("物料编码不存在！");
                }
                else
                {
                    model.PartId = part.Id;
                }
            }
            else
            {
                throw new Exception("物料编码不能为空！");
            }

            //获取代理商ID
            if (!String.IsNullOrEmpty(model.SupplierShortName))
            {
                var supplierShortName = model.SupplierShortName;
                Expression<Func<WMS_Supplier, bool>> exp = x => x.SupplierShortName == supplierShortName;
                                
                var supplier = m_SupplierRep.GetSingleWhere(exp);
                if (supplier == null)
                {
                    throw new Exception("供应商简称不存在！");
                }
                else
                {
                    model.SupplierId = supplier.Id;
                }                
            }
            else
            {
                throw new Exception("供应商简称不能为空！");
            }

            //校验订单号与物料
            if (!String.IsNullOrEmpty(model.PO))
            {
                var partId = model.PartId;
                var po = model.PO;
                Expression<Func<WMS_PO, bool>> exp = x => x.PartId == partId && x.PO == po;

                var part = m_PORep.GetSingleWhere(exp);
                if (part != null)
                {
                    throw new Exception("订单号与物料编码重复！");
                }               
            }
            else
            {
                throw new Exception("订单号不能为空！");
            }
            //校验订单号与供应商
            if (!String.IsNullOrEmpty(model.PO))
            {
                var supplierId = model.SupplierId;
                var po = model.PO;
                Expression<Func<WMS_PO, bool>> exp = x => x.PO == po;

                var result = m_PORep.GetSingleWhere(exp);
                if (result != null && supplierId!= result.SupplierId)
                {
                    throw new Exception("同订单存在不同供应商！");
                }
            }
            

        }
        public List<WMS_POModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_PO> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
    }
 }

