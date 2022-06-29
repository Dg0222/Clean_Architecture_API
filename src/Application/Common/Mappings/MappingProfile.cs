using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CleanArchitecture.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        AutoDetectMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void AutoDetectMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(x => x.GetInterfaces().Any(z => z.IsGenericType &&
            z.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var method = type.GetMethod("Mapping")
                ?? type.GetInterface("IMapFrom`1")
                    ?.GetMethod("Mapping");

            method?.Invoke(instance, new object[] { this });
        }
    }
}
