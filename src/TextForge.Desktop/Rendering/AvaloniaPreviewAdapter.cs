using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using TextForge.Core.Engine;
using TextForge.Core.Modules;

namespace TextForge.Desktop.Rendering;

public class AvaloniaPreviewAdapter : IRenderTarget<Control>
{
    public Control Render(RenderTree tree)
    {
        var rootPanel = new StackPanel
        {
            Spacing = 6
        };

        foreach (var node in tree.RootNodes)
        {
            rootPanel.Children.Add(RenderNode(node, 0));
        }

        return rootPanel;
    }

    private Control RenderNode(RenderNode node, int indentLevel)
    {
        var container = new StackPanel
        {
            Margin = new Thickness(
                node.Layout.MarginLeft + (indentLevel * 18),
                node.Layout.MarginTop,
                node.Layout.MarginRight,
                node.Layout.MarginBottom),
            Spacing = node.Layout.Spacing
        };

        if (!string.IsNullOrEmpty(node.Content))
        {
            var textBlock = new TextBlock
            {
                Text = node.Content,
                TextWrapping = TextWrapping.Wrap,
                FontSize = ResolveFontSize(node, indentLevel),
                FontWeight = node.Features.FontWeight == ModuleFontWeight.Bold ? FontWeight.Bold : FontWeight.Normal,
                FontStyle = node.Features.Italic ? FontStyle.Italic : FontStyle.Normal,
                TextAlignment = ResolveAlignment(node.Layout.Alignment),
                Foreground = !string.IsNullOrEmpty(node.Features.Color)
                    ? Brush.Parse(node.Features.Color)
                    : Brush.Parse("#1F2937")
            };

            // Callout / Highlight marker box
            if (!string.IsNullOrEmpty(node.Features.HighlightMarker))
            {
                var border = new Border
                {
                    Background = Brush.Parse(node.Features.HighlightMarker),
                    BorderBrush = Brush.Parse("#F59E0B"),
                    BorderThickness = new Thickness(4, 0, 0, 0),
                    CornerRadius = new CornerRadius(0, 4, 4, 0),
                    Padding = new Thickness(
                        node.Layout.PaddingLeft > 0 ? node.Layout.PaddingLeft : 12,
                        node.Layout.PaddingTop > 0 ? node.Layout.PaddingTop : 8,
                        node.Layout.PaddingRight > 0 ? node.Layout.PaddingRight : 12,
                        node.Layout.PaddingBottom > 0 ? node.Layout.PaddingBottom : 8),
                    Child = textBlock
                };
                container.Children.Add(border);
            }
            else
            {
                container.Children.Add(textBlock);
            }
        }

        foreach (var child in node.Children)
        {
            container.Children.Add(RenderNode(child, indentLevel + 1));
        }

        return container;
    }

    private static double ResolveFontSize(RenderNode node, int indentLevel)
    {
        if (node.Type == ModuleType.Section && indentLevel == 0)
        {
            return 20;
        }

        return node.Features.FontWeight == ModuleFontWeight.Bold ? 16 : 14;
    }

    private static Avalonia.Media.TextAlignment ResolveAlignment(Core.Engine.TextAlignment alignment) =>
        alignment switch
        {
            Core.Engine.TextAlignment.Center => Avalonia.Media.TextAlignment.Center,
            Core.Engine.TextAlignment.Right => Avalonia.Media.TextAlignment.Right,
            Core.Engine.TextAlignment.Justify => Avalonia.Media.TextAlignment.Justify,
            _ => Avalonia.Media.TextAlignment.Left
        };
}
