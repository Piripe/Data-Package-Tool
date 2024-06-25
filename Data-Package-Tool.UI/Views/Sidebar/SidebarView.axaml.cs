using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Sidebar
{
    public partial class SidebarView : UserControl
    {
        public SidebarView()
        {
            InitializeComponent();

            //this.FindControl<Grid>("TestTooltip")?.SetValue<bool>(ToolTip.IsPointerOverProperty, true);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
        }
    }
}
