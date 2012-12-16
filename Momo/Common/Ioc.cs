using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Momo.Common
{
    public static class Ioc
    {
        public static IServiceLocator CurrentLocator { get; private set; }

        public static IServiceLocator Initialize(IServiceLocator serviceLocator)
        {
            CurrentLocator = serviceLocator;
            ServiceLocator.SetLocatorProvider(() => CurrentLocator);
            return serviceLocator;
        }


        public static T Resolve<T>()
        {
            return CurrentLocator.GetInstance<T>();
        }

        public static T Resolve<T>(string key)
        {
            return CurrentLocator.GetInstance<T>(key);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return CurrentLocator.GetAllInstances<T>();
        }


        public static object Resolve(Type type)
        {
            return CurrentLocator.GetInstance(type);
        }

        public static object Resolve(Type type, string key)
        {
            return CurrentLocator.GetInstance(type, key);
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            return CurrentLocator.GetAllInstances(type);
        }
    }
}
