using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitecture.Infrastructure.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IApplicationDbContext _context;

        public DatabaseHealthCheck(IApplicationDbContext context) {  _context = context; }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.TodoItems.FirstOrDefaultAsync(cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
