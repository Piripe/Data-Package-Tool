using DataPackageTool.UI.Views.Pages;
using DataPackageTool.UI.Views.Pages.ServerPages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages
{
    public class ServerViewLocator : IViewLocator
    {
        public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
        {
            ServerOverviewViewModel => new ServerOverview() { DataContext = viewModel },
            ServerChannelViewModel => new ServerChannel() { DataContext = viewModel },
            _ => default
        };
    }
}
