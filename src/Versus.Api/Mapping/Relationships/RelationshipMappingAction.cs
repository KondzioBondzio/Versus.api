using AutoMapper;
using Versus.Api.Entities;
using Versus.Shared.Relationships;

namespace Versus.Api.Mapping.Relationships;

public class RelationshipMappingAction : IMappingAction<UserRelationship, RelationshipResponse>
{
    private readonly Guid _userId;

    public RelationshipMappingAction(Guid userId)
    {
        _userId = userId;
    }
    
    public void Process(UserRelationship source, RelationshipResponse destination, ResolutionContext context)
    {
        destination.UserId = source.UserId == _userId ? source.RelatedUserId : source.UserId;
        destination.Direction = source.UserId == _userId
            ? RelationshipResponse.RelationshipDirection.Outgoing
            : RelationshipResponse.RelationshipDirection.Incoming;
    }
}