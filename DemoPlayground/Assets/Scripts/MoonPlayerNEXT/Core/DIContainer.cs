using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MoonXR.Player.Core.Attributes;

namespace MoonXR.Player.Core
{
    public class DIContainer
    {
        private readonly Dictionary<Type, Func<object>> registrations = new();
        private readonly Dictionary<Type, object> singletons = new();

        public void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            registrations[typeof(TInterface)] = () => new TImplementation();
        }

        public void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : class, TInterface, new()
        {
            registrations[typeof(TInterface)] = () =>
            {
                var type = typeof(TInterface);
                if (!singletons.ContainsKey(type))
                {
                    singletons[type] = new TImplementation();
                }
                return singletons[type];
            };
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            singletons[typeof(T)] = instance;
            registrations[typeof(T)] = () => instance;
        }

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (registrations.TryGetValue(type, out var factory))
            {
                return factory() as T;
            }

            return null;
        }

        public T Build<T>() where T : class, new()
        {
            var instance = new T();
            InjectDependencies(instance);
            return instance;
        }

        private void InjectDependencies(object target)
        {
            var type = target.GetType();
            var properties = type.GetProperties()
                .Where(p => p.CanWrite && p.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var property in properties)
            {
                var dep = Resolve(property.PropertyType);
                if (dep != null)
                {
                    property.SetValue(target, dep);
                }
            }
        }

        private object Resolve(Type type)
        {
            if (registrations.TryGetValue(type, out var factory))
            {
                return factory();
            }
            return null;
        }
    }
}