using DepthChartsApp.Domain.Common;

namespace DepthChartsApp.Domain;

public sealed class Player : BaseEntity
{
    public int Number { get; }
    public string Name { get; }
    public int PositionDepth { get; private set; }

    public Position Position { get; }

    protected Player()
    {
    }

    internal Player(int number, string name, int positionDepth)
        : this()
    {
        Number = number;
        Name = name;
        PositionDepth = positionDepth;
    }

    internal void MoveToNextPosition()
    {
        PositionDepth += 1;
    }

    internal void MoveToPreviousPosition()
    {
        PositionDepth -= 1;
    }
}