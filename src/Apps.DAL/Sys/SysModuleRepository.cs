using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.IDAL;
using System.Data;
using Apps.Common;
namespace Apps.DAL.Sys
{
    public partial class SysModuleRepository
    {

        public IQueryable<SysModule> GetModuleBySystem(string parentId)
        {
            return Context.SysModule.Where(a => a.ParentId == parentId).AsQueryable();
        }

        public override bool Create(SysModule entity)
        {
                Context.SysModule.Add(entity);
                //创建成功--自动生成增，删，改，查，存，审核操作码(最后一项才执行)并且不是API接口的
                if (entity.IsLast && entity.ParentId!= "ApiInterfaceAuth")
                {
                    string[,] arr = new string[7, 2];
                    arr[0, 0] = "创建";
                    arr[0, 1] = "Create";
                    arr[1, 0] = "删除";
                    arr[1, 1] = "Delete";
                    arr[2, 0] = "修改";
                    arr[2, 1] = "Edit";
                    arr[3, 0] = "保存";
                    arr[3, 1] = "Save";
                    arr[4, 0] = "审核";
                    arr[4, 1] = "Check";
                    arr[5, 0] = "反审核";
                    arr[5, 1] = "UnCheck";
                    arr[6, 0] = "查询";
                    arr[6, 1] = "Query";
                    for (int i = 0; i <7; i++)
                    {
                        SysModuleOperate cretaeEntity = new SysModuleOperate();
                        cretaeEntity.Id = ResultHelper.NewId;
                        cretaeEntity.Name = arr[i, 0];
                        cretaeEntity.KeyCode = arr[i, 1];
                        cretaeEntity.ModuleId = entity.Id;
                        cretaeEntity.IsValid = true;
                        cretaeEntity.Sort = 1;
                        Context.SysModuleOperate.Add(cretaeEntity);
                    }
                }
                return this.SaveChanges()>0;
            }
        

       

        public int Delete(string id)
        {
            SysModule entity = Context.SysModule.SingleOrDefault(a => a.Id == id);
            if (entity != null)
            {
                
                //删除SysRight表数据
                var sr = Context.SysRight.AsQueryable().Where(a => a.ModuleId == id);
                foreach(var o in sr)
                {
                    //删除SysRightOperate表数据
                    var sro = Context.SysRightOperate.AsQueryable().Where(a => a.RightId == o.Id);
                    foreach(var o2 in sro)
                    {
                        Context.SysRightOperate.Remove(o2);
                    }
                    Context.SysRight.Remove(o);
                }
                //删除SysModuleOperate数据
                var smo = Context.SysModuleOperate.AsQueryable().Where(a => a.ModuleId == id);
                foreach (var o3 in smo)
                {
                    Context.SysModuleOperate.Remove(o3);
                }
                Context.SysModule.Remove(entity);
            }
            return this.SaveChanges();
        }

        public void P_Sys_InsertSysRight() {
            Context.P_Sys_InsertSysRight();
        }
        //清理无用的项
        public void P_Sys_ClearUnusedRightOperate(){
            Context.P_Sys_ClearUnusedRightOperate();
        }

        public int GetChildrenCount(string id)
        {
            return Context.SysModule.AsQueryable().Where(a => a.SysModule2.Id == id).Count();
        }
    }
}
