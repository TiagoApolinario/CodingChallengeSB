using DepthChartsApp.Domain.Common;
using DepthChartsApp.Domain.Exceptions;

namespace DepthChartsApp.Domain;

public sealed class Position : BaseEntity, IAggregateRoot
{
    public string Name { get; }
    public DepthChart DepthChart { get;  }
    private int NextAvailablePositionDepth => _players.Count;

    private readonly List<Player> _players = new();
    public IReadOnlyList<Player> Players => _players.ToList();

    protected Position()
    {
    }

    internal Position(string name) : this()
    {
        Name = name;
    }

    internal void AddPlayer(int playerNumber, string playerName, int playerPositionDepth)
    {
        var affectedPlayers = _players.Where(p => p.PositionDepth >= playerPositionDepth);

        foreach (var player in affectedPlayers)
        {
            player.MoveToNextPosition();
        }

        var checkedPositionDepth = IsInformedPositionDepthBiggerThanNextAvailablePosition(playerPositionDepth)
            ? NextAvailablePositionDepth
            : playerPositionDepth;

        _players.Add(new Player(playerNumber, playerName.Trim(), checkedPositionDepth));
    }

    internal void RemovePlayer(Guid playerId)
    {
        var selectedPlayer = _players.Find(p => p.Id == playerId);

        if (selectedPlayer == null) throw new NotFoundException("", "");

        _players.Remove(selectedPlayer);

        var affectedPlayers = _players.Where(p => p.PositionDepth >= selectedPlayer.PositionDepth);

        foreach (var player in affectedPlayers)
        {
            player.MoveToPreviousPosition();
        }
    }

    private bool IsInformedPositionDepthBiggerThanNextAvailablePosition(int informedPositionDepth)
        => informedPositionDepth + 1 > NextAvailablePositionDepth;
}