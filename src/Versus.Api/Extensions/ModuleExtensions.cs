using System.Reflection;
using Versus.Api.Endpoints;

namespace Versus.Api.Extensions;

public static class ModuleExtensions
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            module.RegisterServices(services);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            string groupName = GetGroupName(module);
            module.MapEndpoints(builder.MapGroup("api").MapGroup(groupName).WithTags(groupName));
        }

        return builder;
    }

    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }

    private static string GetGroupName(IModule module)
    {
        var type = module.GetType();
        var groupNameAttribute = type.GetCustomAttribute<EndpointGroupNameAttribute>();
        string groupName = groupNameAttribute?.EndpointGroupName ?? type.Name;
        return groupName.Replace("Module", string.Empty);
    }
}