using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using DataPackageTool.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Controls
{
    public class DiscordMessageList : UserControl
    {
        public static readonly StyledProperty<IEnumerable<Message>?> MessagesProperty =
            AvaloniaProperty.Register<DiscordMessageList, IEnumerable<Message>?>(nameof(Messages));
        public IEnumerable<Message>? Messages
        {
            get => GetValue(MessagesProperty);
            set
            {
                SetValue(MessagesProperty, value);
            }
        }

        private List<StackPanel> _pages = new();
        private StackPanel _pagesPanel;
        private ScrollViewer _scrollViewer;
        private int _pageIndex = 0;
        public DiscordMessageList()
        {
            _pagesPanel = new StackPanel();
            _scrollViewer = new ScrollViewer() { Content = _pagesPanel};
            Content = _scrollViewer;

            _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;

            this.GetObservable(MessagesProperty).Subscribe(value =>
            {
                UpdatePages();
            });
        }

        private void _scrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            UpdatePages();
        }

        private void UpdatePages()
        {
            if (Messages == null) return;
            if (_scrollViewer.Offset.Y < 500)
            {
                int messageIndex = _pageIndex * 25;
                if (messageIndex < Messages?.Count())
                {
                    var pagePanel = new StackPanel();
                    pagePanel.Children.AddRange(Messages.Skip(messageIndex).Take(25).Reverse().Select(message => new DiscordMessage() { Message = message }));
                    _pages.Insert(0, pagePanel);
                    _pagesPanel.Children.Insert(0, pagePanel);

                    void pagePanel_LayoutUpdated(object? sender, EventArgs e)
                    {
                        Debug.Write("Setting scrollviewer offset from " + _scrollViewer.Offset.Y);
                        _scrollViewer.Offset = new Vector(0, _scrollViewer.Offset.Y + pagePanel.DesiredSize.Height);
                        Debug.WriteLine(" to " + _scrollViewer.Offset.Y);
                        pagePanel.LayoutUpdated -= pagePanel_LayoutUpdated;
                    }

                    pagePanel.LayoutUpdated += pagePanel_LayoutUpdated;
                    _pageIndex++;
                }
            }
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true); // Clean garbage from the potential last page opened
        }
    }
}
