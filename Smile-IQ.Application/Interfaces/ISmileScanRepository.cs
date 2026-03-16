using Smile_IQ.Application.DTOs;
using Smile_IQ.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.Interfaces
{
    public interface ISmileScanRepository
    {
        Task AddAsync(SmileScan scan);
        Task<List<SmileScan>> GetByExternalPatientIdAsync(int externalPatientId);
        Task<DTOSmileScanResponse> UploadSmileImageAsync(DTOCreateSmileScanRequest request);
        Task SaveChangesAsync();
    }
}
