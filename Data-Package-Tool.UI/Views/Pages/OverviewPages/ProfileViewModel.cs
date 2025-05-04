using Avalonia.Media;
using DataPackageTool.Core.Models;
using DataPackageTool.Core;
using DataPackageTool.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Avalonia;
using Avalonia.Skia;
using Avalonia.Media.Immutable;
using LiveChartsCore.Drawing;

namespace DataPackageTool.UI.Views.Pages.OverviewPages
{
    public class ProfileViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "profile";
        public DataPackage Package { get; set; } = new DataPackage();
        public IImage Avatar { get; set; } = User.GetDefaultAvatarBitmap(0);
        public string Username => Package.User.DisplayName;
        public ObservableCollection<BadgeModel> Badges { get; set; } = new ObservableCollection<BadgeModel>();

        public ISeries[] MonthlySentMessagesSeries => [
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>(
                        Package.Messages.GroupBy(x => new DateTime(x.Timestamp.Year, x.Timestamp.Month, 1)).OrderBy(x => x.Key).Select(x => new DateTimePoint(x.Key, x.Count()))
                    ),
                    MaxBarWidth = 32,
                    Padding = 0,
                }
            ];
        public ISeries[] MessagesActivityHeatmapSeries => [
                new HeatSeries<WeightedPoint>
                {
                    HeatMap = [
                        new(54, 59, 112, 0),
                        new(54, 59, 112),
                        new(88, 101, 242),
                        new(250, 228, 125),
                        new(250, 5, 10),
                    ],
                    ColorStops = [
                        0,
                        0.025,
                        0.1,
                        0.5,
                        1,
                    ],
                    Values = [
                        ..Package.Messages.GroupBy(x => new DateTime(x.Timestamp.Year, x.Timestamp.Month, x.Timestamp.Day)).OrderBy(x => x.Key).Select(x => new WeightedPoint(x.Key.AddDays(((int)x.Key.DayOfWeek-1)*-1).Ticks/*((int)(x.Key - Package.User.CreationDate.Date.AddDays(((int)Package.User.CreationDate.Date.DayOfWeek-1) * -1)).TotalDays)/7*/,(int)x.Key.DayOfWeek, x.Count()))
                    ],
                    
                }
            ];

        public Axis[] MonthXAxes => [
            new DateTimeAxis(TimeSpan.FromDays(30), date => date.ToString("yyyy\\-MM")) {
                //Position = LiveChartsCore.Measure.AxisPosition.End,
                LabelsRotation = -35,
                ShowSeparatorLines = true,
                Padding = new(8),
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
            }
            ];
        public Axis[] WeekHeatmapXAxes => [
            new DateTimeAxis(TimeSpan.FromDays(7), date => date.ToString("yyyy\\-MM\\-dd")) {
                //Position = LiveChartsCore.Measure.AxisPosition.End,
                LabelsRotation = -35,
                ShowSeparatorLines = false,
                Padding = new(8),
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
                MinLimit = DateTime.Now.AddDays(-7*12).Ticks,
                MaxLimit = DateTime.Now.Ticks
            }
            ];
        public Axis[] WeekHeatmapYAxes => [
            new Axis() {
                Labels = ["Mon","Tue","Wen","Thu","Fri","Sat","Sun"],
                //Position = LiveChartsCore.Measure.AxisPosition.End,
                LabelsRotation = -35,
                ShowSeparatorLines = true,
                Padding = new(8),
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
            }
            ];
        public Axis[] BaseYAxes => [
            new Axis() {
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
            }
            ];
        public SolidColorPaint LegendTextPaint => new SolidColorPaint
        {
            Color = Application.Current!.TryGetResource("ForegroundBrush", Application.Current.ActualThemeVariant, out object? v) ? ((ImmutableSolidColorBrush)v!).Color.ToSKColor() : new SKColor(127, 127, 127),
            SKTypeface = Constants.ggSansTypeface,
        };

        public ProfileViewModel()
        {
            Badges = new ObservableCollection<BadgeModel>(BadgeModel.GetUserBadges(Package.User, Package.CreationTime));
            Init();
        }
        public ProfileViewModel(DataPackage package)
        {
            Package = package;
            Badges = new ObservableCollection<BadgeModel>(BadgeModel.GetUserBadges(Package.User, Package.CreationTime));
            Init();
        }
        private void Init()
        {
            Task<IImage> avatarTask = Package.User.GetAvatar();
            avatarTask.Wait(); // Supposed to be instant
            Avatar = avatarTask.Result;

        }
    }
}
