using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DataPackageTool.UI.Extensions;
using DataPackageTool.UI.Models;
using LiveChartsCore.Defaults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Views.Pages.ServerPages
{
    public class ServerOverviewViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "overview";

        public Guild Guild { get; set; } = new Guild();
        public DataPackage Package { get; set; } = new DataPackage();
        private string? _name;
        public string? Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        private IImage? _icon;
        public IImage? Icon { get => _icon; set => this.RaiseAndSetIfChanged(ref _icon, value); }
        public ObservableCollection<InviteCopyModel> Invites { get; set; } = [];
        public ServerOverviewViewModel()
        {
            _ = UpdateData();
        }
        public ServerOverviewViewModel(Guild guild) : this()
        {
            Guild = guild;
            Invites = new(guild.Invites.Select(x => new InviteCopyModel(x)));
            Package = guild.DataPackage ?? Package;
        }
        public async Task UpdateData()
        {
            Name = Guild.Name ?? Guild.Id;
            Icon = await Guild.GetIconAsync();
        }
        public void CopyInvite(object invite)
        {
            Application.Current?.GetTopLevel()?.Clipboard?.SetTextAsync("https://discord.gg/" + invite);
        }

        public bool HasMessages => Guild.Channels.Any(x => x.Messages.Count > 0);
        public bool HasVoice => Package.VoiceCalls.Any(x => x.Channel != null && Guild.Channels.Contains(x.Channel));
        public ISeries[] MonthlySentMessagesSeries => [
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>(
                        Guild.Channels.SelectMany(x=>x.Messages).GroupBy(x => new DateTime(x.Timestamp.Year, x.Timestamp.Month, 1)).OrderBy(x => x.Key).Select(x => new DateTimePoint(x.Key, x.Count()))
                    ),
                    MaxBarWidth = 32,
                    Padding = 0,
                }
            ];
        public ISeries[] MonthlyVoiceTimeSeries => [
                new ColumnSeries<DateTimePoint>
                {
                    Values = new ObservableCollection<DateTimePoint>(
                        Package.VoiceCalls.Where(x=>x.Channel!=null && Guild.Channels.Contains(x.Channel)).GroupBy(x => new DateTime(x.StartedAt.Year, x.StartedAt.Month, 1)).OrderBy(x => x.Key).Select(x => new DateTimePoint(x.Key, x.Sum(x=>x.Duration.Ticks)))
                    ),
                    MaxBarWidth = 32,
                    Padding = 0,
                    YToolTipLabelFormatter = (x)=>TimeSpan.FromTicks((long?)x.Model?.Value??0).ToString(@"d\d\ hh\h\ mm\m\ ss\s")
                }
            ];
        public static Axis[] MonthXAxis => Constants.MonthXAxis;
        public static Axis[] BaseYAxis => Constants.BaseYAxis;
        public static Axis[] TimespanYAxis => Constants.TimespanYAxis;
    }
}
