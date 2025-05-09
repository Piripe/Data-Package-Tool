using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DataPackageTool.UI.Views.Pages.OverviewPages;

namespace DataPackageTool.UI.Views.Pages.OverviewPages;

public partial class Friends : ReactiveUserControl<FriendsViewModel>
{
    public Friends()
    {
        InitializeComponent();
    }
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
    }
}