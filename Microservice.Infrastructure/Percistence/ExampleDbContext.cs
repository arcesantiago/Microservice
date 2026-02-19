using Microservice.Application.Contracts.Persistence;
using Microservice.Domain.Common;
using Microservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Infrastructure.Percistence
{
    public class ExampleDbContext(DbContextOptions<ExampleDbContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<Example>? Examples { get; set; }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseDomainModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
