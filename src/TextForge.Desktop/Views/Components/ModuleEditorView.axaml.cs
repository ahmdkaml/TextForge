using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TextForge.Core.Modules;

namespace TextForge.Desktop.Views.Components;

/// <summary>
/// Module editor accordion tree presenting root and nested document modules.
/// </summary>
public partial class ModuleEditorView : UserControl
{
    /// <summary>
    /// Raised when a user requests moving a module up within its level.
    /// </summary>
    public event EventHandler<Module>? ModuleMoveUpRequested;

    /// <summary>
    /// Raised when a user requests moving a module down within its level.
    /// </summary>
    public event EventHandler<Module>? ModuleMoveDownRequested;

    /// <summary>
    /// Raised when a user clicks the delete button for a specific module node.
    /// </summary>
    public event EventHandler<Module>? ModuleDeleteRequested;

    public ModuleEditorView()
    {
        InitializeComponent();
    }

    private void MoveUpButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleMoveUpRequested?.Invoke(this, module);
            e.Handled = true;
        }
    }

    private void MoveDownButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleMoveDownRequested?.Invoke(this, module);
            e.Handled = true;
        }
    }

    private void DeleteModuleButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleDeleteRequested?.Invoke(this, module);
            e.Handled = true;
        }
    }
}
