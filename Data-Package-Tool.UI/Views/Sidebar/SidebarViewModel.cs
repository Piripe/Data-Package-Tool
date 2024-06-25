using Avalonia;
using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Sidebar
{
    public class SidebarViewModel : ReactiveObject
    {
        public DataPackage Package { get; set; } = new DataPackage();
        public IImage Avatar { get; set; } = User.GetDefaultAvatarBitmap(0);
        public string Username => Package.User.GetUsername();
        public ObservableCollection<NavItemModel> NavItems { get; } = new ObservableCollection<NavItemModel>([
                new NavItemModel() {Path = (Application.Current!.TryGetResource("HomeIcon",Application.Current.ActualThemeVariant, out var homeIcon) ? homeIcon : throw new Exception()) as StreamGeometry, Tooltip="Overview"}
            ]);



        public SidebarViewModel()
        {

            Init();
        }
        public SidebarViewModel(DataPackage package)
        {
            Package = package;
            Init();
        }
        private void Init() {
            Task<IImage> avatarTask = Package.User.GetAvatar();
            avatarTask.Wait(); // Supposed to be instant
            Avatar = avatarTask.Result;
            Task dataTask = InitData();
        }
        private async Task InitData()
        {
            foreach (var guild in Package.JoinedGuilds) {
                try
                {
                    NavItems.Add(new NavItemModel()
                    {
                        Image = await guild.GetIcon(),
                        Tooltip = await guild.GetName(),
                    });
                } catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.ToString()}");
                }

            }
        }
    }
}
