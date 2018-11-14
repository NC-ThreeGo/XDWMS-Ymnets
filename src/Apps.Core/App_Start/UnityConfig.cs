using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;

namespace Apps.Core
{
    public static class UnityConfig
    {
        public static void RegisterComponentsByWebApi()
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            //×¢Èë Ioc
            // var container = new UnityContainer();
            UsingUnityContainer.Init();
            DependencyRegisterType.Container(ref UsingUnityContainer._container);
            DependencyRegisterType.ContainerPartial(ref UsingUnityContainer._container);
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(UsingUnityContainer._container);
        }

        public static void RegisterComponents()
        {
            UsingUnityContainer.Init();
            DependencyRegisterType.Container(ref UsingUnityContainer._container);
            DependencyRegisterType.ContainerPartial(ref UsingUnityContainer._container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(UsingUnityContainer._container));
        }
    }
}