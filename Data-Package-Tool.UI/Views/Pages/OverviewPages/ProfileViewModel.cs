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
                    XToolTipLabelFormatter = (x)=>new DateTime((long)(x.Model?.X??0)).AddDays(x.Model?.Y??0).ToShortDateString(),
                    YToolTipLabelFormatter = (x)=>((int?)x.Model?.Weight??0).ToString(),
                }
            ];
        public ISeries[] MonthlyVoiceTimeSeries => [
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>(
                        Package.VoiceCalls.GroupBy(x => new DateTime(x.StartedAt.Year, x.StartedAt.Month, 1)).OrderBy(x => x.Key).Select(x => new DateTimePoint(x.Key, x.Sum(x=>(long)x.Duration.Ticks)))
                    ),
                    MaxBarWidth = 32,
                    Padding = 0,
                    YToolTipLabelFormatter = (x)=>TimeSpan.FromTicks((long?)x.Model?.Value??0).ToString(@"d\d\ hh\h\ mm\m\ ss\s")
                }
            ];
        public ISeries[] VoiceActivityHeatmapSeries => [
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
                        ..Package.VoiceCalls.GroupBy(x => new DateTime(x.StartedAt.Year, x.StartedAt.Month, x.StartedAt.Day)).OrderBy(x => x.Key).Select(x => new WeightedPoint(x.Key.AddDays(((int)x.Key.DayOfWeek-1)*-1).Ticks/*((int)(x.Key - Package.User.CreationDate.Date.AddDays(((int)Package.User.CreationDate.Date.DayOfWeek-1) * -1)).TotalDays)/7*/,(int)x.Key.DayOfWeek, x.Sum(x=>(double)x.Duration.Ticks)))
                    ],
                    XToolTipLabelFormatter = (x)=>new DateTime((long)(x.Model?.X??0)).AddDays(x.Model?.Y??0).ToShortDateString(),
                    YToolTipLabelFormatter = (x)=>TimeSpan.FromTicks((long?)x.Model?.Weight??0).ToString(@"hh\h\ mm\m\ ss\s"),
                }
            ];
        public ISeries[] MonthlySentAttachmentsSeries => [
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>(
                        Package.Attachments.GroupBy(x => new DateTime(x.Message.Timestamp.Year, x.Message.Timestamp.Month, 1)).OrderBy(x => x.Key).Select(x => new DateTimePoint(x.Key, x.Count()))
                    ),
                    MaxBarWidth = 32,
                    Padding = 0,
                }
            ];
        public ISeries[] AttachmentsActivityHeatmapSeries => [
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
                        ..Package.Attachments.GroupBy(x => new DateTime(x.Message.Timestamp.Year, x.Message.Timestamp.Month, x.Message.Timestamp.Day)).OrderBy(x => x.Key).Select(x => new WeightedPoint(x.Key.AddDays(((int)x.Key.DayOfWeek-1)*-1).Ticks/*((int)(x.Key - Package.User.CreationDate.Date.AddDays(((int)Package.User.CreationDate.Date.DayOfWeek-1) * -1)).TotalDays)/7*/,(int)x.Key.DayOfWeek, x.Count()))
                    ],
                    XToolTipLabelFormatter = (x)=>new DateTime((long)(x.Model?.X??0)).AddDays(x.Model?.Y??0).ToShortDateString(),
                    YToolTipLabelFormatter = (x)=>((int?)x.Model?.Weight??0).ToString(),
                }
            ];
        public ISeries[] MonthlyClickedNotificationsSeries => [
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>(
                        Package.NotificationsClicked.GroupBy(x => new DateTime(x.Timestamp.Year, x.Timestamp.Month, 1)).OrderBy(x => x.Key).Select(x => new DateTimePoint(x.Key, x.Count()))
                    ),
                    MaxBarWidth = 32,
                    Padding = 0,
                }
            ];
        public ISeries[] NotificationsClickedActivityHeatmapSeries => [
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
                        ..Package.NotificationsClicked.GroupBy(x => new DateTime(x.Timestamp.Year, x.Timestamp.Month, x.Timestamp.Day)).OrderBy(x => x.Key).Select(x => new WeightedPoint(x.Key.AddDays(((int)x.Key.DayOfWeek-1)*-1).Ticks/*((int)(x.Key - Package.User.CreationDate.Date.AddDays(((int)Package.User.CreationDate.Date.DayOfWeek-1) * -1)).TotalDays)/7*/,(int)x.Key.DayOfWeek, x.Count()))
                    ],
                    XToolTipLabelFormatter = (x)=>new DateTime((long)(x.Model?.X??0)).AddDays(x.Model?.Y??0).ToShortDateString(),
                    YToolTipLabelFormatter = (x)=>((int?)x.Model?.Weight??0).ToString(),
                }
            ];

        public static Axis[] MonthXAxis => Constants.MonthXAxis;
        public static Axis[] WeekHeatmapXAxis => Constants.WeekHeatmapXAxis;
        public static Axis[] BaseYAxis => Constants.BaseYAxis;
        public static Axis[] TimespanYAxis => Constants.TimespanYAxis;
        public static Axis[] WeekHeatmapYAxis => Constants.WeekHeatmapYAxis;

        public int JoinCallShare => Package.JoinCalls.Count*20 / (Package.StartCalls.Count + Package.JoinCalls.Count);

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
