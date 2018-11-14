using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.Spl;
using Apps.Locale;
using Apps.BLL.Core;
using Apps.IDAL.Sys;
using Unity.Attributes;

namespace Apps.BLL.Spl
{
    public partial class Spl_WareStockPileBLL
    {
        [Dependency]
        public ISysUserRepository userBLL { get; set; }

        public List<Spl_WreStockPileListModel> GetPileList(string wareDetailsId, string warehouseId)
        {
            return m_Rep.GetPileList(wareDetailsId, warehouseId).ToList();
        }
        //public List<Spl_WareStockPileModel> GetListview(string WarehouseId, int Quantity, int WaringQuantity)
        //{
        //    return m_Rep.GetListview(WarehouseId, Quantity, WaringQuantity).ToList();//修改于2018年3月7
        //}
        public List<Spl_WareStockPileModel> GetListByParentId(ref GridPager pager, string queryStr, object parentId,string sysUserId)
    {
            List<string> houseList = userBLL.GetHouseList(sysUserId);
            IQueryable<Spl_WareStockPile> queryData = null;
        string pid = parentId.ToString();
        if (pid != "0")
        {
            queryData = m_Rep.GetList(a => a.WarehouseId == pid && houseList.Contains(a.WarehouseId));
        }
        else
        {
            queryData = m_Rep.GetList(a=> houseList.Contains(a.WarehouseId));
        }
        if (!string.IsNullOrWhiteSpace(queryStr))
        {
           if (queryStr == "querywaring")
           {
               queryData = m_Rep.GetList(a => a.Quantity <= a.WaringQuantity && houseList.Contains(a.WarehouseId));
                }
           else
           {
               queryData = m_Rep.GetList(
                     a => (
                             (a.Id.Contains(queryStr)
                            || a.WarehouseId.Contains(queryStr)
                            || a.WareDetailsId.Contains(queryStr)
                            || a.Spl_WareDetails.Code.Contains(queryStr)
                            || a.Spl_WareDetails.Name.Contains(queryStr)) && houseList.Contains(a.WarehouseId)
                          )
                     );
           }
        }



        pager.totalRows = queryData.Count();
        queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
        return CreateModelList(ref queryData);
    }
        //public override List<Spl_WareStockPileModel> GetListview(ref GridPager pager, string queryStr, int Quantity, int WaringQuantity)
        //{

        //    //修改2018年3月7
        //    IQueryable<Spl_WareStockPile> queryData = null;
        //    int Value = Quantity.GetInt();
        //    int WaringValue = WaringQuantity.GetInt();

        //    if (Value <= WaringValue)
        //    {
        //        queryData = m_Rep.GetList(a => a.Quantity<a.WaringQuantity);
        //    }
        //    else
        //    {
        //        queryData = m_Rep.GetList();
        //    }
        //    if (!string.IsNullOrWhiteSpace(queryStr))
        //    {
        //        queryData = m_Rep.GetList(
        //                    a => (
        //                            a.Id.Contains(queryStr)
        //                           || a.WarehouseId.Contains(queryStr)
        //                           || a.WareDetailsId.Contains(queryStr)
        //                           || a.Spl_WareDetails.Code.Contains(queryStr)
        //                           || a.Spl_WareDetails.Name.Contains(queryStr)
        //                         )
        //                    );
        //    }
        //    pager.totalRows = queryData.Count();
        //    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
        //    return CreateModelList(ref queryData);
        //}
        public override List<Spl_WareStockPileModel> CreateModelList(ref IQueryable<Spl_WareStockPile> queryData)
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
                                                  WarehouseName = r.Spl_Warehouse.Name,
                                                  WareDetailsCode = r.Spl_WareDetails.Code,
                                                  WareDetailsUnit = r.Spl_WareDetails.Unit,
                                                  WareDetailsCategory = r.Spl_WareDetails.Spl_WareCategory.Name,
                                                  WareDetailsVender = r.Spl_WareDetails.Vender,
                                                  WareDetailsBrand = r.Spl_WareDetails.Brand,
                                                  WareDetailsSize = r.Spl_WareDetails.Size,
                                                  WareDetailsName = r.Spl_WareDetails.Name,
                                                  WaringQuantity = r.WaringQuantity,
                                              }).ToList();
            return modelList;
        }


        public List<Spl_WareStockReportModel> GetWareStockList(string warehouseId)
        {
            return m_Rep.GetWareStockList(warehouseId).ToList();//修改2018年3月13 begin end
        }

        public override Spl_WareStockPileModel GetById(object id)
        {
            if (IsExists(id))
            {
                Spl_WareStockPile entity = m_Rep.GetById(id);
                Spl_WareStockPileModel model = new Spl_WareStockPileModel();
                model.Id = entity.Id;
                model.WarehouseId = entity.WarehouseId;
                model.WareDetailsId = entity.WareDetailsId;
                model.FirstEnterDate = entity.FirstEnterDate;
                model.LastLeaveDate = entity.LastLeaveDate;
                model.WaringQuantity = entity.WaringQuantity;
                model.Quantity = entity.Quantity;
                model.Price = entity.Price;
                model.CreateTime = entity.CreateTime;
                model.WarehouseName = entity.Spl_Warehouse.Name;
                model.WareDetailsCode = entity.Spl_WareDetails.Code;
                model.WareDetailsUnit = entity.Spl_WareDetails.Unit;
                model.WareDetailsCategory = entity.Spl_WareDetails.Spl_WareCategory.Name;
                model.WareDetailsVender = entity.Spl_WareDetails.Vender;
                model.WareDetailsBrand = entity.Spl_WareDetails.Brand;
                model.WareDetailsSize = entity.Spl_WareDetails.Size;
                model.WareDetailsName = entity.Spl_WareDetails.Name;
                model.WaringQuantity = entity.WaringQuantity;
                return model;
            }
            else
            {
                return null;
            }
        }


        //public override bool Edit(ref ValidationErrors errors, Spl_WareStockPileModel model)
        //{
        //    try
        //    {
        //        Spl_WareStockPile entity = m_Rep.GetById(model.Id);
        //        if (entity == null)
        //        {
        //            errors.Add(Resource.Disable);
        //            return false;
        //        }
        //        entity.Id = model.Id;
        //        entity.WarehouseId = model.WarehouseId;
        //        entity.WareDetailsId = model.WareDetailsId;
        //        entity.FirstEnterDate = model.FirstEnterDate;
        //        entity.LastLeaveDate = model.LastLeaveDate;
        //        entity.WaringQuantity = model.WaringQuantity;
        //        entity.Quantity = model.Quantity;
        //        entity.Price = model.Price;
        //        entity.CreateTime = model.CreateTime;



        //        if (m_Rep.Edit(entity))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            errors.Add(Resource.NoDataChange);
        //            return false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(ex.Message);
        //        ExceptionHander.WriteException(ex);
        //        return false;
        //    }
        //}
    }
 }

