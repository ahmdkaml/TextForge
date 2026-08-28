namespace TextForge.Core.Models;

public interface IModule
{
    Guid Id { get; }
    IModule Clone();
}
