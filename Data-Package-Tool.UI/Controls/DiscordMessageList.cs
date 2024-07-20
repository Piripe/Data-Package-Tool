using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.LogicalTree;
using DataPackageTool.Core.Models;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Control> MessageContextMenu { get; } = new();

        private List<StackPanel> _pages = new();
        private StackPanel _pagesPanel;
        private ScrollViewer _scrollViewer;
        private List<Control> _defaultMessageContextMenu;
        private ContextMenu _contextMenu;
        private int _pageIndex = 0;
        public DiscordMessageList()
        {
            _contextMenu = new ContextMenu();

            _defaultMessageContextMenu = [
                new MenuItem() { Header = "Copy Text" },
                new MenuItem() { Header = "Copy Message Link" },
                new Separator(),
                new MenuItem() { Header = "Copy Message ID" },
            ];
            _defaultMessageContextMenu[0].PointerPressed += CopyText_PointerPressed;
            _defaultMessageContextMenu[1].PointerPressed += CopyLink_PointerPressed;
            _defaultMessageContextMenu[3].PointerPressed += CopyID_PointerPressed;

            _pagesPanel = new StackPanel();
            _scrollViewer = new ScrollViewer() { Content = _pagesPanel};
            Content = _scrollViewer;
 
            _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;

            MessageContextMenu.CollectionChanged += MessageContextMenu_CollectionChanged;

            UpdateContextMenuItems();

            this.GetObservable(MessagesProperty).Subscribe(value =>
            {
                _pages.Clear();
                _pagesPanel.Children.Clear();
                _pageIndex = 0;
                UpdatePages();
            });
        }

        private void MessageContextMenu_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateContextMenuItems();
        }

        private void CopyLink_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_selectedMessage == null) return;
            string? msgLink = _selectedMessage.GetMessageLink();
            if (msgLink == null) return;
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, msgLink);
            clipboard?.SetDataObjectAsync(dataObject);
        }

        private void CopyID_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_selectedMessage == null) return;
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, _selectedMessage.Id);
            clipboard?.SetDataObjectAsync(dataObject);
        }

        private void CopyText_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_selectedMessage==null||_selectedMessage.Content==null) return;
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, _selectedMessage.Content);
            clipboard?.SetDataObjectAsync(dataObject);
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
                    pagePanel.Children.AddRange(Messages.Skip(messageIndex).Take(25).Reverse().Select(message => {
                        var msg = new DiscordMessage() { Message = message, ContextMenu = _contextMenu };
                        msg.AddHandler(PointerPressedEvent,Msg_PointerPressed,Avalonia.Interactivity.RoutingStrategies.Tunnel);
                        return msg;
                    }));
                    _pages.Insert(0, pagePanel);
                    _pagesPanel.Children.Insert(0, pagePanel);

                    void pagePanel_LayoutUpdated(object? sender, EventArgs e)
                    {
                        _scrollViewer.Offset = new Vector(0, _scrollViewer.Offset.Y + pagePanel.DesiredSize.Height);
                        pagePanel.LayoutUpdated -= pagePanel_LayoutUpdated;
                    }

                    pagePanel.LayoutUpdated += pagePanel_LayoutUpdated;
                    _pageIndex++;
                }
            }
        }

        private Message? _selectedMessage;
        public Message? SelectedMessage { get => _selectedMessage; }
        private void Msg_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(null).Properties.IsRightButtonPressed) return;
            _selectedMessage = (sender as DiscordMessage)?.Message;
        }

        private void UpdateContextMenuItems()
        {
            _contextMenu.Items.Clear();

            if (MessageContextMenu.Count > 0)
            {
                foreach (var item in MessageContextMenu)
                {
                    _contextMenu.Items.Add(item);
                }
                _contextMenu.Items.Add(new Separator());
            }
            foreach (var item in _defaultMessageContextMenu)
            {
                _contextMenu.Items.Add(item);
            }
            _pages.Clear();
            _pagesPanel.Children.Clear();
            _pageIndex = 0;
            UpdatePages();
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true); // Clean garbage from the potential last page opened
        }
    }
}
