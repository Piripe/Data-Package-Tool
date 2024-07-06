using Avalonia.Controls;
using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages
{
    public class ServerViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "server";

        public Guild Guild { get; set; } = new Guild();
        public DataPackage Package { get; set; } = new DataPackage();
        private string? _name;
        public string? Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

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
        }
        public async Task InitData()
        {
            if (Guild.Name == null)
            {
                Name = await Guild.GetNameAsync(Core.Enums.DataSourceUsability.Manual);
            }
            else
            {
                Name = Guild.Name;
            }
        }
        
    }
}
