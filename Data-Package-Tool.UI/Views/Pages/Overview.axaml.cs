using Avalonia.Controls;
using Avalonia.ReactiveUI;
using DataPackageTool.UI.Models;
using ReactiveUI;

namespace DataPackageTool.UI.Views.Pages
{
    public partial class Overview : ReactiveUserControl<OverviewViewModel>
    {
        public Overview()
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
            ((OverviewViewModel)DataContext!).Router!.Navigate.Execute(link);
        }
    }
}
