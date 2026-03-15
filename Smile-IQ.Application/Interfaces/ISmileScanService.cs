using Smile_IQ.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.Interfaces
{
    public interface ISmileScanService
    {
        Task<DTOSmileScanResponse> CreateAsync(DTOCreateSmileScanRequest request);
        Task<List<DTOSmileScanResponse>> GetByExternalPatientIdAsync(int externalPatientId);
    }
}
