using FlexBus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// This code is adapted from 
/// </summary>
namespace FlexBus.Host
{
    public static class Resolver
    {
        static readonly List<Assembly> assemblies;
        const string PLUGINS_PATH_KEY = "FLEXBUS_PLUGINS_PATH";
        const string DEFAULT_PLUGINS_PATH = @"plugins";

        static string PluginsPath
        {
            get
            {
                return Environment.GetEnvironmentVariable(PLUGINS_PATH_KEY) ?? DEFAULT_PLUGINS_PATH;
            }
        }
        static Resolver()
        {
            var codebase = typeof(Resolver).Assembly.CodeBase.Remove(0, 8);
            var currentDirectory = Path.Combine(Path.GetDirectoryName(codebase), PluginsPath);
            if (Directory.Exists(currentDirectory))
            {
                assemblies = Directory.GetFiles(currentDirectory, "*.dll")
                    .Select(Assembly.LoadFrom)
                    .Where(ReferencesShared)
                    .ToList();
            }else
            {
                assemblies = new List<Assembly>();
            }
        }

        static bool ReferencesShared(Assembly assembly)
        {
            var sharedAssembly = typeof(FlexEndpointAttribute).Assembly.GetName().Name;
            return assembly.GetReferencedAssemblies()
                .Any(name => name.Name == sharedAssembly);
        }

        public static List<TAttribute> FindByAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return assemblies
                    .SelectMany(a => a.ExportedTypes)
                    .Where(a => a.GetCustomAttribute<TAttribute>() != null)
                    .Select(a => a.GetCustomAttribute<TAttribute>())
                    .ToList();
        }

        public static List<Type> FindByInterface<TInterface>()
        {
            return assemblies
                        .SelectMany(a => a.GetImplementationTypes<TInterface>())
                        .ToList();
        }

        public static async Task Execute<T>(Func<T, Task> action)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetImplementationTypes<T>())
                {
                    var instance = (T)Activator.CreateInstance(type);
                    await action(instance)
                        .ConfigureAwait(false);
                }
            }
        }

        static IEnumerable<Type> GetImplementationTypes<TInterface>(this Assembly assembly)
        {
            return assembly.GetTypes().Where(IsConcreteClass<TInterface>);
        }

        static bool IsConcreteClass<TInterface>(Type type)
        {
            return typeof(TInterface).IsAssignableFrom(type) &&
                   !type.IsAbstract &&
                   !type.ContainsGenericParameters &&
                   type.IsClass;
        }
    }
}