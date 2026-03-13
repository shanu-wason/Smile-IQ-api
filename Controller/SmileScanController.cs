using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Smile_IQ.Application.DTOs;
using Smile_IQ.Application.Interfaces;

namespace Smile_IQ_api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmileScansController : ControllerBase
    {
        private readonly ISmileScanService _smileService;

        public SmileScansController(ISmileScanService service)
        {
            _smileService = service;
        }

        [EnableRateLimiting("SmilePolicy")]
        [HttpPost]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> UploadSmileImage ([FromForm] DTOCreateSmileScanRequest scanRequest)
        {
            if (scanRequest is null)
                return BadRequest("The request cannot be null.");

            if (scanRequest.ExternalPatientId <= 0)
                return BadRequest("ExternalPatientId must be a positive number.");

            if (scanRequest.Image is null || scanRequest.Image.Length == 0)
                return BadRequest("An image file is required.");

            var result = await _smileService.UploadSmileImageAsync(scanRequest);
            return Ok(result);
        }

        [HttpGet("{externalPatientId}")]
        public async Task<IActionResult> GetPatientSmile (int externalPatientId)
        {
            var result = await _smileService.GetByExternalPatientIdAsync(externalPatientId);
            return Ok(result);
        }
    }
}
