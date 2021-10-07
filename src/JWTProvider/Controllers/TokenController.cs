using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController: ControllerBase
    {
        [HttpPost]
        [SwaggerOperation("Получить токен JWT")]
        public async Task<IActionResult> GetToken()
        {
            return Ok();
        }

        [HttpGet]
        [SwaggerOperation("Проверить актуальность токена")]
        public async Task<IActionResult> CheckToken()
        {
            return Ok();
        }

        [HttpPut]
        [SwaggerOperation("Разлогиниться")]
        public async Task<IActionResult> LogOut()
        {
            return Ok();
        }
    }
}
