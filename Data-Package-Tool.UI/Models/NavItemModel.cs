using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Models
{
    public class NavItemModel
    {
        public IImage? Image { get; set; }
        public object? Tooltip { get; set; }
        public StreamGeometry? Path { get; set; }
    }
}
