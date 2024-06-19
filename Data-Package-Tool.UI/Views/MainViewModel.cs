using DataPackageTool.Core;
using DataPackageTool.UI.Views.Pages;
using DataPackageTool.UI.Views.Sidebar;
using ReactiveUI;

namespace DataPackageTool.UI.Views;

public class MainViewModel : ReactiveObject, IScreen
{
    public DataPackage Package { get; set; } = new DataPackage();
    public string Username => Package.User.GetUsername();
    public SidebarViewModel Sidebar => new SidebarViewModel(Package);
    public OverviewViewModel Overview => new OverviewViewModel(Package);

    public RoutingState Router { get; } = new RoutingState();

    public MainViewModel()
    {
    }
}
