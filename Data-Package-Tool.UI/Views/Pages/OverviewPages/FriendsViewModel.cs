using Avalonia.Media;
using DataPackageTool.Core.Models;
using DataPackageTool.Core;
using DataPackageTool.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages.OverviewPages
{
    public class FriendsViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "friends";
        public DataPackage Package { get; set; } = new DataPackage();
        public ObservableCollection<Channel> ChannelsDataGrid { get; set; } = null!;
        public FriendsViewModel()
        {
            Init();
        }
        public FriendsViewModel(DataPackage package)
        {
            Package = package;
            Init();
        }
        private void Init()
        {
            HashSet<Channel> channelsDataGrid = Package.Channels.Where(x => x.IsDM()).ToHashSet();
            channelsDataGrid.UnionWith(Package.UsersMap.Values.Where(x => channelsDataGrid.All(y => y.DMRecipientId != x.Id)).Select(x => new Channel() { DMRecipientId = x.Id }).ToHashSet());
            ChannelsDataGrid = new ObservableCollection<Channel>(channelsDataGrid.OrderByDescending(x=>x.Messages.Count));
        }
    }
}
