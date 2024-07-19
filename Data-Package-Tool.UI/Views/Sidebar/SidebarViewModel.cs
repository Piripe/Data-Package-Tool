using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using DataPackageTool.UI.Views.Pages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataPackageTool.UI.Views.Sidebar
{
    public class SidebarViewModel : ReactiveObject
    {
        public DataPackage Package { get; set; } = new DataPackage();
        public IImage Avatar { get; set; } = User.GetDefaultAvatarBitmap(0);
        public string Username => Package.User.DisplayName;
        public ObservableCollection<NavItemModel> NavItems { get; } = new ObservableCollection<NavItemModel>([
                new NavItemModel() {Path = Application.Current!.FindResource(Application.Current!.ActualThemeVariant,"HomeIcon") as StreamGeometry, Tooltip="Overview"},
                new NavItemModel() {Path = Application.Current!.FindResource(Application.Current!.ActualThemeVariant,"SearchIcon") as StreamGeometry, Tooltip="Search"},
            ]);
        public RoutingState? Router { get; }

        public SidebarViewModel()
        {

            Init();
        }
        public SidebarViewModel(DataPackage package, RoutingState router, OverviewViewModel overview)
        {
            Package = package;
            Router = router;
            NavItems[0].Link = overview;
            NavItems[1].LinkGetter = ()=>new SearchViewModel(package);
            Init();
        }
        private void Init()
        {
            Task<IImage> avatarTask = Package.User.GetAvatar();
            avatarTask.Wait(); // Supposed to be instant
            Avatar = avatarTask.Result;
            InitData();
        }
        private void InitData()
        {
            Task partialGuilds = Package.GetPartialGuilds();
            foreach (var guild in Package.Guilds)
            {
                IImage? icon = guild.GetIcon();
                string? name = guild.Name;

                IRoutableViewModel? LinkGetter() => new ServerViewModel(guild);

                var model = new NavItemModel()
                {
                    Image = icon ?? guild.DefaultIcon(),
                    Tooltip = name ?? guild.Id,
                    LinkGetter = LinkGetter
                };

                NavItems.Add(model);

                if (icon == null || name == null)
                {
                    Task.Run(async () =>
                    {
                        await partialGuilds;
                        model.Image = await guild.GetIconAsync();
                        model.Tooltip = await guild.GetNameAsync();
                    });
                }

            }
        }
    }
}
