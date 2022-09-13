namespace DepthChartsApp.Domain.Common;

/// <summary>
/// "Aggregate Root" is a concept of domain driven design. This interface is only to mark any entity as an
/// aggregate root, it doesn't impose any constraints at the moment. We could introduce the "repository" design
/// pattern and only make aggregate roots visible through an Unit of work (another pattern) 
/// </summary>
public interface IAggregateRoot
{
}