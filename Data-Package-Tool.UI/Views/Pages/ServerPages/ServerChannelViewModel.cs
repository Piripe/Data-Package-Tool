using DataPackageTool.Core.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataPackageTool.UI.Views.Pages.ServerPages
{
    public class ServerChannelViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "channel";
        public ServerChannelViewModel()
        {
            Init();
        }
        public ServerChannelViewModel(Channel channel)
        {
            Init();
        }
        private void Init()
        {
        }
    }
}
