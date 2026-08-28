namespace TextForge.Core.Modules;

/// <summary>
/// Dictates whether a module participates in the document layout.
/// </summary>
public enum ModuleConnectionState
{
    /// <summary>
    /// The module is part of the layout and will be rendered.
    /// </summary>
    Connected,

    /// <summary>
    /// The module is preserved in the document model but excluded from layout and rendering.
    /// </summary>
    Detached
}
