using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using TextForge.Core;
using TextForge.Core.Modules;
using TextForge.Desktop.Rendering;
using TextForge.Desktop.Views.Components;
using Avalonia.Controls.Primitives;
using System.Linq;
using TextForge.Desktop.ViewModels;

namespace TextForge.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly AvaloniaPreviewAdapter _previewAdapter = new();
    // Safely casts DataContext to MainViewModel
    private MainViewModel? ViewModel => DataContext as MainViewModel;
    
    #region Lifecycle & Initialization

    public MainWindow()
    {
        InitializeComponent();

        // ONLY listen for DataContext assignment. Do not call Bind/Render here!
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        // Guard against null ViewModel or Document
        if (ViewModel?.CurrentDocument is null) return;

        // Subscribe preview re-rendering to document changes
        ViewModel.CurrentDocument.Changed += RenderCurrentPreview;

        RenderCurrentPreview();
    }
    
    private void ModuleEditor_RootSelected(object? sender, EventArgs e)
    {
        if (ViewModel?.CurrentDocument is null) return;

        ViewModel.CurrentDocument.SelectRoot();
    }
    
    private void ModuleEditor_ModuleMoveUpRequested(object? sender, Module module)
    {
        ViewModel?.CurrentDocument?.MoveModuleUp(module);
    }

    private void ModuleEditor_ModuleMoveDownRequested(object? sender, Module module)
    {
        ViewModel?.CurrentDocument?.MoveModuleDown(module);
    }

    private void ModuleEditor_ModuleDeleteRequested(object? sender, Module module)
    {
        ViewModel?.CurrentDocument?.RemoveModule(module);
    }

    // private void ModuleEditor_ModuleDuplicateRequested(Module module)
    // {
    //     ViewModel.CurrentDocument.DuplicateModule(module)
    // }

    private void ModuleEditor_ModuleDetachRequested(object? sender, Module module)
    {
        if (ViewModel?.CurrentDocument is not null)
        {
            ViewModel.CurrentDocument.DetachModule(module);
        }
    }

    #endregion

    #region Document & Preview Synchronization

    private void RenderCurrentPreview()
    {
        if (ViewModel?.CurrentDocument is null) return;

        // Direct auto-generated field access on MainWindow partial class
        if (PreviewView?.PreviewHost is not null)
        {
            PreviewView.PreviewHost.Content = ViewModel.Engine.Render(
                ViewModel.CurrentDocument, 
                ViewModel.Template, 
                _previewAdapter
            );
        }
    }

    private void ModuleContent_TextChanged(object? sender, TextChangedEventArgs e)
    {
        ViewModel?.CurrentDocument?.NotifyChanged();
    }

    #endregion
    
}