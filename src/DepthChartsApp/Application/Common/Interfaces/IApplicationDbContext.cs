using DepthChartsApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace DepthChartsApp.Application.Common.Interfaces;

/// <summary>
/// Commands and Queries will use this interface.
/// Note that at the moment the actual implementation of "ApplicationDbContext" is visible, that would not happen if "Infrastructure" was in a separated project
/// </summary>
public interface IApplicationDbContext
{
    DbSet<DepthChart> DepthCharts { get; }
    DbSet<Position> Positions { get; }
    DbSet<Player> Players { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}