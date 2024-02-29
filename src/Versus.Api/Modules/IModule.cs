namespace Versus.Api.Modules;

public interface IModule
{
    IServiceCollection RegisterServices(IServiceCollection services);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder);
}
