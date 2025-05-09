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
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Security.Cryptography;
using DynamicData;

namespace DataPackageTool.UI.Views.Pages.OverviewPages
{
    public class FriendsViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; } = null!;
        public string? UrlPathSegment => "friends";
        public DataPackage Package { get; set; } = new DataPackage();


        private ObservableCollection<Channel> _channelsDataGrid = [];
        public ObservableCollection<Channel> ChannelsDataGrid
        {
            get => _channelsDataGrid;
            set => this.RaiseAndSetIfChanged(ref _channelsDataGrid, value);
        }


        public ObservableCollection<ISeries> MessagesFriendsShareSeries { get; } = [];
        public ObservableCollection<ISeries> VoiceFriendsShareSeries { get;} = [];
        public FriendsViewModel()
        {
            Init();
        }
        public FriendsViewModel(DataPackage package)
        {
            Package = package;
            Init();
        }
        private void Init()
        {
            Task.Run(() =>
            {
                HashSet<Channel> channelsDataGrid = Package.Channels.Where(x => x.IsDM()).ToHashSet();

                var messagesFriendsShareSeries = new List<ISeries>();
                var voiceFriendsShareSeries = new List<ISeries>();
                int messagesCount = 0;
                int totalMessageCount = channelsDataGrid.Sum(x => x.Messages.Count);
                long voiceTime = 0;
                long totalVoiceTime = channelsDataGrid.Sum(x => x.VoiceTimeIn.Ticks);

                foreach (var channel in channelsDataGrid.OrderByDescending(x => x.Messages.Count))
                {
                    long voiceTimeIn = channel.VoiceTimeIn.Ticks;
                    bool validMsgChan = channel.Messages.Count > totalMessageCount * 0.01;
                    bool validVoiceChan = voiceTimeIn > totalVoiceTime * 0.01;
                    //if (!(validMsgChan | validVoiceChan)) c;
                    if (validMsgChan)
                    {
                        messagesCount += channel.Messages.Count;
                        messagesFriendsShareSeries.Add(new PieSeries<int>
                        {
                            Values = [channel.Messages.Count],
                            Name = channel.DMRecipient?.DisplayName,
                            Fill = new SolidColorPaint(SKColor.FromHsl(MD5.HashData(Encoding.UTF8.GetBytes(channel.DMRecipient?.Tag ?? "")).FirstOrDefault() / 255f * 360, 55, 50))
                        });
                    }
                    if (validVoiceChan)
                    {
                        voiceTime += voiceTimeIn;
                        voiceFriendsShareSeries.Add(new PieSeries<long>
                        {
                            Values = [voiceTimeIn],
                            Name = channel.DMRecipient?.DisplayName,
                            Fill = new SolidColorPaint(SKColor.FromHsl(MD5.HashData(Encoding.UTF8.GetBytes(channel.DMRecipient?.Tag ?? "")).FirstOrDefault() / 255f * 360, 55, 50)),
                            ToolTipLabelFormatter = (x) => new TimeSpan(x.Model).ToString(@"d\d\ hh\h\ mm\m\ ss\s"),
                        });
                    }
                }
                MessagesFriendsShareSeries.AddRange([.. messagesFriendsShareSeries, new PieSeries<int>
                {
                    Values = [totalMessageCount - messagesCount],
                    Name = "Others",
                    Fill = new SolidColorPaint(new(90, 88, 95))
                }]);
                 VoiceFriendsShareSeries.AddRange([.. voiceFriendsShareSeries.OrderByDescending(x=>(x as PieSeries<long>)!.Values!.First()), new PieSeries<long>
                {
                    Values = [totalVoiceTime - voiceTime],
                    Name = "Others",
                    Fill = new SolidColorPaint(new(90, 88, 95)),
                    ToolTipLabelFormatter = (x) => new TimeSpan(x.Model).ToString(@"d\d\ hh\h\ mm\m\ ss\s"),
                }]);

                channelsDataGrid.UnionWith(Package.UsersMap.Values.Where(x => channelsDataGrid.All(y => y.DMRecipientId != x.Id)).Select(x => new Channel() { DMRecipientId = x.Id }).ToHashSet());
                ChannelsDataGrid = new(channelsDataGrid.OrderByDescending(x => x.Messages.Count));
            });

        }
    }
}
