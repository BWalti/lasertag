using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace AdminApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProvisioningController : ControllerBase
{
    readonly IClusterClient _clusterClient;

    public ProvisioningController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [HttpGet]
    public Task<ProvisioningModel> Provision([FromQuery] Guid id)
    {
        return Task.FromResult(new ProvisioningModel
        {
            LasertagSetId = id
        });
    }
}


public class ProvisioningModel
{
    public Guid LasertagSetId { get; set; }
}