using DataPackageTool.UI.Views.Pages;
using DataPackageTool.UI.Views.Pages.OverviewPages;
using DataPackageTool.UI.Views.Pages.ServerPages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI
{
    public class ViewLocator : IViewLocator
    {
        public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
        {
            OverviewViewModel overview => new Overview { DataContext = overview },
            ServerViewModel server => new Server { DataContext = server },
            SearchViewModel search => new Search { DataContext = search },
            ServerOverviewViewModel => new ServerOverview() { DataContext = viewModel },
            ServerChannelViewModel => new ServerChannel() { DataContext = viewModel },
            ProfileViewModel => new Profile() { DataContext = viewModel },
            _ => default
        };
    }
}
