//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Apps.Models;
using Apps.Common;
using Unity.Attributes;
using System.Transactions;
using Apps.BLL.Core;
using Apps.Locale;
using LinqToExcel;
using System.IO;
using System.Text;
using Apps.IDAL.WMS;
using Apps.Models.WMS;
using Apps.IBLL.WMS;
namespace Apps.BLL.WMS
{
	public partial class WMS_Sale_OrderBLL: Virtual_WMS_Sale_OrderBLL,IWMS_Sale_OrderBLL
	{
        

	}
	public class Virtual_WMS_Sale_OrderBLL
	{
        [Dependency]
        public IWMS_Sale_OrderRepository m_Rep { get; set; }

		public virtual List<WMS_Sale_OrderModel> GetList(ref GridPager pager, string queryStr)
        {

            IQueryable<WMS_Sale_Order> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
								
								a=>a.SaleBillNum.Contains(queryStr)
								|| a.SellBillNum.Contains(queryStr)
								
								
								
								
								
								
								
								|| a.Lot.Contains(queryStr)
								|| a.Remark.Contains(queryStr)
								|| a.PrintStaus.Contains(queryStr)
								
								|| a.PrintMan.Contains(queryStr)
								|| a.ConfirmStatus.Contains(queryStr)
								|| a.ConfirmMan.Contains(queryStr)
								
								|| a.Attr1.Contains(queryStr)
								|| a.Attr2.Contains(queryStr)
								|| a.Attr3.Contains(queryStr)
								|| a.Attr4.Contains(queryStr)
								|| a.Attr5.Contains(queryStr)
								|| a.CreatePerson.Contains(queryStr)
								
								|| a.ModifyPerson.Contains(queryStr)
								
								);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

		public virtual List<WMS_Sale_OrderModel> GetListByUserId(ref GridPager pager, string userId,string queryStr)
		{
			return new List<WMS_Sale_OrderModel>();
		}
		
		public virtual List<WMS_Sale_OrderModel> GetListByParentId(ref GridPager pager, string queryStr,object parentId)
        {
			return new List<WMS_Sale_OrderModel>();
		}

        public virtual List<WMS_Sale_OrderModel> CreateModelList(ref IQueryable<WMS_Sale_Order> queryData)
        {

            List<WMS_Sale_OrderModel> modelList = (from r in queryData
                                              select new WMS_Sale_OrderModel
                                              {
													Id = r.Id,
													SaleBillNum = r.SaleBillNum,
													SellBillNum = r.SellBillNum,
													PlanDeliveryDate = r.PlanDeliveryDate,
													CustomerId = r.CustomerId,
													PartId = r.PartId,
													Qty = r.Qty,
													BoxQty = r.BoxQty,
													InvId = r.InvId,
													SubInvId = r.SubInvId,
													Lot = r.Lot,
													Remark = r.Remark,
													PrintStaus = r.PrintStaus,
													PrintDate = r.PrintDate,
													PrintMan = r.PrintMan,
													ConfirmStatus = r.ConfirmStatus,
													ConfirmMan = r.ConfirmMan,
													ConfirmDate = r.ConfirmDate,
													Attr1 = r.Attr1,
													Attr2 = r.Attr2,
													Attr3 = r.Attr3,
													Attr4 = r.Attr4,
													Attr5 = r.Attr5,
													CreatePerson = r.CreatePerson,
													CreateTime = r.CreateTime,
													ModifyPerson = r.ModifyPerson,
													ModifyTime = r.ModifyTime,
          
                                              }).ToList();

            return modelList;
        }

        public virtual bool Create(ref ValidationErrors errors, WMS_Sale_OrderModel model)
        {
            try
            {
                WMS_Sale_Order entity = m_Rep.GetById(model.Id);
                if (entity != null)
                {
                    errors.Add(Resource.PrimaryRepeat);
                    return false;
                }
                entity = new WMS_Sale_Order();
               				entity.Id = model.Id;
				entity.SaleBillNum = model.SaleBillNum;
				entity.SellBillNum = model.SellBillNum;
				entity.PlanDeliveryDate = model.PlanDeliveryDate;
				entity.CustomerId = model.CustomerId;
				entity.PartId = model.PartId;
				entity.Qty = model.Qty;
				entity.BoxQty = model.BoxQty;
				entity.InvId = model.InvId;
				entity.SubInvId = model.SubInvId;
				entity.Lot = model.Lot;
				entity.Remark = model.Remark;
				entity.PrintStaus = model.PrintStaus;
				entity.PrintDate = model.PrintDate;
				entity.PrintMan = model.PrintMan;
				entity.ConfirmStatus = model.ConfirmStatus;
				entity.ConfirmMan = model.ConfirmMan;
				entity.ConfirmDate = model.ConfirmDate;
				entity.Attr1 = model.Attr1;
				entity.Attr2 = model.Attr2;
				entity.Attr3 = model.Attr3;
				entity.Attr4 = model.Attr4;
				entity.Attr5 = model.Attr5;
				entity.CreatePerson = model.CreatePerson;
				entity.CreateTime = model.CreateTime;
				entity.ModifyPerson = model.ModifyPerson;
				entity.ModifyTime = model.ModifyTime;
  

                if (m_Rep.Create(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.InsertFail);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }



         public virtual bool Delete(ref ValidationErrors errors, object id)
        {
            try
            {
                if (m_Rep.Delete(id) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }

        public virtual bool Delete(ref ValidationErrors errors, object[] deleteCollection)
        {
            try
            {
                if (deleteCollection != null)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        if (m_Rep.Delete(deleteCollection) == deleteCollection.Length)
                        {
                            transactionScope.Complete();
                            return true;
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                            return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }

		
       

        public virtual bool Edit(ref ValidationErrors errors, WMS_Sale_OrderModel model)
        {
            try
            {
                WMS_Sale_Order entity = m_Rep.GetById(model.Id);
                if (entity == null)
                {
                    errors.Add(Resource.Disable);
                    return false;
                }
                              				entity.Id = model.Id;
				entity.SaleBillNum = model.SaleBillNum;
				entity.SellBillNum = model.SellBillNum;
				entity.PlanDeliveryDate = model.PlanDeliveryDate;
				entity.CustomerId = model.CustomerId;
				entity.PartId = model.PartId;
				entity.Qty = model.Qty;
				entity.BoxQty = model.BoxQty;
				entity.InvId = model.InvId;
				entity.SubInvId = model.SubInvId;
				entity.Lot = model.Lot;
				entity.Remark = model.Remark;
				entity.PrintStaus = model.PrintStaus;
				entity.PrintDate = model.PrintDate;
				entity.PrintMan = model.PrintMan;
				entity.ConfirmStatus = model.ConfirmStatus;
				entity.ConfirmMan = model.ConfirmMan;
				entity.ConfirmDate = model.ConfirmDate;
				entity.Attr1 = model.Attr1;
				entity.Attr2 = model.Attr2;
				entity.Attr3 = model.Attr3;
				entity.Attr4 = model.Attr4;
				entity.Attr5 = model.Attr5;
				entity.CreatePerson = model.CreatePerson;
				entity.CreateTime = model.CreateTime;
				entity.ModifyPerson = model.ModifyPerson;
				entity.ModifyTime = model.ModifyTime;
 


                if (m_Rep.Edit(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.NoDataChange);
                    return false;
                }

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }

      

        public virtual WMS_Sale_OrderModel GetById(object id)
        {
            if (IsExists(id))
            {
                WMS_Sale_Order entity = m_Rep.GetById(id);
                WMS_Sale_OrderModel model = new WMS_Sale_OrderModel();
                              				model.Id = entity.Id;
				model.SaleBillNum = entity.SaleBillNum;
				model.SellBillNum = entity.SellBillNum;
				model.PlanDeliveryDate = entity.PlanDeliveryDate;
				model.CustomerId = entity.CustomerId;
				model.PartId = entity.PartId;
				model.Qty = entity.Qty;
				model.BoxQty = entity.BoxQty;
				model.InvId = entity.InvId;
				model.SubInvId = entity.SubInvId;
				model.Lot = entity.Lot;
				model.Remark = entity.Remark;
				model.PrintStaus = entity.PrintStaus;
				model.PrintDate = entity.PrintDate;
				model.PrintMan = entity.PrintMan;
				model.ConfirmStatus = entity.ConfirmStatus;
				model.ConfirmMan = entity.ConfirmMan;
				model.ConfirmDate = entity.ConfirmDate;
				model.Attr1 = entity.Attr1;
				model.Attr2 = entity.Attr2;
				model.Attr3 = entity.Attr3;
				model.Attr4 = entity.Attr4;
				model.Attr5 = entity.Attr5;
				model.CreatePerson = entity.CreatePerson;
				model.CreateTime = entity.CreateTime;
				model.ModifyPerson = entity.ModifyPerson;
				model.ModifyTime = entity.ModifyTime;
 
                return model;
            }
            else
            {
                return null;
            }
        }


		 /// <summary>
        /// 校验Excel数据,这个方法一般用于重写校验逻辑
        /// </summary>
        public virtual bool CheckImportData(string fileName, List<WMS_Sale_OrderModel> list,ref ValidationErrors errors )
        {
          
            var targetFile = new FileInfo(fileName);

            if (!targetFile.Exists)
            {

                errors.Add("导入的数据文件不存在");
                return false;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            //对应列头
			 				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.SaleBillNum, "销售单号（业务）");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.SellBillNum, "销售单号（系统）");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PlanDeliveryDate, "计划发货日期");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.CustomerId, "客户");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PartId, "PartId");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Qty, "数量");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.BoxQty, "箱数");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.InvId, "库存");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.SubInvId, "子库存");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Lot, "批次号：YYYYMM");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Remark, "备注");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PrintStaus, "打印状态");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PrintDate, "打印日期");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.PrintMan, "打印人");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ConfirmStatus, "确认状态");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ConfirmMan, "确认人");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ConfirmDate, "确认时间");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr1, "Attr1");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr2, "Attr2");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr3, "Attr3");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr4, "Attr4");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.Attr5, "Attr5");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.CreatePerson, "创建人");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.CreateTime, "创建时间");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ModifyPerson, "修改人");
				 excelFile.AddMapping<WMS_Sale_OrderModel>(x => x.ModifyTime, "修改时间");
 
            //SheetName
            var excelContent = excelFile.Worksheet<WMS_Sale_OrderModel>(0);
            int rowIndex = 1;
            //检查数据正确性
            foreach (var row in excelContent)
            {
                var errorMessage = new StringBuilder();
                var entity = new WMS_Sale_OrderModel();
						 				  entity.Id = row.Id;
				  entity.SaleBillNum = row.SaleBillNum;
				  entity.SellBillNum = row.SellBillNum;
				  entity.PlanDeliveryDate = row.PlanDeliveryDate;
				  entity.CustomerId = row.CustomerId;
				  entity.PartId = row.PartId;
				  entity.Qty = row.Qty;
				  entity.BoxQty = row.BoxQty;
				  entity.InvId = row.InvId;
				  entity.SubInvId = row.SubInvId;
				  entity.Lot = row.Lot;
				  entity.Remark = row.Remark;
				  entity.PrintStaus = row.PrintStaus;
				  entity.PrintDate = row.PrintDate;
				  entity.PrintMan = row.PrintMan;
				  entity.ConfirmStatus = row.ConfirmStatus;
				  entity.ConfirmMan = row.ConfirmMan;
				  entity.ConfirmDate = row.ConfirmDate;
				  entity.Attr1 = row.Attr1;
				  entity.Attr2 = row.Attr2;
				  entity.Attr3 = row.Attr3;
				  entity.Attr4 = row.Attr4;
				  entity.Attr5 = row.Attr5;
				  entity.CreatePerson = row.CreatePerson;
				  entity.CreateTime = row.CreateTime;
				  entity.ModifyPerson = row.ModifyPerson;
				  entity.ModifyTime = row.ModifyTime;
 
                //=============================================================================
                if (errorMessage.Length > 0)
                {
                    errors.Add(string.Format(
                        "第 {0} 列发现错误：{1}{2}",
                        rowIndex,
                        errorMessage,
                        "<br/>"));
                }
                list.Add(entity);
                rowIndex += 1;
            }
            if (errors.Count > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public virtual void SaveImportData(IEnumerable<WMS_Sale_OrderModel> list)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    foreach (var model in list)
                    {
                        WMS_Sale_Order entity = new WMS_Sale_Order();
                       						entity.Id = 0;
						entity.SaleBillNum = model.SaleBillNum;
						entity.SellBillNum = model.SellBillNum;
						entity.PlanDeliveryDate = model.PlanDeliveryDate;
						entity.CustomerId = model.CustomerId;
						entity.PartId = model.PartId;
						entity.Qty = model.Qty;
						entity.BoxQty = model.BoxQty;
						entity.InvId = model.InvId;
						entity.SubInvId = model.SubInvId;
						entity.Lot = model.Lot;
						entity.Remark = model.Remark;
						entity.PrintStaus = model.PrintStaus;
						entity.PrintDate = model.PrintDate;
						entity.PrintMan = model.PrintMan;
						entity.ConfirmStatus = model.ConfirmStatus;
						entity.ConfirmMan = model.ConfirmMan;
						entity.ConfirmDate = model.ConfirmDate;
						entity.Attr1 = model.Attr1;
						entity.Attr2 = model.Attr2;
						entity.Attr3 = model.Attr3;
						entity.Attr4 = model.Attr4;
						entity.Attr5 = model.Attr5;
						entity.CreatePerson = model.CreatePerson;
						entity.CreateTime = ResultHelper.NowTime;
						entity.ModifyPerson = model.ModifyPerson;
						entity.ModifyTime = model.ModifyTime;
 
                        db.WMS_Sale_Order.Add(entity);
                    }
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
		public virtual bool Check(ref ValidationErrors errors, object id,int flag)
        {
			return true;
		}

        public virtual bool IsExists(object id)
        {
            return m_Rep.IsExist(id);
        }
		
		public void Dispose()
        { 
            
        }

	}
}
