using DepthChartsApp.Application.Common.Interfaces;
using DepthChartsApp.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DepthChartsApp.Application.DepthCharts.Queries;

public sealed class FullDepthChartResponseDto
{
    public Guid DepthChartId { get; }
    public IEnumerable<string> Rows { get; }

    public FullDepthChartResponseDto(Guid depthChartId, IEnumerable<string> rows)
    {
        DepthChartId = depthChartId;
        Rows = rows;
    }
}

public sealed class GetFullDepthChartQuery : IRequest<Result<FullDepthChartResponseDto>>
{
    public sealed class Handler : IRequestHandler<GetFullDepthChartQuery, Result<FullDepthChartResponseDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public Handler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<FullDepthChartResponseDto>> Handle(GetFullDepthChartQuery request,
            CancellationToken cancellationToken)
        {
            var depthChart = await _dbContext
                .DepthCharts
                .Include(x => x.Positions)
                .ThenInclude(x => x.Players)
                .SingleOrDefaultAsync(cancellationToken);

            if (depthChart is null) return Result.Failure<FullDepthChartResponseDto>("Depth chart not found");

            // The code below could be converted to a LINQ expression, but I think it's cleaner this way

            var result = new List<string>();

            foreach (var position in depthChart.Positions)
            {
                var players = position
                    .Players
                    .OrderBy(p => p.PositionDepth)
                    .Select(p => $"(#{p.Number}, {p.Name})");
                
                result.Add($"{position.Name} - {string.Join(", ", players)}");
            }

            return Result.Success(new FullDepthChartResponseDto(depthChart.Id, result));
        }
    }
}