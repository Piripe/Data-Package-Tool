using Avalonia;
using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Enums;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using DataPackageTool.UI.Views.Pages.OverviewPages;
using DataPackageTool.UI.Views.Pages.ServerPages;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages
{
    public class OverviewViewModel : ReactiveObject, IRoutableViewModel, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "overview";

        public DataPackage Package { get; set; } = new DataPackage();
        public ObservableCollection<NavItemModel> NavItems { get; } = new ObservableCollection<NavItemModel>();
        public OverviewViewModel()
        {
            Init();
        }
        public OverviewViewModel(DataPackage package)
        {
            Package = package;
            Init();
        }
        private void Init()
        {
            NavItems.AddRange([
                new NavItemModel() {Path = Constants.HomeIcon, Name="Profile", LinkGetter=()=>new ProfileViewModel(Package)},
                ]);
        }
    }
}
