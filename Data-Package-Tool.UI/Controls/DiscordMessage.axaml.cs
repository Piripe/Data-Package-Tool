using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DataPackageTool.Core.Models;
using System;

namespace DataPackageTool.UI.Controls
{
    public partial class DiscordMessage : UserControl
    {
        public static readonly StyledProperty<Message?> MessageProperty =
            AvaloniaProperty.Register<DiscordMessage, Message?>(nameof(Message));
        public Message? Message
        {
            get => GetValue(MessageProperty);
            set
            {
                SetValue(MessageProperty, value);
            }
        }


        public DiscordMessage()
        {
            InitializeComponent();
            this.GetObservable(MessageProperty).Subscribe(value =>
            {
                MessageContainer.Text = value?.Content;
                UsernameContainer.Text = value?.Author?.DisplayName;
                AvatarContainer.Source = value?.Author?.AvatarImage;
                DateContainer.Text = value?.Timestamp.ToString(@"MM/dd/yyyy h:mm:ss tt");
            });
        }
    }
}
