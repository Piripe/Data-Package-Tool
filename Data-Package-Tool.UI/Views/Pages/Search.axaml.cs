using Avalonia.Controls;
using Avalonia.ReactiveUI;
using DataPackageTool.UI.Views.Pages.ServerPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace DataPackageTool.UI.Views.Pages
{
    public partial class Search : ReactiveUserControl<SearchViewModel>
    {
        public Search()
        {
            InitializeComponent();
            OpenChannelButton.PointerPressed += OpenChannelButton_PointerPressed;
        }

        private void OpenChannelButton_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var navigate = (DataContext as SearchViewModel)?.HostScreen.Router.Navigate;
            var msg = Results.SelectedMessage;
            var channel = msg?.Channel;
            if (navigate == null || msg == null || channel == null) return;
            if (channel.IsDM() || channel.IsGroupDM())
            {

            }
            else
            {
                var guild = channel.Guild;
                if (guild == null) return;
                var server = new ServerViewModel(guild);
                navigate.Execute(server);
                server.Router.Navigate.Execute(new ServerChannelViewModel(channel));
            }
        }
    }
}
