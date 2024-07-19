using Avalonia;
using Avalonia.Controls;
using System;

namespace DataPackageTool.UI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true); // Collect garbage from Avalonia and data package loading
    }
}
