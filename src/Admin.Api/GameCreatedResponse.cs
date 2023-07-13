using Wolverine.Http;

namespace Admin.Api;

public record GameCreatedResponse(Guid Id) : CreationResponse(ApiRouteBuilder.GetGameById(Id.ToString()));