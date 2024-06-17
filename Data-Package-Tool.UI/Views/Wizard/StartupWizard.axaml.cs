using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using DataPackageTool.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        private async void OpenDialogButton_Tapped(object? sender, TappedEventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var result = await new OpenFileDialog() { AllowMultiple = false, Filters = [new FileDialogFilter { Name = "Discord Package files", Extensions = ["zip"] }]}.ShowAsync(GetWindow());
#pragma warning restore CS0618 // Type or member is obsolete

            if (result != null && result.Length > 0)
            {
                DataPackage package = await DataPackage.LoadAsync(result[0], (LoadStatus status) =>
                {
                    StatusLabel.Content = status.Status;
                });
            }
        }

        async void onDrop(object? sender, DragEventArgs e)
        {
            if (isDragDropValid(e))
            {
                var files = e.Data.GetFiles() ?? Array.Empty<IStorageItem>();
                if (files.Count() < 1) return;
                string file = files.First().Path.LocalPath;

                DataPackage package = await DataPackage.LoadAsync(file, (LoadStatus status) =>
                {
                    StatusLabel.Content = status.Status;
                });
            }
        }
        bool dragOnWindow = false;
        DateTime lastAnimPlay = DateTime.MinValue;
        void onDragEnter(object? sender, DragEventArgs e)
        {
            if (isDragDropValid(e) && e.Source == DropZone && !dragOnWindow && DateTime.Now.Subtract(lastAnimPlay).Seconds > 0.8)
            {
                lastAnimPlay = DateTime.Now;
                var anim = (Animation)Resources["SurprisedFile"]!;
                anim.RunAsync(CenterFile);
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
