using DataPackageTool.Core;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;

namespace DataPackageTool.UI.Views.Pages.OverviewPages
{
    public class NotificationsViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "notifications";
        public DataPackage Package { get; set; } = new DataPackage();

        public static Axis[] MonthXAxis => Constants.MonthXAxis;
        public static Axis[] WeekHeatmapXAxis => Constants.WeekHeatmapXAxis;
        public static Axis[] WeekXAxis => Constants.WeekXAxis;
        public static Axis[] BaseYAxis => Constants.BaseYAxis;
        public static Axis[] WeekHeatmapYAxis => Constants.WeekHeatmapYAxis;


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
        public ISeries[] NotifyingClock { get; set; } = null!;
        public string MaxNotifyingClockHour { get; set; } = "";
        public int MaxNotifyingClock { get; set; } = 0;

        public ISeries[] WeeklyClickedNotificationsSeries { get; set; } = null!;
        public string MaxWeeklyClickedNotificationsDay { get; set; } = "";
        public int MaxWeeklyClickedNotifications { get; set; } = 0;

        public NotificationsViewModel()
        {
            Init();
        }
        public NotificationsViewModel(DataPackage package)
        {
            Package = package;
            Init();
        }


        private void Init()
        {
            var weeklyClickedNotificationsSeries = Package.Messages.GroupBy(x => x.Timestamp.DayOfWeek).OrderBy(x => x.Key);
            WeeklyClickedNotificationsSeries = [
                new ColumnSeries<int>
                {
                    Values = weeklyClickedNotificationsSeries.Select(x => x.Count()).AsArray(),
                    MaxBarWidth = 64,
                    Padding = 8,
                }
            ];
            var maxWeeklyClickedNotifications = weeklyClickedNotificationsSeries.MaxBy(x => x.Count())!;
            MaxWeeklyClickedNotifications = maxWeeklyClickedNotifications.Count();
            MaxWeeklyClickedNotificationsDay = maxWeeklyClickedNotifications.Key.ToString();
            var clockData = Package.NotificationsClicked.GroupBy(x => x.Timestamp.Hour).OrderBy(x => x.Key).Select(x => (x.Key, Value: x.Count()));
            MaxNotifyingClock = clockData.Max(x => x.Value);
            MaxNotifyingClockHour = new DateTime(1, 1, 1, clockData.First(x => x.Value == MaxNotifyingClock).Key, 0, 0).ToShortTimeString();
            NotifyingClock = clockData.Select((value) =>
            {
                var series = new PieSeries<int>(1)
                {
                    InnerRadius = 35,
                    Pushout = 12,
                    HoverPushout = 24,
                    CornerRadius = 4,
                    OuterRadiusOffset = 250 - value.Value * 250 / MaxNotifyingClock,
                    Name = new DateTime(1, 1, 1, value.Key, 0, 0).ToShortTimeString(),
                    ToolTipLabelFormatter = (x) => value.Value + " Notifications clicked",
                    Fill = new RadialGradientPaint(Constants.SKBlurple.WithAlpha(80), Constants.SKBlurple),
                };
                return series;
            }).ToArray();
            Task.Run(() =>
            {

            });
        }
    }
}
