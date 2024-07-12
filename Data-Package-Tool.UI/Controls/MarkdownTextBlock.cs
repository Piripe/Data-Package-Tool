using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataPackageTool.UI.Controls
{
    public class MarkdownTextBlock : UserControl
    {

        /// <summary>
        /// Defines the <see cref="Text"/> property.
        /// </summary>
        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<TextBlock, string?>(nameof(Text));
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string? Text
        {
            get => GetValue(TextProperty);
            set {
                SetValue(TextProperty, value);
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            // TODO: Parse text to apply Discord's markdown https://support.discord.com/hc/en-us/articles/210298617-Markdown-Text-101-Chat-Formatting-Bold-Italic-Underline
            Content = new SelectableTextBlock() { Text = Text, Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Ibeam), FontSize = 14, LineHeight = 22, TextWrapping = TextWrapping.Wrap };
        }
    }
}
