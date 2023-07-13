using Wolverine.Http;

namespace Admin.Api;

public record ServerCreatedResponse(Guid Id, string Name) : CreationResponse(ApiRouteBuilder.GetServerById(Id.ToString()));