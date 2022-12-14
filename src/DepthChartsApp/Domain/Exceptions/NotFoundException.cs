namespace DepthChartsApp.Domain.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"\"{name}\" with key: {key} was not found.")
    {
    }
}