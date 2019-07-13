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
    public  partial class WMS_Feed_ListBLL
    {

        public override List<WMS_Feed_ListModel> CreateModelList(ref IQueryable<WMS_Feed_List> queryData)
        {

            List<WMS_Feed_ListModel> modelList = (from r in queryData
                                              select new WMS_Feed_ListModel
                                              {
                                                  AssemblyPartId = r.AssemblyPartId,
                                                  Attr1 = r.Attr1,
                                                  Attr2 = r.Attr2,
                                                  Attr3 = r.Attr3,
                                                  Attr4 = r.Attr4,
                                                  Attr5 = r.Attr5,
                                                  BoxQty = r.BoxQty,
                                                  Capacity = r.Capacity,
                                                  ConfirmDate = r.ConfirmDate,
                                                  ConfirmMan = r.ConfirmMan,
                                                  ConfirmStatus = r.ConfirmStatus,
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  Department = r.Department,
                                                  FeedBillNum = r.FeedBillNum,
                                                  FeedQty = r.FeedQty,
                                                  Id = r.Id,
                                                  InvId = r.InvId,
                                                  Lot = r.Lot,
                                                  ModifyPerson = r.ModifyPerson,
                                                  ModifyTime = r.ModifyTime,
                                                  PrintDate = r.PrintDate,
                                                  PrintMan = r.PrintMan,
                                                  PrintStaus = r.PrintStaus,
                                                  ReleaseBillNum = r.ReleaseBillNum,
                                                  Remark = r.Remark,
                                                  SubAssemblyPartId = r.SubAssemblyPartId,
                                                  SubInvId = r.SubInvId,
                                                  ConfirmMessage = r.ConfirmMessage,

                                                  AssemblyPartCode = r.WMS_Part.PartCode,
                                                  AssemblyPartName = r.WMS_Part.PartName,
                                                  SubAssemblyPartCode = r.WMS_Part1.PartCode,
                                                  SubAssemblyPartName = r.WMS_Part1.PartName,
                                                  InvCode = r.WMS_InvInfo.InvCode,
                                                  InvName = r.WMS_InvInfo.InvName,
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
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.FeedBillNum, "投料单号（业务）(必输)");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ReleaseBillNum, "投料单号（系统）");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Department, "投料部门");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.AssemblyPartCode, "总成物料");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.SubAssemblyPartCode, "投料物料(必输)");
                    excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Lot, "批次号(格式：YYYY-MM-DD)");
                    excelFile.AddMapping<WMS_Feed_ListModel>(x => x.FeedQty, "投料数量(必输)");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.BoxQty, "箱数");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Capacity, "体积");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.InvName, "库房");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.SubInvId, "子库存");
					excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Remark, "备注");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.PrintStaus, "打印状态");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.PrintDate, "打印时间");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.PrintMan, "打印人");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ConfirmStatus, "确认状态");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ConfirmMan, "确认人");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ConfirmDate, "确认时间");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr1, "");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr2, "");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr3, "");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr4, "");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.Attr5, "");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.CreatePerson, "创建人");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.CreateTime, "创建时间");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ModifyPerson, "修改人");
					//excelFile.AddMapping<WMS_Feed_ListModel>(x => x.ModifyTime, "修改时间");

					//SheetName，第一个Sheet
					var excelContent = excelFile.Worksheet<WMS_Feed_ListModel>(0);

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
								var model = new WMS_Feed_ListModel();
								model.Id = row.Id;
								model.FeedBillNum = row.FeedBillNum;
								//model.ReleaseBillNum = row.ReleaseBillNum;
								model.Department = row.Department;
								model.AssemblyPartCode = row.AssemblyPartCode;
								model.SubAssemblyPartCode = row.SubAssemblyPartCode;
								model.FeedQty = row.FeedQty;
								model.BoxQty = row.BoxQty;
								model.Capacity = row.Capacity;
								model.InvName = row.InvName;
                            model.Lot = row.Lot;
								//model.SubInvId = row.SubInvId;
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
									WMS_Feed_List entity = new WMS_Feed_List();
									entity.Id = model.Id;
									entity.FeedBillNum = model.FeedBillNum;
                            //entity.ReleaseBillNum = model.ReleaseBillNum;
                            //entity.ReleaseBillNum = "TL" + DateTime.Now.ToString("yyyyMMddHHmmssff");打印时生成
                            entity.Department = model.Department;
									entity.AssemblyPartId = model.AssemblyPartId;
									entity.SubAssemblyPartId = model.SubAssemblyPartId;
									entity.FeedQty = model.FeedQty;
									entity.BoxQty = model.BoxQty;
									entity.Capacity = model.Capacity;
									entity.InvId = model.InvId;
                            entity.Lot = model.Lot;
									//entity.SubInvId = model.SubInvId;
									entity.Remark = model.Remark;
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

									db.WMS_Feed_List.Add(entity);
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

		public void AdditionalCheckExcelData(DBContainer db, ref WMS_Feed_ListModel model)
		{
            //获取总成物料ID
            //if (!String.IsNullOrEmpty(model.AssemblyPartCode))
            //{
            //    var partCode = model.AssemblyPartCode;
            //    Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

            //    //var part = m_PartRep.GetSingleWhere(exp);
            //    var part = db.WMS_Part.FirstOrDefault(exp);
            //    if (part == null)
            //    {
            //        throw new Exception("总成物料编码不存在！");
            //    }
            //    else
            //    {
            //        model.AssemblyPartId = part.Id;
            //    }
            //}
            //else
            //{
            //    throw new Exception("总成物料编码不能为空！");
            //}
            //获取物料ID
            if (!String.IsNullOrEmpty(model.SubAssemblyPartCode))
            {
                var partCode = model.SubAssemblyPartCode;
                Expression<Func<WMS_Part, bool>> exp = x => x.PartCode == partCode;

                //var part = m_PartRep.GetSingleWhere(exp);
                var part = db.WMS_Part.FirstOrDefault(exp);
                if (part == null)
                {
                    throw new Exception("投料物料编码不存在！");
                }
                else
                {
                    model.SubAssemblyPartId = part.Id;
                }
            }
            else
            {
                throw new Exception("投料物料编码不能为空！");
            }

            //获取库房ID
            if (!String.IsNullOrEmpty(model.SubAssemblyPartCode))
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
            else
            {
                throw new Exception("库房不能为空！");
            }
           
            //校验批次号,没有批次号自动赋值为当前月
            if (!String.IsNullOrEmpty(model.Lot))
            {
                if (!DateTimeHelper.CheckYearMonth(model.Lot))
                {
                    throw new Exception("批次号不合符规范！");
                }
            }
            //投料单号不能为空
            if (String.IsNullOrEmpty(model.FeedBillNum))
            {
                throw new Exception("投料单号不能为空！");
            }
            //投料数量不能为空
            if (model.FeedQty == 0)
            {
                throw new Exception("投料数量不能为空！");
            }

        }

        public List<WMS_Feed_ListModel> GetListByWhere(ref GridPager pager, string where)
		{
			IQueryable<WMS_Feed_List> queryData = null;
			queryData = m_Rep.GetList().Where(where);
			pager.totalRows = queryData.Count();
			//排序
			queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
			return CreateModelList(ref queryData);
		}

        public string PrintFeedList(ref ValidationErrors errors, string opt, string feedBillNum, int id)
        {
            try
            {
                string releaseBillNum = null;
                var rtn = m_Rep.PrintFeedList(opt, feedBillNum, id, ref releaseBillNum);
                if (!String.IsNullOrEmpty(rtn))
                {
                    errors.Add(rtn);
                    return null;
                }
                else
                {
                    return releaseBillNum;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return null;
            }
        }

        public bool UnPrintFeedList(ref ValidationErrors errors, string opt, string releaseBillNum, int id)
        {
            try
            {
                m_Rep.UnPrintFeedList(opt, releaseBillNum, id);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }

        public bool ConfirmFeedList(ref ValidationErrors errors, string opt, string releaseBillNum)
        {
            try
            {
                m_Rep.ConfirmFeedList(opt, releaseBillNum);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return false;
            }
        }
    }
}

