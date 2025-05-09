using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace DataPackageTool.UI.Views.Pages.OverviewPages;

public partial class Notifications : ReactiveUserControl<NotificationsViewModel>
{
    public Notifications()
    {
        InitializeComponent();
    }
}