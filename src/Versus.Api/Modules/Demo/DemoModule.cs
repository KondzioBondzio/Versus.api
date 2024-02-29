namespace Versus.Api.Modules.Demo;

[EndpointGroupName("Demo")]
public class DemoModule : IModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup(string.Empty)
            .RequireAuthorization();

        routeGroup.MapGet("/test", AuthorizeTestHandler.Handle);

        return routeGroup;
    }
}
