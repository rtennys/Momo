using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Momo.Common
{
    public interface IDynamicTypeBuilderFactory
    {
        IDynamicTypeBuilder CreateTypeBuilder(string typeName);
        IDynamicTypeBuilder CreateTypeBuilder(string typeName, string assemblyName);
    }

    public interface IDynamicTypeBuilder
    {
        Type CreateType();
        IDynamicTypeBuilder AddInterface<T>();
        IDynamicTypeBuilder AddProperty<T>(string propertyName);
        IDynamicTypeBuilder AddProperty(Type propertyType, string propertyName);
        IDynamicTypeBuilder AddProperty(Type propertyType, string propertyName, Action<PropertyBuilder> callback);
    }

    public class DynamicTypeBuilderFactory : IDynamicTypeBuilderFactory
    {
        public IDynamicTypeBuilder CreateTypeBuilder(string typeName)
        {
            return CreateTypeBuilder(typeName, "_DynamicTypeAssembly");
        }

        public IDynamicTypeBuilder CreateTypeBuilder(string typeName, string assemblyName)
        {
            return new Inner(typeName, assemblyName);
        }

        private class Inner : IDynamicTypeBuilder
        {
            public Inner(string typeName, string assemblyName)
            {
                _typeBuilder = AppDomain.CurrentDomain
                    .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                    .DefineDynamicModule(assemblyName)
                    .DefineType(typeName, TypeAttributes.Public);

                _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            }

            private readonly TypeBuilder _typeBuilder;

            public Type CreateType()
            {
                return _typeBuilder.CreateType();
            }

            public IDynamicTypeBuilder AddInterface<T>()
            {
                _typeBuilder.AddInterfaceImplementation(typeof(T));
                return this;
            }

            public IDynamicTypeBuilder AddProperty<T>(string propertyName)
            {
                return AddProperty(typeof(T), propertyName);
            }

            public IDynamicTypeBuilder AddProperty(Type propertyType, string propertyName)
            {
                return AddProperty(propertyType, propertyName, null);
            }

            public IDynamicTypeBuilder AddProperty(Type propertyType, string propertyName, Action<PropertyBuilder> callback)
            {
                var property = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                var field = _typeBuilder.DefineField(string.Concat("_", propertyName), propertyType, FieldAttributes.Private);
                var getter = _typeBuilder.DefineMethod(string.Concat("get_", propertyName), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, propertyType, Type.EmptyTypes);
                var setter = _typeBuilder.DefineMethod(string.Concat("set_", propertyName), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, null, new[] { propertyType });

                var getterIL = getter.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, field);
                getterIL.Emit(OpCodes.Ret);

                var setterIL = setter.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, field);
                setterIL.Emit(OpCodes.Ret);

                property.SetGetMethod(getter);
                property.SetSetMethod(setter);

                if (callback != null) callback(property);

                return this;
            }
        }
    }
}
