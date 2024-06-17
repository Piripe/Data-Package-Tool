using DataPackageTool.Core;
using DataPackageTool.UI.ViewModels;
using DataPackageTool.UI.Views.Sidebar;

namespace DataPackageTool.UI.Views;

public class MainViewModel : ViewModelBase
{
    public DataPackage Package { get; set; } = new DataPackage();
    public string Username => Package.User.GetUsername();
    public SidebarViewModel Sidebar => new SidebarViewModel(Package);
}
