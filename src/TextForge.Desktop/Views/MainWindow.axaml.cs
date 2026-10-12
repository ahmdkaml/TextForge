using System;
using Avalonia.Controls;
using TextForge.Desktop.Rendering;
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