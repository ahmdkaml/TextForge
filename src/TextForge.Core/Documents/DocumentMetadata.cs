using System;

namespace TextForge.Core.Documents;

public record DocumentMetadata
{
    public string Title { get; init; } = "Untitled Document";
    public string? Author { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedAt { get; init; } = DateTimeOffset.UtcNow;
    public int SchemaVersion { get; init; } = 1;
}
