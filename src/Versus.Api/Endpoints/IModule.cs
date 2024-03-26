namespace Versus.Api.Endpoints;

public interface IModule
{
    IServiceCollection RegisterServices(IServiceCollection services);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder);
}