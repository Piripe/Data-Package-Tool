using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.VisualTree;

namespace DataPackageTool.UI.Extensions
{
    public static class AvaloniaExtension
    {
        public static TopLevel? GetTopLevel(this Application app)
        {
            if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            if (app.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
            {
                var visualRoot = viewApp.MainView?.GetVisualRoot();
                return visualRoot as TopLevel;
            }
            return null;
        }
    }
}
