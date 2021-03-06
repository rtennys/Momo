﻿using System;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Momo.Domain.Entities;
using NHibernate;

namespace Momo.UI.Infrastructure
{
    public interface INHibernateSessionFactoryHelper
    {
        ISessionFactory CurrentSessionFactory { get; }
    }

    public sealed class NHibernateSessionFactoryHelper : INHibernateSessionFactoryHelper
    {
        private ISessionFactory _currentSessionFactory;

        public ISessionFactory CurrentSessionFactory => _currentSessionFactory ?? (_currentSessionFactory = CreateSessionFactory());

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

        private sealed class MyIdConvention : IIdConvention
        {
            public void Apply(IIdentityInstance instance)
            {
                instance.CustomType<int>();
                instance.GeneratedBy.Identity();
            }
        }

        private sealed class MyForeignKeyConvention : ForeignKeyConvention
        {
            protected override string GetKeyName(Member property, Type type)
            {
                object arg0 = property != null ? property.Name : type.Name;
                return $"{arg0}Id";
            }
        }

        private sealed class MyCollectionConvention : ICollectionConvention
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
