using Wolverine.Http;

namespace Admin.Api.Models;

public static class Response
{
    public record GameCreatedResponse(Guid Id) : CreationResponse(ApiRouteBuilder.GetGameById(Id.ToString()));
    public record GameStartedResponse(Guid Id, Guid ServerId);
    public record RegisterGameSetResponse(Guid ServerId, int Id);
    public record ServerCreatedResponse(Guid Id, string Name) : CreationResponse(ApiRouteBuilder.GetServerById(Id.ToString()));
}
