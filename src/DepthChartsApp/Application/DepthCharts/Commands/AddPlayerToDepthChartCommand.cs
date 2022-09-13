using DepthChartsApp.Application.Common.Interfaces;
using DepthChartsApp.Domain;
using DepthChartsApp.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DepthChartsApp.Application.DepthCharts.Commands;

public sealed class AddPlayerToDepthChartCommand : IRequest<Result<Unit>>
{
    private string PositionName { get; }
    private int PlayerNumber { get; }
    private string PlayerName { get; }
    private int? PositionDepth { get; }

    public AddPlayerToDepthChartCommand(string? positionName, int playerNumber, string? playerName, int? positionDepth)
    {
        PositionName = positionName ?? string.Empty;
        PlayerNumber = playerNumber;
        PlayerName = playerName ?? string.Empty;
        PositionDepth = positionDepth;
    }

    public sealed class Handler : IRequestHandler<AddPlayerToDepthChartCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _dbContext;

        public Handler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Unit>> Handle(
            AddPlayerToDepthChartCommand request,
            CancellationToken cancellationToken)
        {
            var depthChart = await _dbContext
                .DepthCharts
                .Include(x => x.Positions)
                .ThenInclude(x => x.Players)
                .SingleOrDefaultAsync(cancellationToken);

            if (depthChart is null)
            {
                depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
                _dbContext.DepthCharts.Add(depthChart);
            }

            var result = depthChart.CanAddPlayerToPosition(
                request.PositionName,
                request.PlayerNumber,
                request.PlayerName,
                request.PositionDepth);

            if (result.IsFailure) return Result.Failure<Unit>(result.Errors);

            depthChart.AddPlayerToPosition(
                request.PositionName,
                request.PlayerNumber,
                request.PlayerName,
                request.PositionDepth);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(Unit.Value);
        }
    }
}