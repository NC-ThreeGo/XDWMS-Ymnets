using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using Apps.Models.Spl;
using System;
using System.Data.Entity;
using Unity.Attributes;
using Apps.IDAL.Sys;
using System.Data.SqlClient;
using Apps.BLL.Core;
using Apps.Locale;

namespace Apps.BLL.Spl
{
    public partial class Spl_WarehouseAllocationBLL
    {
        [Dependency]
        public ISysUserRepository userBLL { get; set; }


        public List<Spl_WarehouseAllocationModel> GetList(ref GridPager pager, string queryStr,string sysUserId)
        {
            List<string> houseList = userBLL.GetHouseList(sysUserId);
            IQueryable<Spl_WarehouseAllocation> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                                a => (a.Id.Contains(queryStr)

                                || a.Handler.Contains(queryStr)
                                || a.Remark.Contains(queryStr)


                                || a.Checker.Contains(queryStr)


                                || a.CreatePerson.Contains(queryStr)

                                || a.ModifyPerson.Contains(queryStr)

                                || a.FromWarehouseId.Contains(queryStr)
                                || a.ToWarehouseId.Contains(queryStr)
                                || a.ContractNumber.Contains(queryStr))
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
        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData(IEnumerable<Spl_WarehouseAllocationDetailsModel> personList)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    foreach (var model in personList)
                    {
                        Spl_WarehouseAllocationDetails entity = new Spl_WarehouseAllocationDetails();
                        entity.Id = model.Id;
                        entity.WareDetailsId = model.WareDetailsId;
                        entity.WarehouseId = model.WarehouseId;
                        entity.WarehouseAllocationId = model.WarehouseAllocationId;
                        entity.Quantity = model.Quantity;
                        entity.Price = model.Price;
                        entity.TotalPrice = model.TotalPrice;
                        entity.Defined = model.Defined;
                        entity.CreateTime = model.CreateTime;
                        //新增
                        if (string.IsNullOrEmpty(model.Id))
                        {
                            entity.Id = ResultHelper.NewId;
                            db.Spl_WarehouseAllocationDetails.Add(entity);
                        }
                        else
                        {
                            //修改
                            db.Set<Spl_WarehouseAllocationDetails>().Attach(entity);
                            db.Entry<Spl_WarehouseAllocationDetails>(entity).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void SaveEditData(IEnumerable<Spl_WarehouseAllocationDetailsModel> personList, string WarehouseAllocationId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {

                    int count = db.Database.ExecuteSqlCommand("delete Spl_WarehouseAllocationDetails where WarehouseAllocationId=@WarehouseAllocationId", new SqlParameter("@WarehouseAllocationId", WarehouseAllocationId));
                    foreach (var model in personList)
                    {
                        Spl_WarehouseAllocationDetails entity = new Spl_WarehouseAllocationDetails();
                        entity.Id = model.Id;
                        entity.WareDetailsId = model.WareDetailsId;
                        entity.WarehouseId = model.WarehouseId;
                        entity.WarehouseAllocationId = model.WarehouseAllocationId;
                        entity.Quantity = model.Quantity;
                        entity.Price = model.Price;
                        entity.TotalPrice = model.TotalPrice;
                        entity.Defined = model.Defined;
                        entity.CreateTime = model.CreateTime;
                        entity.Id = ResultHelper.NewId;
                        db.Spl_WarehouseAllocationDetails.Add(entity);

                    }

                    db.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public override List<Spl_WarehouseAllocationModel> CreateModelList(ref IQueryable<Spl_WarehouseAllocation> queryData)
        {

            List<Spl_WarehouseAllocationModel> modelList = (from r in queryData
                                                         select new Spl_WarehouseAllocationModel
                                                         {
                                                             Id = r.Id,
                                                             InTime = r.InTime,
                                                             Handler = r.Handler,
                                                             Remark = r.Remark,
                                                             PriceTotal = r.PriceTotal,
                                                             State = r.State,
                                                             Checker = r.Checker,
                                                             CheckerName=r.SysUser.TrueName,
                                                             CheckTime = r.CheckTime,
                                                             CreateTime = r.CreateTime,
                                                             CreatePerson = r.CreatePerson,
                                                             ModifyTime = r.ModifyTime,
                                                             ModifyPerson = r.ModifyPerson,
                                                             Confirmation = r.Confirmation,
                                                         
                                                             ContractNumber = r.ContractNumber,
                                                             FromWarehouseId = r.FromWarehouseId,
                                                             ToWarehouseId = r.ToWarehouseId,
                                                             FromWarehouseName = r.Spl_Warehouse.Name,
                                                             ToWarehouseName = r.Spl_Warehouse1.Name
        }).ToList();
            return modelList;
        }

        public override Spl_WarehouseAllocationModel GetById(object id)
        {
            if (IsExists(id))
            {
                Spl_WarehouseAllocation entity = m_Rep.GetById(id);
                Spl_WarehouseAllocationModel model = new Spl_WarehouseAllocationModel();
                model.Id = entity.Id;
                model.InTime = entity.InTime;
                model.Handler = entity.Handler;
                model.Remark = entity.Remark;
                model.PriceTotal = entity.PriceTotal;
                model.State = entity.State;
                model.Checker = entity.Checker;
                model.CheckerName = entity.SysUser.TrueName;
                model.CheckTime = entity.CheckTime;
                model.CreateTime = entity.CreateTime;
                model.CreatePerson = entity.CreatePerson;
                model.ModifyTime = entity.ModifyTime;
                model.ModifyPerson = entity.ModifyPerson;
                model.Confirmation = entity.Confirmation;
                model.FromWarehouseId = entity.FromWarehouseId;
                model.ToWarehouseId = entity.ToWarehouseId;
                model.FromWarehouseName = entity.Spl_Warehouse.Name;
                model.ToWarehouseName = entity.Spl_Warehouse1.Name;
                model.ContractNumber = entity.ContractNumber;
                return model;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="errors">持久的错误信息</param>
        /// <param name="id">主键</param>
        /// <param name="checkFlag">1审核0反审核</param>
        /// <param name="checker">审核人</param>
        /// <returns>是否成功</returns>
        public bool Check(ref ValidationErrors errors, string id, int checkFlag, string checker)
        {
            try
            {
                Spl_WarehouseAllocation entity = m_Rep.GetById(id);
                if (entity.State == 1)
                {
                    errors.Add("单据已经审核");
                    return false;
                }
                if (entity.Confirmation == false)
                {
                    errors.Add("单据未经确认不能审核");
                    return false;
                }
                if (entity == null)
                {
                    errors.Add(Resource.Disable);
                    return false;
                }
                entity.State = checkFlag;
                entity.Checker = checker;
                entity.CheckTime = DateTime.Now;

                if (m_Rep.Edit(entity))
                {
                    //更新库存表
                    m_Rep.UpdateWareStockPileAllocation(entity.Id);
                    return true;
                }
                else
                {
                    errors.Add(Resource.CheckFail);
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

        ///修改
        ///
        public override bool Edit(ref ValidationErrors errors, Spl_WarehouseAllocationModel model)
        {
            try
            {
                Spl_WarehouseAllocation entity = m_Rep.GetById(model.Id);
                if (entity.Confirmation == true)
                {
                    errors.Add("单据已确认不能被修改");
                    return false;
                }//修改于2018年2月5日
                if (entity == null)
                {
                    errors.Add(Resource.Disable);
                    return false;
                }
                entity.Id = model.Id;
                entity.InTime = model.InTime;
                entity.Handler = model.Handler;
                entity.Remark = model.Remark;
                entity.PriceTotal = model.PriceTotal;
                entity.State = model.State;
                entity.Checker = model.Checker;
                entity.CheckTime = model.CheckTime;
                entity.CreateTime = model.CreateTime;
                entity.CreatePerson = model.CreatePerson;
                entity.ModifyTime = model.ModifyTime;
                entity.ModifyPerson = model.ModifyPerson;
                entity.Confirmation = model.Confirmation;
                entity.ToWarehouseId = model.ToWarehouseId;
                entity.FromWarehouseId = model.FromWarehouseId;
                entity.ContractNumber = model.ContractNumber;             

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

        //删除
        public override bool Delete(ref ValidationErrors errors, object id)
        {
            try
            {
                Spl_WarehouseAllocation entity = new Spl_WarehouseAllocation();
                if (entity.State == 1)
                {
                    errors.Add("单据已审核不能被删除");
                    return false;
                }

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

        public List<Spl_WareAllocationReportModel> GetWareAllocationList(string warehouseId, DateTime begin, DateTime end)
        {
            return m_Rep.GetWareAllocationList(warehouseId, begin,end).ToList();
        }
    }
}
