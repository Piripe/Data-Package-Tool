using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace DataPackageTool.UI.Views.Pages.OverviewPages;

public partial class Attachments : ReactiveUserControl<AttachmentsViewModel>
{
    public Attachments()
    {
        InitializeComponent();
    }
}