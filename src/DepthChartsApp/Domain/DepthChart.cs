using DepthChartsApp.Domain.Common;

namespace DepthChartsApp.Domain;

/// <summary>
/// Why private setters ? By having them we can better control how this object is mutated and we move away from "anemic entities"
/// Methods are also better encapsulated and more declarative, better explaining what they do
/// Unit tests also become easier with this approach, because we can test our domain logic with no need of mock objects
/// Note that collections also is not exposed to the "external world", so they can't be mutated
/// </summary>
public sealed class DepthChart : BaseEntity
{
    public Guid TeamId { get; }

    private readonly List<Position> _positions = new();
    public IReadOnlyList<Position> Positions => _positions.ToList();

    protected DepthChart()
    {
    }

    private DepthChart(Guid teamId) : this()
    {
        TeamId = teamId;
    }

    public static Result<DepthChart> NewDepthChartResult(Guid teamId)
    {
        if (teamId == Guid.Empty)
            return Result.Failure<DepthChart>("Team not provided");

        return Result.Success(new DepthChart(teamId));
    }

    public Result CanAddPlayerToPosition(
        string? positionName,
        int playerNumber,
        string playerName,
        int? playerPositionDepth = null)
    {
        if (string.IsNullOrWhiteSpace(positionName))
            return Result.Failure($"{nameof(positionName)} is required");

        if (playerNumber <= 0)
            return Result.Failure($"The provided number '{playerNumber}' is invalid");

        if (string.IsNullOrWhiteSpace(playerName))
            return Result.Failure($"{nameof(playerName)} is required");

        if (playerPositionDepth is < 0)
            return Result.Failure($"The provided position depth '{playerPositionDepth.Value}' is invalid");

        var position = _positions.SingleOrDefault(p => p.Name == positionName.Trim().ToUpper());
        if (position != null && position.Players.Any(p => p.Number == playerNumber))
            return Result.Failure($"Player '{playerNumber}' has already been added to position '{positionName}'");

        return Result.Success();
    }

    public void AddPlayerToPosition(
        string? positionName,
        int playerNumber,
        string playerName,
        int? playerPositionDepth = null)
    {
        var result = CanAddPlayerToPosition(positionName, playerNumber, playerName, playerPositionDepth);

        if (result.IsFailure) throw new InvalidOperationException(string.Join(", ", result.Errors));

        var position = _positions.SingleOrDefault(p => p.Name == positionName?.Trim().ToUpper());

        if (position == null)
        {
            position = new Position(positionName!.Trim().ToUpper());
            _positions.Add(position);
        }

        position.AddPlayer(playerNumber, playerName, playerPositionDepth ?? position.Players.Count);
    }

    public Result CanRemovePlayerFromPosition(string? positionName, int playerNumber)
    {
        var position = _positions.SingleOrDefault(p => p.Name == positionName?.ToUpper());
        if (position is null)
            return Result.Failure($"Position '{positionName}' not found");

        var player = position.Players.SingleOrDefault(p => p.Number == playerNumber);
        if (player is null)
            return Result.Failure($"Player '{playerNumber}' not found");

        return Result.Success();
    }

    public void RemovePlayerFromPosition(string? positionName, int playerNumber)
    {
        var result = CanRemovePlayerFromPosition(positionName, playerNumber);

        if (result.IsFailure) throw new InvalidOperationException(string.Join(", ", result.Errors));

        var position = _positions.Find(p => p.Name == positionName?.Trim().ToUpper());
        var player = position!.Players.Single(p => p.Number == playerNumber);
        position.RemovePlayer(player.Id);
    }
}