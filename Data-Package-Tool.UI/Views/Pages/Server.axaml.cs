using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using DataPackageTool.UI.Views.Sidebar;
using System.Diagnostics;

namespace DataPackageTool.UI.Views.Pages
{
    public partial class Server : ReactiveUserControl<ServerViewModel>
    {
        public Server()
        {
            InitializeComponent();
        }
    }
}
