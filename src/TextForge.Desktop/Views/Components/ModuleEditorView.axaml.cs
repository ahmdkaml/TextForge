using Avalonia.Controls;
using Avalonia.Input;
using TextForge.Desktop.ViewModels;

namespace TextForge.Desktop.Views.Components;

public partial class ModuleEditorView : UserControl
{
    public ModuleEditorView()
    {
        InitializeComponent();
    }

    // 1. Root Selection Context (MainViewModel)
    private void DocumentRootCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled) return; // Child card already handled the click

        if (DataContext is MainViewModel mainVm)
        {
            mainVm.SelectRootCommand.Execute(null);
            e.Handled = true;
        }
    }

    // 2. Module Selection Context (ModuleViewModel)
    private void ModuleCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control { DataContext: ModuleViewModel moduleVm })
        {
            moduleVm.SelectCommand.Execute(null);
        }
    }
    private void StopPropagation_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
    }
}