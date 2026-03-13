using Microsoft.EntityFrameworkCore;
using Smile_IQ.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<SmileScan> SmileScans { get; set; }

        public DbSet<AIUsageLog> AIUsageLog { get; set; }
    }
}
