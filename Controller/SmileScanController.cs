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
        private readonly ISmileScanService _service;

        public SmileScansController(ISmileScanService service)
        {
            _service = service;
        }

        [EnableRateLimiting("SmilePolicy")]
        [HttpPost]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Create ([FromForm] DTOCreateSmileScanRequest request)
        {
            if (request == null)
                throw new ArgumentException("Request cannot be null.");

            if (request.ExternalPatientId <= 0)
                throw new ArgumentException("ExternalPatientId must be greater than 0.");

            if (request.Image == null || request.Image.Length == 0)
                throw new ArgumentException("Image is required.");

            var result = await _service.CreateAsync(request);

            return Ok(result);
        }

        [HttpGet("{externalPatientId}")]
        public async Task<IActionResult> GetByPatient(int externalPatientId)
        {
            var result = await _service.GetByExternalPatientIdAsync(externalPatientId);
            return Ok(result);
        }
    }
}
