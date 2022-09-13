using System.Reflection;
using DepthChartsApp.Application.Common.Interfaces;
using DepthChartsApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace DepthChartsApp.Infrastructure;

/// <summary>
/// TODO: Move all "Infrastructure" to another project, that way application layer won't have access to EF DbContext 
/// </summary>

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<DepthChart> DepthCharts => Set<DepthChart>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}