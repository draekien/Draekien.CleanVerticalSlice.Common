using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

namespace Draekien.CleanVerticalSlice.Common.Application.Mappings
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Creates a profile from classes that implement <see cref="IMapFrom{T}"/> in the calling assembly
        /// </summary>
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Creates a profile from classes that implement <see cref="IMapFrom{T}"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/></param>
        public MappingProfile(Assembly assembly)
        {
            ApplyMappingsFromAssembly(assembly);
        }

        /// <summary>
        /// Finds all classes that implement <see cref="IMapFrom{T}"/> in the specified assembly and registers their profiles
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/></param>
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type> types = assembly.GetExportedTypes()
                                       .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                                       .ToList();

            foreach (Type type in types)
            {
                object instance = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
