using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.Spl;
using Unity.Attributes;
using Apps.IDAL.Spl;
using Apps.Locale;
using Apps.BLL.Core;
using Apps.IDAL.Sys;


namespace Apps.BLL.Spl
{
    public partial class Spl_WareCheckTotalBLL
    {
        [Dependency]
        public ISysUserRepository userBLL { get; set; }

        [Dependency]
        public ISpl_WareStockPileRepository checkRepository { get; set; }


        public List<Spl_WareCheckTotalModel> GetList(ref GridPager pager, string queryStr,string sysUserId)
        {
            //获取用户对应的仓库权限 
            List<string> houseList = userBLL.GetHouseList(sysUserId);
            IQueryable<Spl_WareCheckTotal> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                                a => (a.Id.Contains(queryStr)
                                || a.WareDetailsId.Contains(queryStr)
                                || a.WarehouseId.Contains(queryStr)
                                || a.Remark.Contains(queryStr)
                                || a.Creater.Contains(queryStr)
                                || a.Checker.Contains(queryStr)) && houseList.Contains(a.WarehouseId)
                                );
            }
            else
            {
                queryData = m_Rep.GetList(a=> houseList.Contains(a.WarehouseId));
            }
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public Spl_WareStockPileModel GetQuantity(string warehouseid, string waredetailsid)
        {

            Spl_WareStockPileModel model = new Spl_WareStockPileModel();
            IQueryable<Spl_WareStockPile> queryData = checkRepository.GetList(a=>a.WarehouseId==warehouseid && a.WareDetailsId == waredetailsid);
            List<Spl_WareStockPileModel> list = CreateModelList2(ref queryData);
            if (list.Count == 0)
            {
                return model;
            }
         
            return list[0];
        }


        public virtual List<Spl_WareStockPileModel> CreateModelList2(ref IQueryable<Spl_WareStockPile> queryData)
        {

            List<Spl_WareStockPileModel> modelList = (from r in queryData
                                                      select new Spl_WareStockPileModel
                                                      {
                                                          Id = r.Id,
                                                          WarehouseId = r.WarehouseId,
                                                          WareDetailsId = r.WareDetailsId,
                                                          FirstEnterDate = r.FirstEnterDate,
                                                          LastLeaveDate = r.LastLeaveDate,
                                                          Quantity = r.Quantity,
                                                          Price = r.Price,
                                                          CreateTime = r.CreateTime,

                                                      }).ToList();

            return modelList;
        }

        public List<Spl_WareCheckTotalModel> GetListByParentId(ref GridPager pager, string queryStr, object parentId,string sysUserId)
        {
            //获取用户对应的仓库权限 
            List<string> houseList = userBLL.GetHouseList(sysUserId);
            IQueryable<Spl_WareCheckTotal> queryData = null;
            string pid = parentId.ToString();
            if (pid != "0")
            {
                queryData = m_Rep.GetList(a => a.WarehouseId == pid && houseList.Contains(a.WarehouseId));
            }
            else
            {
                queryData = m_Rep.GetList(a=>houseList.Contains(a.WarehouseId));
            }
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                            a => (
                                    (a.Id.Contains(queryStr)
                                   || a.WareDetailsId.Contains(queryStr)
                                   || a.WarehouseId.Contains(queryStr)
                                   || a.Remark.Contains(queryStr)
                                   || a.Creater.Contains(queryStr)
                                   || a.Checker.Contains(queryStr)) && houseList.Contains(a.WarehouseId)
                                 )
                            );
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<Spl_WareCheckTotalModel> CreateModelList(ref IQueryable<Spl_WareCheckTotal> queryData)
        {

            List<Spl_WareCheckTotalModel> modelList = (from r in queryData
                                                       select new Spl_WareCheckTotalModel
                                                       {
                                                           Id = r.Id,
                                                           WareDetailsId = r.WareDetailsId,
                                                           WarehouseId = r.WarehouseId,
                                                           Remark = r.Remark,
                                                           DiffQuantity = r.DiffQuantity,
                                                           Quantity = r.Quantity,
                                                           Price = r.Price,
                                                           State = r.State,
                                                           Creater = r.Creater,
                                                           Checker = r.Checker,
                                                           CheckTime = r.CheckTime,
                                                           Confirmation = r.Confirmation,
                                                           CreateTime = r.CreateTime,
                                                           WarehouseName = r.Spl_Warehouse.Name,
                                                           WareDetailsCode = r.Spl_WareDetails.Code,
                                                           WareDetailsUnit = r.Spl_WareDetails.Unit,
                                                           WareDetailsCategory = r.Spl_WareDetails.Spl_WareCategory.Name,
                                                           WareDetailsVender = r.Spl_WareDetails.Vender,
                                                           WareDetailsBrand = r.Spl_WareDetails.Brand,
                                                           WareDetailsSize = r.Spl_WareDetails.Size,
                                                           WareDetailsName = r.Spl_WareDetails.Name,
                                                       }).ToList();
            return modelList;
        }

        public override Spl_WareCheckTotalModel GetById(object id)
        {
            if (IsExists(id))
            {
                Spl_WareCheckTotal entity = m_Rep.GetById(id);
                Spl_WareCheckTotalModel model = new Spl_WareCheckTotalModel();
                model.Id = entity.Id;
                model.WareDetailsId = entity.WareDetailsId;
                model.WarehouseId = entity.WarehouseId;
                model.Remark = entity.Remark;
                model.DiffQuantity = entity.DiffQuantity;
                model.Quantity = entity.Quantity;
                model.Price = entity.Price;
                model.State = entity.State;
                model.Creater = entity.Creater;
                model.Checker = entity.Checker;
                model.CreaterName = entity.SysUser1.TrueName;
                model.CheckerName = entity.SysUser.TrueName;
                model.CheckTime = entity.CheckTime;
                model.Confirmation = entity.Confirmation;
                model.CreateTime = entity.CreateTime;
                model.WarehouseName = entity.Spl_Warehouse.Name;
                model.WareDetailsCode = entity.Spl_WareDetails.Code;
                model.WareDetailsUnit = entity.Spl_WareDetails.Unit;
                model.WareDetailsCategory = entity.Spl_WareDetails.Spl_WareCategory.Name;
                model.WareDetailsVender = entity.Spl_WareDetails.Vender;
                model.WareDetailsBrand = entity.Spl_WareDetails.Brand;
                model.WareDetailsSize = entity.Spl_WareDetails.Size;
                model.WareDetailsName = entity.Spl_WareDetails.Name;
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
                Spl_WareCheckTotal entity = m_Rep.GetById(id);
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
                    m_Rep.UpdateWareStockPileCheck(entity.Id);
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

        public override bool Delete(ref ValidationErrors errors, object id)
        {
            try
            {
                Spl_WareCheckTotal entity = m_Rep.GetById(id);
                if (entity.State == 1)
                {
                    errors.Add("单据已经审核不能被删除");
                    return false;
                }
                //if (entity.Confirmation == false)
                //{
                //    errors.Add("单据未经确认不能审核");
                //    return false;
                //}
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
    }
}
