using System.Reflection;
using Versus.Api.Endpoints;

namespace Versus.Api.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        var endpointGroups = DiscoverEndpointGroups();
        foreach (var group in endpointGroups)
        {
            var groupBuilder = builder
                .MapGroup("api")
                .MapGroup(group.Key)
                .WithTags(group.Key);
            foreach (var endpoint in group.Value)
            {
                groupBuilder.MapEndpoint(endpoint);
            }
        }

        return builder;
    }

    private static IDictionary<string, Type[]> DiscoverEndpointGroups()
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IEndpoint)))
            .GroupBy(x => x.Namespace?.Split('.').LastOrDefault() ?? string.Empty)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder builder)
        where TEndpoint : IEndpoint
    {
        MapEndpoint(builder, typeof(TEndpoint));
        return builder;
    }

    private static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder builder, Type endpointType)
    {
        endpointType.GetMethod(nameof(IEndpoint.Map))!.Invoke(null, [builder]);
        return builder;
    }
}