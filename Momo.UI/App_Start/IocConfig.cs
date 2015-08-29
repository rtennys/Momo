using System;
using System.Web.Http;
using System.Web.Mvc;
using Momo.Common;
using Momo.Common.DataAccess;
using Momo.Domain;
using Momo.UI.Infrastructure;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Web;

namespace Momo.UI
{
    public static class IocConfig
    {
        public static Container Container { get; private set; }

        public static void Initialize()
        {
            Container = new Container(x =>
            {
                x.For<INHibernateSessionFactoryHelper>().Singleton().Use<NHibernateSessionFactoryHelper>();

                x.For<INHibernateUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<NHibernateUnitOfWork>();
                x.For<IUnitOfWork>().Use(ctx => ctx.GetInstance<INHibernateUnitOfWork>());

                x.For<IRepository>().Use<NHibernateRepository>();

                x.Scan(scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.StartsWith("Momo"));
                    scan.WithDefaultConventions();
                    scan.ConnectImplementationsToTypesClosing(typeof(ICommandHandler<>));
                });
            });

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(Container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapDependencyResolver(Container);

            Ioc.Initialize(new StructureMapDependencyResolver(Container));
        }
    }
}
