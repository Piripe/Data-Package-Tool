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
    public class VoiceViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "voice";
        public DataPackage Package { get; set; } = new DataPackage();

        public static Axis[] MonthXAxis => Constants.MonthXAxis;
        public static Axis[] WeekHeatmapXAxis => Constants.WeekHeatmapXAxis;
        public static Axis[] TimespanYAxis => Constants.TimespanYAxis;
        public static Axis[] WeekHeatmapYAxis => Constants.WeekHeatmapYAxis;


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

        public VoiceViewModel()
        {
            Init();
        }
        public VoiceViewModel(DataPackage package)
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
