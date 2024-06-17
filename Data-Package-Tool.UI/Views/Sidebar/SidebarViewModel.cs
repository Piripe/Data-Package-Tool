using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Sidebar
{
    public class SidebarViewModel : ViewModelBase
    {
        public DataPackage Package { get; set; } = new DataPackage();
        public IImage Avatar { get; set; } = DUser.GetDefaultAvatarBitmap(0);
        public string Username => Package.User.GetUsername();

        public SidebarViewModel()
        {
            GetAvatar();
        }
        public SidebarViewModel(DataPackage package)
        {
            Package = package;
            GetAvatar();
        }
        private void GetAvatar() {
            Task<IImage> avatarTask = Package.User.GetAvatar();
            avatarTask.Wait(); // Supposed to be instant
            Avatar = avatarTask.Result;
        }
    }
}
