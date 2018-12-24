using System;
using System.Reflection;
using Ninject;
using Ninject.Extensions.NamedScope;

namespace Momo
{
    public static class Ioc
    {
        private static readonly Type _iocType = typeof(Ioc);
        private static readonly MethodInfo _doActionMethod = _iocType.GetMethod("DoAction", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo _doFuncMethod = _iocType.GetMethod("DoFunc", BindingFlags.Static | BindingFlags.NonPublic);

        private static IKernel _kernel;

        public static void Initialize(IKernel kernel)
        {
            _kernel = kernel;
        }

        public static T Resolve<T>()
        {
            return _kernel.Get<T>();
        }

        public static void Do<T>(Action<T> action)
        {
            DoAction(action);
        }

        public static void Do<T1, T2>(Action<T1, T2> action)
        {
            DoAction<Tuple<T1, T2>>(t => action(t.Item1, t.Item2));
        }

        public static TResult Do<T, TResult>(Func<T, TResult> func)
        {
            return DoFunc(func);
        }

        public static TResult Do<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            return DoFunc<Tuple<T1, T2>, TResult>(t => func(t.Item1, t.Item2));
        }

        public static void Do(Type serviceType, Action<object> action)
        {
            _doActionMethod.MakeGenericMethod(serviceType).Invoke(null, new object[] { action });
        }

        public static TResult Do<TResult>(Type serviceType, Func<object, TResult> action)
        {
            return (TResult)_doFuncMethod.MakeGenericMethod(serviceType, typeof(TResult)).Invoke(null, new object[] { action });
        }

        private static void DoAction<T>(Action<T> action)
        {
            DoFunc((T service) =>
            {
                action(service);
                return 0;
            });
        }

        private static TResult DoFunc<T, TResult>(Func<T, TResult> func)
        {
            var parameter = new NamedScopeParameter($"Call scope for {_iocType.FullName}");
            using (parameter.Scope)
            using (var proxy = _kernel.Get<DisposeNotifyingProxy<T>>(parameter))
                return func(proxy.Service);
        }

        public sealed class DisposeNotifyingProxy<T> : DisposeNotifyingObject
        {
            public DisposeNotifyingProxy(T service)
            {
                Service = service;
            }

            public T Service { get; }
        }
    }
}
