using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Models
{
    public class NavItemModel : ReactiveObject
    {
        private IImage? _image;
        public IImage? Image { get => _image; set => this.RaiseAndSetIfChanged(ref _image, value); }
        private object? _tooltip;
        public object? Tooltip { get => _tooltip; set => this.RaiseAndSetIfChanged(ref _tooltip, value); }
        public StreamGeometry? Path { get; set; }
    }
}
