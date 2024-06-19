using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages
{
    public class OverviewViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "overview";

        public DataPackage Package { get; set; } = new DataPackage();
        public IImage Avatar { get; set; } = DUser.GetDefaultAvatarBitmap(0);
        public string Username => Package.User.GetUsername();
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
            Task<IImage> avatarTask = Package.User.GetAvatar();
            avatarTask.Wait(); // Supposed to be instant
            Avatar = avatarTask.Result;
        }
    }
}
