using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace DataPackageTool.UI.Controls
{
    public partial class DiscordMessage : UserControl
    {
        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<DiscordMessage, string?>(nameof(Text));
        public static readonly StyledProperty<string?> UsernameProperty =
            AvaloniaProperty.Register<DiscordMessage, string?>(nameof(Username));
        public static readonly StyledProperty<DateTime?> DateProperty =
            AvaloniaProperty.Register<DiscordMessage, DateTime?>(nameof(Date));
        public static readonly StyledProperty<IImage?> AvatarProperty =
            AvaloniaProperty.Register<DiscordMessage, IImage?>(nameof(Avatar));
        public string? Text
        {
            get => MessageContainer.Text;
            set => MessageContainer.Text = value;
        }
        public string? Username
        {
            get => UsernameContainer.Text;
            set => UsernameContainer.Text = value;
        }
        public DateTime? Date
        {
            get => GetValue(DateProperty);
            set
            {
                SetValue(DateProperty, value);
                DateContainer.Text = value?.ToString(@"MM/dd/yyyy h:mm:ss tt");
            }
        }
        public IImage? Avatar
        {
            get => GetValue(AvatarProperty);
            set
            {
                SetValue(AvatarProperty, value);
                AvatarContainer.Source = value;
            }
        }


        public DiscordMessage()
        {
            InitializeComponent();
            this.GetObservable(AvatarProperty).Subscribe(value => AvatarContainer.Source = value);
        }
    }
}
