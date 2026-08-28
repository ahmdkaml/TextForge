using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextForge.Core.Modules;

namespace TextForge.Core;

public static class DocumentOperations
{
    /// <summary>
    /// Duplicates the target module and inserts it directly after the target in its parent collection
    /// or in the root collection if it is a root module.
    /// </summary>
    public static Module Duplicate(IList<Module> rootModules, Module target)
    {
        var clone = target.Clone();

        if (target.Parent is not null)
        {
            var parentCollection = target.Parent.SubModules;
            int index = parentCollection.IndexOf(target);
            if (index >= 0)
            {
                parentCollection.Insert(index + 1, clone);
                return clone;
            }
        }

        // Target is a root-level module
        int rootIndex = rootModules.IndexOf(target);
        if (rootIndex >= 0)
        {
            rootModules.Insert(rootIndex + 1, clone);
        }
        else
        {
            rootModules.Add(clone);
        }

        return clone;
    }
}
