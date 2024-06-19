using DataPackageTool.UI.Views.Pages;
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
        public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
        {
            return new Overview { DataContext = viewModel };
        }
    }
}
