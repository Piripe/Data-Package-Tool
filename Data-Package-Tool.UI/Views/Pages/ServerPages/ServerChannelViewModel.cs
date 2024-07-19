using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataPackageTool.UI.Views.Pages.ServerPages
{
    public class ServerChannelViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "channel";
        public Channel Channel { get; set; } = new Channel();
        public DataPackage Package { get; set; } = new DataPackage();
        public ObservableCollection<Message> Messages { get; set; } = new();
        public IImage? Avatar { get; set; }
        public string Username => Package.User.DisplayName;
        public ServerChannelViewModel()
        {
            Init();
        }
        public ServerChannelViewModel(Channel channel)
        {
            Channel = channel;
            Package = channel.DataPackage??Package;
            Init();
        }
        private void Init()
        {
            Task<IImage> avatarTask = Package.User.GetAvatar();
            avatarTask.Wait(); // Supposed to be instant
            Avatar = avatarTask.Result;

            Messages.AddRange(Channel.Messages);
        }
    }
}
