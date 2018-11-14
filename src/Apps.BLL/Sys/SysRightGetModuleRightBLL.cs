using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.IBLL;
using Apps.Common;
using Unity.Attributes;
using Apps.IDAL;
using Apps.BLL.Core;
using Apps.Models;
using Apps.IDAL.Sys;
using Apps.IBLL.Sys;

namespace Apps.BLL.Sys
{
    public class SysRightGetModuleRightBLL : ISysRightGetModuleRightBLL
    {
        [Dependency]
        public ISysModuleRepository moduleRepository { get; set; }
        [Dependency]
        public ISysRightGetModuleRightRepository sysRightGetModuleRightRepository { get; set; }
        public object GetPerantModule(GridPager pager, string nodeid, string parentid, int? n_level)
        {
            string pid = string.IsNullOrEmpty(nodeid) ? "0" : nodeid;

            int level = (n_level == null ? 0 : Convert.ToInt32(n_level) + 1);
            var parentModule = from r in moduleRepository.GetList()
                               where r.IsLast == false
                               orderby r.Id
                               select r;

            int totalRecords = parentModule.Count();

            var modules = parentModule.Where(a => a.ParentId == pid && a.Id != "0").OrderBy(a => a.Sort).ToList();
            var moduleChilds = from r in moduleRepository.GetList()
                               where r.IsLast == false
                               orderby r.Id
                               select r;


            var jsonData = new
            {
                total = 1,
                page = 1,
                records = totalRecords,
                rows = (
                    from r in modules
                    select new
                    {
                        cell = new string[]
                            {
                                r.Id.ToString(),//模块ID
                                r.Name,         //
                                level.ToString(),            //Level:the current row level,0=root row,1=first row of root,and so on.
                                pid,//ParentId :The parent row id of the current cell,null=no parent.
                                //测试当前记录是否还有子项
                                moduleChilds.Where(a=>a.ParentId==r.Id).Count()>0?"false":"true",  
                                "false"         //Is expanded:if it is expanded when the first loading?
                            }
                    }

                ).ToArray()

            };

            return jsonData;
        }

        /// <summary>
        /// 取模块操作项
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<SysModuleOperate> GetModuleOperateByModuleId(string moduleId)
        {
            return sysRightGetModuleRightRepository.GetList().Where(a => a.ModuleId == moduleId).ToList();
        }


        public List<P_Sys_GetModule_RoleRight_Result> GetModuleRoleRight(string moduleId)
        {
            return sysRightGetModuleRightRepository.GetModuleRoleRight(moduleId);
        }

        public List<P_Sys_GetModule_UserRight_Result> GetModuleUserRight(string moduleId)
        {
            return sysRightGetModuleRightRepository.GetModuleUserRight(moduleId);
        }
    }
}
