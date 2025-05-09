using DataPackageTool.Core;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;

namespace DataPackageTool.UI.Views.Pages.OverviewPages
{
    public class MessagesViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "messages";
        public DataPackage Package { get; set; } = new DataPackage();

        public static Axis[] MonthXAxis => Constants.MonthXAxis;
        public static Axis[] WeekHeatmapXAxis => Constants.WeekHeatmapXAxis;
        public static Axis[] BaseYAxis => Constants.BaseYAxis;
        public static Axis[] WeekHeatmapYAxis => Constants.WeekHeatmapYAxis;

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


        public MessagesViewModel()
        {
            Init();
        }
        public MessagesViewModel(DataPackage package)
        {
            Package = package;
            Init();
        }


        private void Init()
        {
            Task.Run(() =>
            {

            });
        }
    }
}
