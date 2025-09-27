using Microsoft.EntityFrameworkCore;
using VlcTracker.Service.Persistence.Entities;
using VlcTracker.Service.Persistence.Entities.Configurations;

namespace VlcTracker.Service.Persistence;

public class TrackingContext(DbContextOptions<TrackingContext> options) : DbContext(options)
{
    public DbSet<Scrobble> Scrobbles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new ScrobbleEntityConfiguration().Configure(modelBuilder.Entity<Scrobble>());

        base.OnModelCreating(modelBuilder);
    }
}
