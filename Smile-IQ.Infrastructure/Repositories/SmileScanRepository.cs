using Microsoft.EntityFrameworkCore;
using Polly;
using Smile_IQ.Application.DTOs;
using Smile_IQ.Application.Interfaces;
using Smile_IQ.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Infrastructure.Repositories
{
    public class SmileScanRepository : ISmileScanRepository
    {
        private readonly AppDbContext _dbContext;

        public SmileScanRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddAsync(SmileScan scan)
        {
            await _dbContext.SmileScans.AddAsync(scan);
        }

        public Task<DTOSmileScanResponse> CreateAsync(DTOCreateSmileScanRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SmileScan>> GetByExternalPatientIdAsync(int externalPatientId)
        {
            return await _dbContext.SmileScans
                .Where(x => x.ExternalPatientId == externalPatientId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
