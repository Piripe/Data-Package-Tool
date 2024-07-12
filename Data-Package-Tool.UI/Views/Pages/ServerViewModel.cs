using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using DataPackageTool.UI.Views.Pages.ServerPages;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages
{
    public class ServerViewModel : ReactiveObject, IRoutableViewModel, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "server";

        public Guild Guild { get; set; } = new Guild();
        public DataPackage Package { get; set; } = new DataPackage();
        private string? _name;
        public string? Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public ObservableCollection<NavItemModel> NavItems { get; } = new ObservableCollection<NavItemModel>([
                new NavItemModel() {Path = (Application.Current!.TryGetResource("HomeIcon",Application.Current.ActualThemeVariant, out var homeIcon) ? homeIcon : throw new Exception()) as StreamGeometry, Name="Overview", Link=new ServerOverviewViewModel()},
            ]);
        public ServerViewModel()
        {
            Init();
        }
        public ServerViewModel(Guild guild)
        {
            Guild = guild;
            Package = guild.DataPackage??Package;
            Init();
        }
        private void Init()
        {
            Name = Guild.Name ?? Guild.Id;
            UpdateChannels();
        }
        public async Task InitData()
        {
            if (Guild.Name == null || Guild.Icon == null)
            {
                int lastChannelLength = Guild.Channels.Count;
                Name = await Guild.GetNameAsync(Core.Enums.DataSourceUsability.Manual);
                await Guild.GetIconAsync(Core.Enums.DataSourceUsability.Manual);
                if (Guild.Channels.Count != lastChannelLength)
                {
                    UpdateChannels();
                }
            }
            else
            {
                Name = Guild.Name;
            }
        }
        private void UpdateChannels()
        {
            NavItems.Remove(NavItems.Skip(1)); // Remove all items except the default one
            NavItems.AddRange(Guild.Channels.Select(x => {
                IRoutableViewModel? LinkGetter() => new ServerChannelViewModel(x);
                return new NavItemModel() { Path = Constants.ChannelIcons[x.Type], Name = x.Name, LinkGetter = LinkGetter };
            }));
        }
        
    }
}
