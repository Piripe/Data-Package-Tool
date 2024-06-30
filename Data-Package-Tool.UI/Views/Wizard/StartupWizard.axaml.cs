using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using DataPackageTool.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Wizard
{
    public partial class StartupWizard : UserControl
    {
        public event EventHandler<DataPackage>? DataPackageLoaded;


        public StartupWizard()
        {
            InitializeComponent();
            AddHandler(DragDrop.DropEvent, onDrop);
            AddHandler(DragDrop.DragEnterEvent, onDragEnter);
            AddHandler(DragDrop.DragOverEvent, onDragOver);
            AddHandler(DragDrop.DragLeaveEvent, onDragLeave);
            OpenDialogButton.Tapped += OpenDialogButton_Tapped;
        }

        private async Task LoadPackage(string fileName)
        {
            PlaySurprisedAnim();

            // TODO: Manage exceptions with a cute error message
            DataPackage? package = null;

            void LoadDataSourceConfigAndInvoke()
            {
                if (package != null && DataSource.Config != null)
                {
                    package.InviteUsability = DataSource.Config.InviteMode;
                    package.SelfbotUsability = DataSource.Config.SelfBotMode;
                    package.BotUsability = DataSource.Config.BotMode;

                    DataPackageLoaded?.Invoke(this, package);
                }
            }

            ProgressbarBox.Classes.Add("Shown");

            DataSource.DataSourceConfigLoaded += (sender, e) =>
            {
                OpenDialogButton.IsVisible = false;
                DataSource.Classes.Remove("Shown");
                LoadDataSourceConfigAndInvoke();
            };

            DataSource.Classes.Add("Shown");

            package = await DataPackage.LoadAsync(fileName, (LoadStatus status) =>
            {
                StatusText.Text = status.Status;
                StatusProgressBar.Value = status.Progress*100;
            });

            LoadDataSourceConfigAndInvoke();

            ProgressbarBox.Classes.Remove("Shown");
        }

        private void PlaySurprisedAnim()
        {
            if (DateTime.Now.Subtract(lastAnimPlay).Seconds > 0.8)
            {
                lastAnimPlay = DateTime.Now;
                var anim = (Animation)Resources["SurprisedFile"]!;
                anim.RunAsync(CenterFile);
            }
        }

        private async void OpenDialogButton_Tapped(object? sender, TappedEventArgs e)
        {
            PlaySurprisedAnim();
#pragma warning disable CS0618 // Type or member is obsolete
            var result = await new OpenFileDialog() { AllowMultiple = false, Filters = [new FileDialogFilter { Name = "Discord Package files", Extensions = ["zip"] }]}.ShowAsync(GetWindow());
#pragma warning restore CS0618 // Type or member is obsolete

            if (result != null && result.Length > 0)
            {
                await LoadPackage(result[0]);
            }
        }

        async void onDrop(object? sender, DragEventArgs e)
        {
            if (isDragDropValid(e))
            {
                var files = e.Data.GetFiles() ?? Array.Empty<IStorageItem>();
                if (files.Count() < 1) return;
                string file = files.First().Path.LocalPath;

                await LoadPackage(file);
            }
        }
        bool dragOnWindow = false;
        DateTime lastAnimPlay = DateTime.MinValue;
        void onDragEnter(object? sender, DragEventArgs e)
        {
            if (isDragDropValid(e) && e.Source == DropZone && !dragOnWindow)
            {
                PlaySurprisedAnim();
            }
            dragOnWindow = true;
        }
        void onDragLeave(object? sender, DragEventArgs e)
        {
            if (e.Source == DropZone) dragOnWindow = false;
        }
        void onDragOver(object? sender, DragEventArgs e)
        {
            if (isDragDropValid(e))
            {

                e.DragEffects = DragDropEffects.Link;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }
        bool isDragDropValid(DragEventArgs e) =>
            e.Data.Contains(DataFormats.Files) &&
            ((e.Data.GetFiles() ?? Array.Empty<IStorageItem>()).FirstOrDefault()?.Name.EndsWith(".zip") ?? false); // Check if the dragged file is a .zip file

        Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");
    }
}
