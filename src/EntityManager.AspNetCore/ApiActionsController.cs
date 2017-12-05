using Microsoft.AspNetCore.Mvc;

namespace EntityManager.AspNetCore
{
    /// <summary>
    /// A base class for an API controller with api/[controller]/[action] route.
    /// Use it when controller has actions with the same signature/http method.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public abstract class ApiActionsController : Controller { }
}
