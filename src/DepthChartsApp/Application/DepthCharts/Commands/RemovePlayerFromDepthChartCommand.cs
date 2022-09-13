using DepthChartsApp.Application.Common.Interfaces;
using DepthChartsApp.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DepthChartsApp.Application.DepthCharts.Commands;

public sealed class RemovePlayerFromDepthChartCommand : IRequest<Result<string>>
{
    public string Position { get; }
    public int PlayerNumber { get; }

    public RemovePlayerFromDepthChartCommand(string? position, int playerNumber)
    {
        Position = position?.Trim() ?? string.Empty;
        PlayerNumber = playerNumber;
    }

    public sealed class Handler : IRequestHandler<RemovePlayerFromDepthChartCommand, Result<string>>
    {
        private readonly IApplicationDbContext _dbContext;

        public Handler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<string>> Handle(
            RemovePlayerFromDepthChartCommand request,
            CancellationToken cancellationToken)
        {
            var depthChart = await _dbContext
                .DepthCharts
                .Include(x => x.Positions)
                .ThenInclude(p => p.Players)
                .SingleOrDefaultAsync(cancellationToken);

            if (depthChart is null) return Result.Failure<string>("Depth chart not found");

            var result = depthChart.CanRemovePlayerFromPosition(request.Position, request.PlayerNumber);
            if (result.IsFailure) return Result.Failure<string>(result.Errors);

            var playerDetails = depthChart
                .Positions
                .Where(p => p.Name == request.Position.ToUpper())
                .SelectMany(p => p.Players)
                .Single(p => p.Number == request.PlayerNumber);

            depthChart.RemovePlayerFromPosition(request.Position, request.PlayerNumber);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success($"#{playerDetails.Number} - {playerDetails.Name}");
        }
    }
}