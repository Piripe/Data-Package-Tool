using Avalonia;
using Avalonia.Controls;
using DataPackageTool.Core;
using DataPackageTool.UI.ViewModels;

namespace DataPackageTool.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif

        Wizard.DataPackageLoaded += (object? sender, DataPackage package) =>
        {
            ViewContainer.Children.Clear();
            ViewContainer.Children.Add(new MainView() { DataContext = new MainViewModel() { Package = package } });
        };
    }
}
