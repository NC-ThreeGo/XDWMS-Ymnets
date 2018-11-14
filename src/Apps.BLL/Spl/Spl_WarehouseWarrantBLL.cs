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
    public partial class Spl_WarehouseWarrantBLL
    {
        [Dependency]
        public ISysUserRepository userBLL { get; set; }


        public List<Spl_WarehouseWarrantModel> GetList(ref GridPager pager, string queryStr,string sysUserId)
        {
            //获取用户对应的仓库权限 
            List<string> houseList = userBLL.GetHouseList(sysUserId);

            IQueryable<Spl_WarehouseWarrant> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                                a => (a.Id.Contains(queryStr)

                                || a.Handler.Contains(queryStr)
                                || a.Remark.Contains(queryStr)


                                || a.Checker.Contains(queryStr)


                                || a.CreatePerson.Contains(queryStr)

                                || a.ModifyPerson.Contains(queryStr)

                                || a.InOutCategoryId.Contains(queryStr)
                                || a.WarehouseId.Contains(queryStr)
                                || a.ContractNumber.Contains(queryStr))&& houseList.Contains(a.WarehouseId) 
                                );
            }
            else
            {
                queryData = m_Rep.GetList( a=>houseList.Contains(a.WarehouseId));
            }
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData(IEnumerable<Spl_WarehouseWarrantDetailsModel> personList)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    foreach (var model in personList)
                    {
                        Spl_WarehouseWarrantDetails entity = new Spl_WarehouseWarrantDetails();
                        entity.Id = model.Id;
                        entity.WareDetailsId = model.WareDetailsId;
                        entity.WarehouseId = model.WarehouseId;
                        entity.WarehouseWarrantId = model.WarehouseWarrantId;
                        entity.Quantity = model.Quantity;
                        entity.Price = model.Price;
                        entity.TotalPrice = model.TotalPrice;
                        entity.Defined = model.Defined;
                        entity.CreateTime = model.CreateTime;
                        //新增
                        if (string.IsNullOrEmpty(model.Id))
                        {
                            entity.Id = ResultHelper.NewId;
                            db.Spl_WarehouseWarrantDetails.Add(entity);
                        }
                        else
                        {
                            //修改
                            db.Set<Spl_WarehouseWarrantDetails>().Attach(entity);
                            db.Entry<Spl_WarehouseWarrantDetails>(entity).State = EntityState.Modified;
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

        public void SaveEditData(IEnumerable<Spl_WarehouseWarrantDetailsModel> personList, string warehouseWarrantId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {

                    int count = db.Database.ExecuteSqlCommand("delete Spl_WarehouseWarrantDetails where WarehouseWarrantId=@WarehouseWarrantId", new SqlParameter("@WarehouseWarrantId", warehouseWarrantId));
                    foreach (var model in personList)
                    {
                        Spl_WarehouseWarrantDetails entity = new Spl_WarehouseWarrantDetails();
                        entity.Id = model.Id;
                        entity.WareDetailsId = model.WareDetailsId;
                        entity.WarehouseId = model.WarehouseId;
                        entity.WarehouseWarrantId = model.WarehouseWarrantId;
                        entity.Quantity = model.Quantity;
                        entity.Price = model.Price;
                        entity.TotalPrice = model.TotalPrice;
                        entity.Defined = model.Defined;
                        entity.CreateTime = model.CreateTime;
                        entity.Id = ResultHelper.NewId;
                        db.Spl_WarehouseWarrantDetails.Add(entity);

                    }

                    db.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public override List<Spl_WarehouseWarrantModel> CreateModelList(ref IQueryable<Spl_WarehouseWarrant> queryData)
        {

            List<Spl_WarehouseWarrantModel> modelList = (from r in queryData
                                                         select new Spl_WarehouseWarrantModel
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
                                                             InOutCategoryId = r.InOutCategoryId,
                                                             WarehouseId = r.WarehouseId,
                                                             WarehouseName = r.Spl_Warehouse.Name,
                                                             ContractNumber = r.ContractNumber,
                                                             InOutCategoryName = r.Spl_InOutCategory.Name,
                                                         }).ToList();
            return modelList;
        }

        public override Spl_WarehouseWarrantModel GetById(object id)
        {
            if (IsExists(id))
            {
                Spl_WarehouseWarrant entity = m_Rep.GetById(id);
                Spl_WarehouseWarrantModel model = new Spl_WarehouseWarrantModel();
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
                model.InOutCategoryId = entity.InOutCategoryId;
                model.InOutCategoryName = entity.Spl_InOutCategory.Name;
                model.WarehouseId = entity.WarehouseId;
                model.WarehouseName = entity.Spl_Warehouse.Name;
                model.InOutCategoryName = entity.Spl_InOutCategory.Name;
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
                Spl_WarehouseWarrant entity = m_Rep.GetById(id);
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
                    m_Rep.UpdateWareStockPileIn(entity.Id);
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
        public override bool Edit(ref ValidationErrors errors, Spl_WarehouseWarrantModel model)
        {
            try
            {
                Spl_WarehouseWarrant entity = m_Rep.GetById(model.Id);
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
                entity.InOutCategoryId = model.InOutCategoryId;
                entity.WarehouseId = model.WarehouseId;
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
                Spl_WarehouseWarrant entity = new Spl_WarehouseWarrant();
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

        public List<Spl_WareInReportModel> GetWareInList(string warehouseId, DateTime begin, DateTime end)
        {
            return m_Rep.GetWareInList(warehouseId, begin,end).ToList();
        }
    }
}
