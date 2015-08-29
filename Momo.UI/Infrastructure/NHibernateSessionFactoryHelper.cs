using System;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Momo.Common;
using Momo.Domain.Entities;
using NHibernate;

namespace Momo.UI.Infrastructure
{
    public interface INHibernateSessionFactoryHelper
    {
        ISessionFactory CurrentSessionFactory { get; }
    }

    public class NHibernateSessionFactoryHelper : INHibernateSessionFactoryHelper
    {
        private ISessionFactory _currentSessionFactory;

        public ISessionFactory CurrentSessionFactory { get { return _currentSessionFactory ?? (_currentSessionFactory = CreateSessionFactory()); } }

        private static ISessionFactory CreateSessionFactory()
        {
            var entityBaseType = typeof(EntityBase);
            var logType = typeof(Log);

            var autoPersistenceModel = AutoMap
                .AssemblyOf<EntityBase>()
                .Where(x => entityBaseType.IsAssignableFrom(x) || x == logType)
                .Conventions.Add<MyIdConvention>()
                .Conventions.Add<MyForeignKeyConvention>()
                .Conventions.Add<MyCollectionConvention>()
                .Override<ShoppingList>(mapping => mapping.HasManyToMany(x => x.SharedWith).Table("ShoppingListToUserProfile").Not.Inverse().Cascade.SaveUpdate())
                .Override<UserProfile>(mapping =>
                {
                    mapping.HasMany(x => x.ShoppingLists).Table("ShoppingList");
                    mapping.HasManyToMany(x => x.SharedLists).Table("ShoppingListToUserProfile").Inverse().Cascade.None();
                });

            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(x => x.FromConnectionStringWithKey("momo_conn")))
                .Mappings(map => map.AutoMappings.Add(autoPersistenceModel))
                .BuildSessionFactory();
        }

        public class MyIdConvention : IIdConvention
        {
            public void Apply(IIdentityInstance instance)
            {
                instance.CustomType<int>();
                instance.GeneratedBy.Identity();
            }
        }

        public class MyForeignKeyConvention : ForeignKeyConvention
        {
            protected override string GetKeyName(Member property, Type type)
            {
                return "{0}Id".F(property != null ? property.Name : type.Name);
            }
        }

        public class MyCollectionConvention : ICollectionConvention
        {
            public void Apply(ICollectionInstance instance)
            {
                instance.Access.CamelCaseField(CamelCasePrefix.Underscore);
                instance.Cascade.AllDeleteOrphan();
                instance.Inverse();
            }
        }
    }
}
