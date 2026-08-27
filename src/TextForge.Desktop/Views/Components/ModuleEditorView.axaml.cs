using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TextForge.Core.Modules;

namespace TextForge.Desktop.Views.Components;

public partial class ModuleEditorView : UserControl
{
    public event EventHandler<Module>? ModuleMoveUpRequested;
    public event EventHandler<Module>? ModuleMoveDownRequested;
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
        }
    }

    private void MoveDownButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleMoveDownRequested?.Invoke(this, module);
        }
    }

    private void DeleteModuleButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleDeleteRequested?.Invoke(this, module);
        }
    }
}
