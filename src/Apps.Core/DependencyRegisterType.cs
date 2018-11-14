using Unity.Attributes;
using Apps.IBLL;
using Apps.BLL;
using Apps.IDAL;
using Apps.DAL;
using Apps.IBLL.Sys;
using Apps.IDAL.Sys;
using Apps.BLL.Sys;
using Apps.DAL.Sys;
using Unity;

namespace Apps.Core
{

    public partial class DependencyRegisterType
    {
        //如果是表命名的模块在DependencyRegisterTypeAuto会自动生成，必要时手写的模块需要再这里手写注入
        public static void ContainerPartial(ref UnityContainer container)
        {
            container.RegisterType<IHomeBLL, HomeBLL>();//首页
            container.RegisterType<IHomeRepository, HomeRepository>();
            container.RegisterType<IWebpartBLL, WebpartBLL>();//首页
            container.RegisterType<IWebpartRepository, WebpartRepository>();
            container.RegisterType<IAccountBLL, AccountBLL>();//用户
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<ISysRightGetRoleRightBLL, SysRightGetRoleRightBLL>();//查看角色权限
            container.RegisterType<ISysRightGetRoleRightRepository, SysRightGetRoleRightRepository>();
            container.RegisterType<ISysRightGetUserRightBLL, SysRightGetUserRightBLL>();//查看用户权限
            container.RegisterType<ISysRightGetUserRightRepository, SysRightGetUserRightRepository>();
            container.RegisterType<ISysRightGetModuleRightBLL, SysRightGetModuleRightBLL>();//查看模块被赋予权限
            container.RegisterType<ISysRightGetModuleRightRepository, SysRightGetModuleRightRepository>();
        }



    }
}