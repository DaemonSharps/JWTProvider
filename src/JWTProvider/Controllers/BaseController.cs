using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JWTProvider.Controllers
{
    /// <summary>
    /// Базовый контроллер с общими для всех зависимостями
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IMediator Mediator => HttpContext.RequestServices.GetService<IMediator>();

        protected IMemoryCache Cache => HttpContext.RequestServices.GetService<IMemoryCache>();
    }
}
