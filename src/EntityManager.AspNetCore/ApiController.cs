using Microsoft.AspNetCore.Mvc;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// A base class for an API controller with api/[controller] route.
    /// </summary>
    [Route("api/[controller]")]
    public abstract class ApiController : Controller { }
}
