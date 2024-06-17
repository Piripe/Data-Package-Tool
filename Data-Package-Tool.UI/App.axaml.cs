using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using DataPackageTool.UI.ViewModels;
using DataPackageTool.UI.Views;

namespace DataPackageTool.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            throw new System.NotImplementedException("Not implemented yet");
            //singleViewPlatform.MainView = new MainView
            //{
            //    DataContext = new MainViewModel()
            //};
        }

        base.OnFrameworkInitializationCompleted();
    }
}
