using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Enums;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public IImage Avatar { get; set; } = User.GetDefaultAvatarBitmap(0);
        public string Username => Package.User.GetUsername();
        public ObservableCollection<BadgeModel> Badges { get; set; } = new ObservableCollection<BadgeModel>();

        public OverviewViewModel()
        {
            Badges = new ObservableCollection<BadgeModel>(BadgeModel.GetUserBadges(Package.User, Package.CreationTime));
            Init();
        }
        public OverviewViewModel(DataPackage package)
        {
            Package = package;
            Badges = new ObservableCollection<BadgeModel>(BadgeModel.GetUserBadges(Package.User, Package.CreationTime));
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
