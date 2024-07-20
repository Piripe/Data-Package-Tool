using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Models;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages
{
    public class SearchViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string? UrlPathSegment => "search";

        public DataPackage Package { get; set; } = new DataPackage();
        private string _searchText = "";
        public string SearchText { get => _searchText; set {
                _searchText = value;
                Search();
            } }
        private IEnumerable<Message>? _results;
        public IEnumerable<Message>? Results { get => _results; set => this.RaiseAndSetIfChanged(ref _results, value); }

        public SearchViewModel()
        {
            HostScreen = Locator.Current.GetService<IScreen>()!;
        }
        public SearchViewModel(DataPackage package, IScreen? screen = null)
        {
            Package = package;
            HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        }

        private CancellationTokenSource? _searchCts;
        private void Search()
        {
            if (_searchCts == null || !_searchCts.IsCancellationRequested)
            {
                _searchCts?.Cancel();
                _searchCts = new CancellationTokenSource();
            }
            Task.Run(() => {
                var results = Package.Messages.Where((msg) => msg.Content?.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ?? false).OrderByDescending((msg) => msg.Timestamp).ToArray();
                Dispatcher.UIThread.Invoke(() => {
                    Results = results;
                });
            }, _searchCts.Token);

        }

    }
}
