
using Apps.IBLL;
using Apps.Models;
using Apps.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Apps.Common;
using Apps.DAL;
using Apps.DAL.Sys;
using Apps.BLL.Sys;
using Apps.DAL.Spl;
using System.Configuration;

namespace Apps.Web.Core
{
    public class CommonHelper
    {
        #region 系统级


        #region 获取多选不带人员的组织架构
        public string GetStructMulTree()
        {
            StringBuilder sb = new StringBuilder();
            using (SysStructRepository structRep = new SysStructRepository(new DBContainer()))
            {
                IQueryable<SysStruct> queryData = structRep.GetList();
                IQueryable<SysStruct> query = queryData.Where(a => a.ParentId == "0").OrderBy(a => a.Sort);
                sb.Append("<ul id=\"StructMulTree\" class=\"easyui-tree\"  data-options=\"checkbox:true\">");
                foreach (var l in query)
                {
                    sb.Append("<li data-options=\"attributes:{'id':'" + l.Id + "'}\">");
                    sb.AppendFormat("<span>{0}</span>", l.Name);
                    sb.Append(GetStructLayout(queryData, l.Id, false));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }
        #endregion


        #region 组织架构
        public string GetStructTree(bool isCount)
        {
            StringBuilder sb = new StringBuilder();
            using (SysStructRepository structRep = new SysStructRepository(new DBContainer()))
            {
                IQueryable<SysStruct> queryData = structRep.GetList();
                IQueryable<SysStruct> query = queryData.Where(a => a.ParentId == "0").OrderBy(a => a.Sort);
                sb.Append("<ul id=\"StructTree\" class=\"easyui-tree\"  data-options=\"onClick:function(node){ getSelected();}\">");
                foreach (var l in query)
                {
                    sb.Append("<li data-options=\"attributes:{'id':'" + l.Id + "'}\">");
                    if (isCount)
                    {
                        sb.AppendFormat("<span>{0}</span>", l.Name);
                    }
                    else
                    {
                        sb.AppendFormat("<span>{0}</span>", l.Name);
                    }
                    sb.Append(GetStructLayout(queryData, l.Id, isCount));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }

        public int GetMemberCount(string depId)
        {
            using (SysUserRepository m_Rep = new SysUserRepository(new DBContainer()))
            {

                return m_Rep.GetUserCountByDepId(depId);
            }
        }
        public string GetStructLayout(IQueryable<SysStruct> queryData, string parentId, bool isCount)
        {
            IQueryable<SysStruct> query = queryData.Where(a => a.ParentId == parentId).OrderBy(a => a.Sort);
            StringBuilder sb = new StringBuilder();
            if (query.Count() > 0)
            {

                sb.Append("<ul>");
                foreach (var r in query)
                {
                    sb.Append("<li data-options=\"attributes:{'id':'" + r.Id + "'}\">");
                    if (isCount)
                    {
                        sb.AppendFormat("<span>{0}</span>", r.Name);
                    }
                    else
                    {
                        sb.AppendFormat("<span>{0}</span>", r.Name);
                    }
                    sb.Append(GetStructLayout(queryData, r.Id, isCount));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }


        #endregion

        #region 组织架构+职位
        public string GetStructAndPosTree()
        {
            StringBuilder sb = new StringBuilder();
            using (SysStructRepository structRep = new SysStructRepository(new DBContainer()))
            {
                IQueryable<SysStruct> queryData = structRep.GetList();
                IQueryable<SysStruct> query = queryData.Where(a => a.ParentId == "0").OrderBy(a => a.Sort);
                sb.Append("<ul id=\"StructTree\" class=\"easyui-tree\"  data-options=\"animate:true,checkbox:true,onClick:function(node){ getSelected();}\">");
                foreach (var l in query)
                {
                    sb.Append("<li data-options=\"attributes:{'id':'" + l.Id + "'}\">");

                    sb.AppendFormat("<span>{0}</span>", l.Name);

                    sb.Append(GetStructAndPosLayout(queryData, l.Id));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }


        public string GetStructAndPosLayout(IQueryable<SysStruct> queryData, string parentId)
        {
            IQueryable<SysStruct> query = queryData.Where(a => a.ParentId == parentId).OrderBy(a => a.Sort);
            StringBuilder sb = new StringBuilder();
            if (query.Count() > 0)
            {

                sb.Append("<ul>");
                foreach (var r in query)
                {
                    sb.Append("<li data-options=\"attributes:{'id':'" + r.Id + "'}\">");
                    sb.AppendFormat("<span>{0}</span>", r.Name);
                    string str = GetStructAndPosLayout(queryData, r.Id);
                    //如果获取不到底层，那么需要组织职位
                    if (string.IsNullOrEmpty(str))
                    {
                        sb.Append(GetPosLayoutByStruct(r.Id));
                        sb.Append("</li>");
                    }
                    else
                    {
                        sb.Append(GetStructAndPosLayout(queryData, r.Id));
                        sb.Append("</li>");
                    }
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }


        //根据组织架构的ID获取职位
        public string GetPosLayoutByStruct(string structId)
        {
            using (SysPositionRepository posRep = new SysPositionRepository(new DBContainer()))
            {
                IQueryable<SysPosition> queryData = posRep.GetList();
                IQueryable<SysPosition> query = queryData.Where(a => a.DepId == structId).OrderBy(a => a.Sort);
                StringBuilder sb = new StringBuilder();
                if (query.Count() > 0)
                {

                    sb.Append("<ul>");
                    foreach (var r in query)
                    {
                        sb.Append("<li data-options=\"attributes:{'id':'" + r.Id + "'}\">");
                        sb.AppendFormat("<span>{0}</span>", r.Name);
                    }
                    sb.Append("</ul>");
                }
                return sb.ToString();
            }
        }

        #endregion
        #endregion

        #region 模块级
        public string GetWareCateogryTree(bool isCount)
        {
            StringBuilder sb = new StringBuilder();
            using (Spl_WareCategoryRepository structRep = new Spl_WareCategoryRepository(new DBContainer()))
            {
                List<Spl_WareCategory> queryData = structRep.GetList().ToList();
                List<Spl_WareCategory> query = queryData.Where(a => a.ParentId == "0").OrderBy(a => a.CreateTime).ToList();
                string str = "";
                sb.Append("<ul id=\"StructTree\" class=\"easyui-tree\"  data-options=\"onClick:function(node){ getSelected();}\">");
                foreach (var l in query)
                {
                    str = GetWareCategoryLayout(queryData, l.Id, isCount);
                    if (str != "")
                    {
                        sb.Append("<li data-options=\"state:'closed',attributes:{'id':'" + l.Id + "'}\">");
                    }
                    else
                    {
                        sb.Append("<li data-options=\"attributes:{'id':'" + l.Id + "'}\">");
                    }

                    if (isCount)
                    {
                        sb.AppendFormat("<span>{0}</span>", l.Name);
                    }
                    else
                    {
                        sb.AppendFormat("<span>{0}</span>", l.Name);
                    }
                    sb.Append(GetWareCategoryLayout(queryData, l.Id, isCount));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }



        public string GetWareCategoryLayout(List<Spl_WareCategory> queryData, string parentId, bool isCount)
        {
            List<Spl_WareCategory> query = queryData.Where(a => a.ParentId == parentId).OrderBy(a => a.CreateTime).ToList();
            StringBuilder sb = new StringBuilder();
            string str = "";
            if (query.Count() > 0)
            {

                sb.Append("<ul>");
                foreach (var r in query)
                {
                    str = GetWareCategoryLayout(queryData, r.Id, isCount);
                    if (str != "")
                    {
                        sb.Append("<li data-options=\"state:'closed',attributes:{'id':'" + r.Id + "'}\">");
                    }
                    else
                    {
                        sb.Append("<li data-options=\"attributes:{'id':'" + r.Id + "'}\">");
                    }
                    if (isCount)
                    {
                        sb.AppendFormat("<span>{0}</span>", r.Name);
                    }
                    else
                    {
                        sb.AppendFormat("<span>{0}</span>", r.Name);
                    }
                    sb.Append(str);
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }



        public int GetWareCount(string depId)
        {
            using (Spl_WareDetailsRepository m_Rep = new Spl_WareDetailsRepository(new DBContainer()))
            {

                return m_Rep.GetWareCountByCategoryId(depId);
            }
        }



        #endregion


        #region 根据日期生成批次，如果大于规定的Day，则作为下个月的批次
        public static string GetLot(DateTime dateTime)
        {
            int maxLotDay = Int32.Parse(ConfigurationManager.AppSettings["MaxLotDay"] ?? "24");
            if (dateTime.Day > maxLotDay)
            {
                return dateTime.AddMonths(1).ToString("yyyyMM");
            }
            else
            {
                return dateTime.ToString("yyyyMM");
            }
        }
        #endregion

    }
}