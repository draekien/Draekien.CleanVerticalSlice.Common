using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Draekien.CleanVerticalSlice.Common.Application.Mappings;

namespace Draekien.CleanVerticalSlice.Common.TestUtils.Mappings
{
    public class AutoMapperTestingProfile : Profile
    {
        public AutoMapperTestingProfile(Assembly assembly)
        {
            ApplyMappingsFromAssembly(assembly);
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                                .Where(t => t.GetInterfaces().Any(i =>
                                                                      i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                              ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
