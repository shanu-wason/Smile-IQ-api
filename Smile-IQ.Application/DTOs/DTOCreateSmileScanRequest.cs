using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.DTOs
{
    public class DTOCreateSmileScanRequest
    {
        public int ExternalPatientId { get; set; }

        public IFormFile Image { get; set; }
    }
}
