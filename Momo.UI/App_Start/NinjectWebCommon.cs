using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Momo.Domain;
using Momo.UI;
using Momo.UI.Infrastructure;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.NamedScope;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof(NinjectWebCommon), "Stop")]

namespace Momo.UI
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<INHibernateSessionFactoryHelper>().To<NHibernateSessionFactoryHelper>().InSingletonScope();
            kernel.Bind<IUnitOfWork, INHibernateUnitOfWork>().To<NHibernateUnitOfWork>().InScope(RequestOrCallScope);
            kernel.Bind<IRepository>().To<NHibernateRepository>().InScope(RequestOrCallScope);

            kernel.Bind(x => x.From("Momo", "Momo.UI")
                .SelectAllClasses()
                .Excluding<NHibernateSessionFactoryHelper>()
                .Excluding<NHibernateUnitOfWork>()
                .BindDefaultInterface());

            Ioc.Initialize(kernel);

            GlobalConfiguration.Configuration.DependencyResolver = kernel.Get<IDependencyResolver>();
        }

        private static object RequestOrCallScope(IContext context)
        {
            var requestScope = context.Kernel.Components
                .GetAll<INinjectHttpApplicationPlugin>()
                .Select(x => x.GetRequestScope(context))
                .FirstOrDefault(x => x != null);

            return requestScope ?? CallScope(context);
        }

        private static object CallScope(IContext context)
        {
            var parameter = context.Parameters.OfType<NamedScopeParameter>().SingleOrDefault();
            if (parameter != null)
                return parameter.Scope;

            if (context.Request.ParentContext != null)
                return CallScope(context.Request.ParentContext);

            throw new Exception("Use Ioc.Do instead of Ioc.Resolve");
        }
    }
}
