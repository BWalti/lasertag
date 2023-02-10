using JasperFx.CodeGeneration;
using Lamar;
using Wolverine.Configuration;
using Wolverine.Runtime.Handlers;

namespace Admin.Api;

public class MyMiddlewarePolicy : IHandlerPolicy
{
    public void Apply(HandlerGraph graph, GenerationRules rules, IContainer container)
    {

        // do something!
    }
}