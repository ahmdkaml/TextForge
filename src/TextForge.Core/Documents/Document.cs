using System;
using System.Collections.Generic;
using TextForge.Core.Modules;

namespace TextForge.Core.Documents;

/// <summary>
/// Root domain entity representing a document structure and its metadata.
/// </summary>
public class Document
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DocumentMetadata Metadata { get; set; } = new();
    public string TemplateName { get; set; } = "Default";
    public List<Module> Modules { get; init; } = [];

    /// <summary>
    /// Currently selected module in the active editing session, if any.
    /// </summary>
    public Module? SelectedModule { get; private set; }

    /// <summary>
    /// Raised whenever content, features, hierarchy, or selection within the document changes.
    /// Used by preview adapters and editors to trigger live re-renders.
    /// </summary>
    public event Action? Changed;

    public Document() { }

    public Document(string title, string templateName = "Default")
    {
        Metadata = new DocumentMetadata { Title = title };
        TemplateName = templateName;
    }

    public Document(DocumentMetadata metadata, IEnumerable<Module>? modules = null, string templateName = "Default")
    {
        Metadata = metadata;
        TemplateName = templateName;
        if (modules is not null)
        {
            Modules.AddRange(modules);
        }
    }

    #region Mutation & Event Dispatch

    /// <summary>
    /// Broadcasts a change notification to all registered UI and preview listeners.
    /// </summary>
    public void NotifyChanged()
    {
        Changed?.Invoke();
    }

    /// <summary>
    /// Appends a module to the root sequential list and returns the document for chaining.
    /// </summary>
    public Document AddModule(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);
        Modules.Add(module);
        NotifyChanged();
        return this;
    }

    /// <summary>
    /// Updates the active module selection, updating all node flags across the tree.
    /// </summary>
    public void SelectModule(Module? module)
    {
        if (SelectedModule == module) return;

        SetSelectionRecursive(Modules, false);
        SelectedModule = module;

        if (SelectedModule is not null)
        {
            SelectedModule.IsSelected = true;
        }

        NotifyChanged();
    }

    private static void SetSelectionRecursive(IEnumerable<Module> modules, bool isSelected)
    {
        foreach (var mod in modules)
        {
            mod.IsSelected = isSelected;
            if (mod.SubModules.Count > 0)
            {
                SetSelectionRecursive(mod.SubModules, isSelected);
            }
        }
    }

    /// <summary>
    /// Removes a module from the root document hierarchy or nested submodules.
    /// Clears selection if the removed module was selected, and broadcasts a change event.
    /// </summary>
    /// <param name="module">The module instance to remove.</param>
    /// <returns>True if the module was found and removed; otherwise false.</returns>
    public bool RemoveModule(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);

        var removed = RemoveRecursive(Modules, module);
        if (removed)
        {
            if (SelectedModule == module)
            {
                SelectedModule = null;
            }

            NotifyChanged();
        }

        return removed;
    }

    private static bool RemoveRecursive(IList<Module> list, Module target)
    {
        if (list.Remove(target))
        {
            return true;
        }

        foreach (var item in list)
        {
            if (item.SubModules.Count > 0 && RemoveRecursive(item.SubModules, target))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Reordering Operations

    /// <summary>
    /// Moves a module one position earlier within its current sibling list (root level or nested level).
    /// </summary>
    /// <param name="module">The module to move up.</param>
    /// <returns>True if the module was moved; false if it was already at the top or not found.</returns>
    public bool MoveModuleUp(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);

        if (TryMoveRelative(Modules, module, -1))
        {
            NotifyChanged();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Moves a module one position later within its current sibling list (root level or nested level).
    /// </summary>
    /// <param name="module">The module to move down.</param>
    /// <returns>True if the module was moved; false if it was already at the bottom or not found.</returns>
    public bool MoveModuleDown(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);

        if (TryMoveRelative(Modules, module, 1))
        {
            NotifyChanged();
            return true;
        }

        return false;
    }

    private static bool TryMoveRelative(IList<Module> list, Module target, int offset)
    {
        var index = list.IndexOf(target);
        if (index >= 0)
        {
            var newIndex = index + offset;
            if (newIndex >= 0 && newIndex < list.Count)
            {
                list.RemoveAt(index);
                list.Insert(newIndex, target);
                return true;
            }

            // Target is in this list but cannot move beyond bounds
            return false;
        }

        // Recursively search nested submodule sibling lists
        foreach (var item in list)
        {
            if (item.SubModules.Count > 0 && TryMoveRelative(item.SubModules, target, offset))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
