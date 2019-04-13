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
using System.Linq.Expressions;
using Apps.IDAL.Sys;
using Unity.Attributes;

namespace Apps.BLL.WMS
{
    public  partial class WMS_SupplierBLL
    {
        [Dependency]
        public ISysParamRepository m_SysParamRep { get; set; }

        [Dependency]
        public IDAL.WMS.IWMS_SupplierRepository m_SupplierRep { get; set; }

        public override List<WMS_SupplierModel> CreateModelList(ref IQueryable<WMS_Supplier> queryData)
        {

            List<WMS_SupplierModel> modelList = (from r in queryData
                                              select new WMS_SupplierModel
                                              {
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Id = r.Id,
                                                  LinkMan = r.LinkMan,
                                                  LinkManAddress = r.LinkManAddress,
                                                  LinkManTel = r.LinkManTel,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  Remark = r.Remark,
                                                  Status = r.Status,
                                                  SupplierCode = r.SupplierCode,
                                                  SupplierName = r.SupplierName,
                                                  SupplierShortName = r.SupplierShortName,
                                                  SupplierType = r.SupplierType,
                                                  MoreAccept = r.MoreAccept,
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
					excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierCode, "供应商编码(必输)");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierShortName, "供应商简称(必输)");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierName, "供应商名称(必输)");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.SupplierType, "供应商类型(必输)");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.LinkMan, "联系人");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.LinkManTel, "联系人电话");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.LinkManAddress, "联系人地址");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.MoreAccept, "超量接收(允许/不允许)(必输)");
					excelFile.AddMapping<WMS_SupplierModel>(x => x.Remark, "说明");
					//excelFile.AddMapping<WMS_SupplierModel>(x => x.CreatePerson, "创建人");
					//excelFile.AddMapping<WMS_SupplierModel>(x => x.CreateTime, "创建时间");
					//excelFile.AddMapping<WMS_SupplierModel>(x => x.ModifyPerson, "修改人");
					//excelFile.AddMapping<WMS_SupplierModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_SupplierModel>(0);

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
								var model = new WMS_SupplierModel();
								model.Id = row.Id;
								model.SupplierCode = row.SupplierCode;
								model.SupplierShortName = row.SupplierShortName;
								model.SupplierName = row.SupplierName;
								model.SupplierType = row.SupplierType;
								model.LinkMan = row.LinkMan;
								model.LinkManTel = row.LinkManTel;
								model.LinkManAddress = row.LinkManAddress;
                                model.MoreAccept = row.MoreAccept;
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
									WMS_Supplier entity = new WMS_Supplier();
									entity.Id = model.Id;
									entity.SupplierCode = model.SupplierCode;
									entity.SupplierShortName = model.SupplierShortName;
									entity.SupplierName = model.SupplierName;
									entity.SupplierType = model.SupplierType;
									entity.LinkMan = model.LinkMan;
									entity.LinkManTel = model.LinkManTel;
									entity.LinkManAddress = model.LinkManAddress;
                                    entity.MoreAccept = model.MoreAccept;
                                    entity.MoreAccept = "允许";
									entity.Status = "有效";
									entity.Remark = model.Remark;
									entity.CreatePerson = oper;
									entity.CreateTime = DateTime.Now;
									entity.ModifyPerson = oper;
									entity.ModifyTime = DateTime.Now;

									db.WMS_Supplier.Add(entity);
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

		public void AdditionalCheckExcelData(DBContainer db, ref WMS_SupplierModel model)
		{
            //获取超量接收
            if (!String.IsNullOrEmpty(model.MoreAccept))
            {                
                if (model.MoreAccept!= "允许" && model.MoreAccept != "不允许")
                {
                    throw new Exception("超量接收只能维护允许或者不允许！");
                }

            }
            else
            {
                throw new Exception("超量接收不能为空！");
            }

            //获取供应商编码
            if (!String.IsNullOrEmpty(model.SupplierCode))
            {
                var supplierCode = model.SupplierCode;
                Expression<Func<WMS_Supplier, bool>> exp = x => x.SupplierCode == supplierCode;

                //var supplier = m_SupplierRep.GetSingleWhere(exp);
                var supplier = db.WMS_Supplier.FirstOrDefault(exp);
                if (supplier != null)
                {
                    throw new Exception("供应商编码重复！");
                }

            }
            else
            {
                throw new Exception("供应商编码不能为空！");
            }
            //获取供应商类型
            if (!String.IsNullOrEmpty(model.SupplierType))
            {
                var supplier = model.SupplierType;
                Expression<Func<SysParam, bool>> exp = x => x.ParamName == supplier && x.TypeCode == "SupplierType";

                var part = m_SysParamRep.GetSingleWhere(exp);
                if (part == null)
                {
                    throw new Exception("供应商类型不存在！");
                }
            }
            //供应商名称不能为空
            if (String.IsNullOrEmpty(model.SupplierName))
            {
                throw new Exception("供应商名称不能为空！");
            }
            //供应商简称不能为空
            if (String.IsNullOrEmpty(model.SupplierShortName))
            {
                throw new Exception("供应商简称不能为空！");
            }
            //供应商类型不能为空
            if (String.IsNullOrEmpty(model.SupplierType))
            {
                throw new Exception("供应商类型不能为空！");
            }            
        }

        public List<WMS_SupplierModel> GetListByWhere(ref GridPager pager, string where)
        {
            IQueryable<WMS_Supplier> queryData = null;
            queryData = m_Rep.GetList().Where(where);
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
    }
}

