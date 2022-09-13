using DepthChartsApp.Application.Common.Interfaces;
using DepthChartsApp.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DepthChartsApp.Application.DepthCharts.Queries;

public sealed class GetBackupsQuery : IRequest<Result<IEnumerable<string>>>
{
    private string Position { get; }
    private int Player { get; }

    public GetBackupsQuery(string? position, int player)
    {
        Position = position?.Trim() ?? string.Empty;
        Player = player;
    }

    public sealed class Handler : IRequestHandler<GetBackupsQuery, Result<IEnumerable<string>>>
    {
        private readonly IApplicationDbContext _dbContext;

        public Handler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<IEnumerable<string>>> Handle(GetBackupsQuery request,
            CancellationToken cancellationToken)
        {
            var depthChart = await _dbContext
                .DepthCharts
                .Include(p => p.Positions)
                .ThenInclude(p => p.Players)
                .SingleOrDefaultAsync(cancellationToken);

            if (depthChart is null) return Result.Success<IEnumerable<string>>(Array.Empty<string>());

            var position = depthChart.Positions.SingleOrDefault(p => p.Name == request.Position.ToUpper());
            if (position is null) return Result.Success<IEnumerable<string>>(Array.Empty<string>());

            var player = position.Players.SingleOrDefault(p => p.Number == request.Player);
            if (player is null) return Result.Success<IEnumerable<string>>(Array.Empty<string>());

            return Result.Success(
                position.Players
                    .Where(p => p.PositionDepth > player.PositionDepth)
                    .OrderBy(p => p.PositionDepth)
                    .Select(p => $"#{p.Number} - {p.Name}")
            );
        }
    }
}