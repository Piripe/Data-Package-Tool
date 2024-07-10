using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using DataPackageTool.UI.Views.Pages;
using ReactiveUI;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Sidebar
{
    public partial class SidebarView : UserControl
    {
        public SidebarView()
        {
            InitializeComponent();

            NavBox.SelectionChanged += NavBox_SelectionChanged;
        }

        private void NavBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            NavItemModel? navItem = e.AddedItems.Count > 0 ? (e.AddedItems[0] as NavItemModel) : null;
            if (navItem == null) return;
            IRoutableViewModel? link = navItem.Link;
            if (link == null) return;
            switch(link)
            {
                case ServerViewModel server:
                    server.InitData().ContinueWith(async (_) =>
                    {
                        navItem.Image = await server.Guild.GetIconAsync();
                        navItem.Tooltip = await server.Guild.GetNameAsync();
                    });
                    break;
            }
            ((SidebarViewModel)DataContext!).Router!.Navigate.Execute(link);
            
        }
    }
}
