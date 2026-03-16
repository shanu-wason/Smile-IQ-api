using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Smile_IQ_api.Controller
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/version")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version?.ToString() ?? "1.0.0.0";

            var versionInfo = new
            {
                api = "Smile IQ API",
                version = assemblyVersion,
                build = assemblyVersion,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                timestampUtc = DateTime.UtcNow
            };

            return Ok(versionInfo);
        }
    }
}

