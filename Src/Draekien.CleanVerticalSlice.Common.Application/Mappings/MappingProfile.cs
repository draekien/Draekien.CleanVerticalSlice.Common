using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;

namespace Draekien.CleanVerticalSlice.Common.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssemblies(ConsumingApplicationAssemblies);
        }

        public static IEnumerable<Assembly> ConsumingApplicationAssemblies { get; set; }

        private void ApplyMappingsFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetExportedTypes()
                                    .Where(
                                        t => t.GetInterfaces()
                                              .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                                    )
                                    .ToList();

                foreach (var type in types)
                {
                    var instance = Activator.CreateInstance(type);

                    var methodInfo = type.GetMethod("Mapping")
                                  ?? type.GetInterface("IMapFrom`1")
                                         ?.GetMethod("Mapping");

                    methodInfo?.Invoke(instance, new object[] { this });
                }
            }
        }
    }
}
