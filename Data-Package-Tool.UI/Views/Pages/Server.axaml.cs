using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using DataPackageTool.UI.Models;
using DataPackageTool.UI.Views.Sidebar;
using ReactiveUI;
using System.Diagnostics;

namespace DataPackageTool.UI.Views.Pages
{
    public partial class Server : ReactiveUserControl<ServerViewModel>
    {
        public Server()
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
            switch (link)
            {
            }
            ((ServerViewModel)DataContext!).Router!.Navigate.Execute(link);

        }
    }
}
