using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using Apps.Models.Spl;
using Unity.Attributes;
using Apps.IDAL.Spl;
using Apps.BLL.Core;
using LinqToExcel;
using System.Text;
using System.IO;
using System;

namespace Apps.BLL.Spl
{
    public partial class Spl_WareDetailsBLL
    {

        [Dependency]
        public ISpl_WareCategoryRepository categoryBLL { get; set; }

    

        public List<Spl_WareDetailsModel> GetListByWareHouse(ref GridPager pager, string queryStr, string category, string wareHouse)
        {

            IQueryable<Spl_WareDetailsModel> queryData = m_Rep.GetListByWareHouse(wareHouse).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = queryData.Where(
                                a => (a.Name.Contains(queryStr)
                                || a.Code.Contains(queryStr)
                                || a.BarCode.Contains(queryStr)
                                || a.WareCategoryId.Contains(queryStr)
                                || a.Unit.Contains(queryStr)
                                || a.Lable.Contains(queryStr))
                                );
            }
        


            if (!string.IsNullOrEmpty(category) && category != "root")
            {
                //查询获取所有级联的级别
                List<string> categoryIdList = GetChildRows(category).Select(a => a.Id).ToList();
                //同时也包含了自己
                categoryIdList.Add(category);
                //获得匹配
                queryData = queryData.Where(a => categoryIdList.Contains(a.WareCategoryId));
            }

            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList2(ref queryData);
        }

        public List<Spl_WareDetailsModel> CreateModelList2(ref IQueryable<Spl_WareDetailsModel> queryData)
        { 
            List<Spl_WareDetailsModel> modelList = (from r in queryData
                                                    select new Spl_WareDetailsModel
                                                    {
                                                        Id = r.Id,
                                                        Name = r.Name,
                                                        Code = r.Code,
                                                        BarCode = r.BarCode,
                                                        WareCategoryId = r.WareCategoryId,
                                                        Unit = r.Unit,
                                                        Lable = r.Lable,
                                                        BuyPrice = r.BuyPrice,
                                                        SalePrice = r.SalePrice,
                                                        RetailPrice = r.RetailPrice,
                                                        Remark = r.Remark,
                                                        Vender = r.Vender,
                                                        Brand = r.Brand,
                                                        Color = r.Color,
                                                        Material = r.Material,
                                                        Size = r.Size,
                                                        Weight = r.Weight,
                                                        ComeFrom = r.ComeFrom,
                                                        UpperLimit = r.UpperLimit,
                                                        LowerLimit = r.LowerLimit,
                                                        PrimeCost = r.PrimeCost,
                                                        Price1 = r.Price1,
                                                        Price2 = r.Price2,
                                                        Price3 = r.Price3,
                                                        Price4 = r.Price4,
                                                        Price5 = r.Price5,
                                                        Photo1 = r.Photo1,
                                                        Photo2 = r.Photo2,
                                                        Photo3 = r.Photo3,
                                                        Photo4 = r.Photo4,
                                                        Photo5 = r.Photo5,
                                                        Enable = r.Enable,
                                                        CreateTime = r.CreateTime,
                                                        WareCategoryName = r.WareCategoryName,
                                                        WareHouseName = r.WareHouseName,
                                                        Quantity = r.Quantity
                                                    }).ToList();
            return modelList;
        }



        public List<Spl_WareDetailsModel> GetList(ref GridPager pager, string queryStr, string category)
        {

            IQueryable<Spl_WareDetails> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                                a => (a.Name.Contains(queryStr)
                                || a.Code.Contains(queryStr)
                                || a.BarCode.Contains(queryStr)
                                || a.WareCategoryId.Contains(queryStr)
                                || a.Unit.Contains(queryStr)
                                || a.Lable.Contains(queryStr))
                                );
            }
            else
            {
                queryData = m_Rep.GetList();
            }

            if (!string.IsNullOrEmpty(category) && category != "root")
            {
                //查询获取所有级联的级别
                List<string> categoryIdList = GetChildRows(category).Select(a => a.Id).ToList();
                //同时也包含了自己
                categoryIdList.Add(category);
                //获得匹配
                queryData = queryData.Where(a => categoryIdList.Contains(a.WareCategoryId));
            }

            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public IEnumerable<Spl_WareCategory> GetChildRows(string categoryId)
        {
            DBContainer db = new DBContainer();

            var query = from c in db.Spl_WareCategory
                        where c.ParentId == categoryId
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetChildRows(t.Id)));
        }


        public override List<Spl_WareDetailsModel> CreateModelList(ref IQueryable<Spl_WareDetails> queryData)
        {

            List<Spl_WareDetailsModel> modelList = (from r in queryData
                                              select new Spl_WareDetailsModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  Code = r.Code,
                                                  BarCode = r.BarCode,
                                                  WareCategoryId = r.WareCategoryId,
                                                  Unit = r.Unit,
                                                  Lable = r.Lable,
                                                  BuyPrice = r.BuyPrice,
                                                  SalePrice = r.SalePrice,
                                                  RetailPrice = r.RetailPrice,
                                                  Remark = r.Remark,
                                                  Vender = r.Vender,
                                                  Brand = r.Brand,
                                                  Color = r.Color,
                                                  Material = r.Material,
                                                  Size = r.Size,
                                                  Weight = r.Weight,
                                                  ComeFrom = r.ComeFrom,
                                                  UpperLimit = r.UpperLimit,
                                                  LowerLimit = r.LowerLimit,
                                                  PrimeCost = r.PrimeCost,
                                                  Price1 = r.Price1,
                                                  Price2 = r.Price2,
                                                  Price3 = r.Price3,
                                                  Price4 = r.Price4,
                                                  Price5 = r.Price5,
                                                  Photo1 = r.Photo1,
                                                  Photo2 = r.Photo2,
                                                  Photo3 = r.Photo3,
                                                  Photo4 = r.Photo4,
                                                  Photo5 = r.Photo5,
                                                  Enable = r.Enable,
                                                  CreateTime = r.CreateTime,
                                                  WareCategoryName = r.Spl_WareCategory.Name,

                                              }).ToList();
            return modelList;
        }


        /// <summary>
        /// 校验Excel数据,这个方法一般用于重写校验逻辑
        /// </summary>
        public bool CheckImportData(string fileName, List<Spl_WareDetailsModel> list, ref ValidationErrors errors, string wareCategoryId)
        {

            var targetFile = new FileInfo(fileName);

            if (!targetFile.Exists)
            {

                errors.Add("导入的数据文件不存在");
                return false;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            //对应列头
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Code, "编码");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Name, "名称");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Brand, "品牌");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Size, "型号");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Unit, "单位");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.SalePrice, "单价");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Vender, "厂家");
            excelFile.AddMapping<Spl_WareDetailsModel>(x => x.Material, "技术参数");


            //SheetName
            var excelContent = excelFile.Worksheet<Spl_WareDetailsModel>(0);
            int rowIndex = 1;
            //检查数据正确性
            foreach (var row in excelContent)
            {
                if (row.Name != "" && row.Code != "")
                {
                    var errorMessage = new StringBuilder();
                    var entity = new Spl_WareDetailsModel();
                    entity.Id = ResultHelper.NewId;
                    entity.Name = row.Name;//名称
                    entity.Code = row.Code;//编码
                    entity.BarCode = row.Code;
                    entity.WareCategoryId = wareCategoryId;
                    entity.Unit = row.Unit;//单位
                    entity.SalePrice = row.SalePrice;//单价
                    entity.Brand = row.Brand;//品牌
                    entity.Vender = row.Vender;//厂家
                    entity.Size = row.Size;//型号
                    entity.Material = row.Material;//型号
                    entity.Enable = true;
                    entity.CreateTime = ResultHelper.NowTime;

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
        public override void SaveImportData(IEnumerable<Spl_WareDetailsModel> list)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    foreach (var model in list)
                    {
                        Spl_WareDetails entity = new Spl_WareDetails();
                        entity.Id = ResultHelper.NewId;
                        entity.Name = model.Name;
                        entity.Code = model.Code;
                        entity.BarCode = model.BarCode;
                        entity.WareCategoryId = model.WareCategoryId;
                        entity.Unit = model.Unit;
                        entity.SalePrice = model.SalePrice;
                        entity.Brand = model.Brand;
                        entity.Size = model.Size;
                        entity.Vender = model.Vender;
                        entity.Material = model.Material;
                        entity.Enable = model.Enable;
                        entity.CreateTime = ResultHelper.NowTime;
                        entity.Remark = "";
                        db.Spl_WareDetails.Add(entity);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }





    }
 }

