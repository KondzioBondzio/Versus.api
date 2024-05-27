using AutoMapper;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Mapping.Relationships;

public class RelationshipResponseMapper : Profile
{
    public RelationshipResponseMapper()
    {
        CreateMap<UserRelationship, RelationshipResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .AfterMap((src, dest, context) =>
            {
                var userId = (Guid)context.Items["userId"];
                var mappingAction = new RelationshipMappingAction(userId);
                mappingAction.Process(src, dest, context);
            });
    }
}