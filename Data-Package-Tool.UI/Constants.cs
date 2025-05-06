using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Skia;
using Avalonia.Svg.Skia;
using DataPackageTool.Core.Enums;
using DataPackageTool.UI.Models;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI
{
    public static class Constants
    {
        public static readonly SKTypeface ggSansTypeface = Application.Current!.TryGetResource("gg sans", null, out object? font) ?
            FontManager.Current.TryGetGlyphTypeface(new Typeface((FontFamily)font!), out IGlyphTypeface? gtf) ?
                gtf.GetType().GetField("_typeface", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(gtf) as SKTypeface ?? SKTypeface.Default
                : SKTypeface.Default
            : SKTypeface.Default;
        public static readonly SolidColorPaint LegendTextPaint = new SolidColorPaint
        {
            Color = Application.Current!.TryGetResource("ForegroundBrush", Application.Current.ActualThemeVariant, out object? v) ? ((ImmutableSolidColorBrush)v!).Color.ToSKColor() : new SKColor(127, 127, 127),
            SKTypeface = ggSansTypeface,
        };
        public static Axis[] MonthXAxis => [
            new DateTimeAxis(TimeSpan.FromDays(30), date => date.ToString("yyyy\\-MM")) {
                //Position = LiveChartsCore.Measure.AxisPosition.End,
                LabelsRotation = -35,
                ShowSeparatorLines = true,
                Padding = new(8),
                LabelsPaint = Constants.LegendTextPaint,
                TextSize = 12,
            }
            ];
        public static Axis[] WeekHeatmapXAxis => [
            new DateTimeAxis(TimeSpan.FromDays(7), date => date.ToString("yyyy\\-MM\\-dd")) {
                //Position = LiveChartsCore.Measure.AxisPosition.End,
                LabelsRotation = -35,
                ShowSeparatorLines = false,
                Padding = new(8),
                LabelsPaint = Constants.LegendTextPaint,
                TextSize = 12,
                MinLimit = DateTime.Now.AddDays(-7*12).Ticks,
                MaxLimit = DateTime.Now.Ticks
            }
            ];
        public static Axis[] BaseYAxis => [
            new Axis() {
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
            }
        ];
        //readonly static Dictionary<long, string> dateSeparatorLabels = new()
        //{
        //    { 10000000, "1s" },
        //    { 100000000, "10s" },
        //    { 300000000, "30s" },
        //    { 600000000, "1m" },
        //    { 6000000000, "10m" },
        //    { 18000000000, "30m" },
        //    { 36000000000, "1h" },
        //    { 180000000000, "5h" },
        //    { 360000000000, "10h" },
        //    { 864000000000, "1d" },
        //    { 4320000000000, "5d" },
        //    { 8640000000000, "10d" },
        //    { 12960000000000, "15d" },
        //    { 17280000000000, "20d" },
        //    { 21600000000000, "25d" },
        //    { 25920000000000, "30d" }
        //};
        public static Axis[] TimespanYAxis => [
            new Axis() {
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
                //CustomSeparators=[10000000, 100000000, 300000000, 600000000, 6000000000, 18000000000, 36000000000, 180000000000, 360000000000, 864000000000, 4320000000000, 8640000000000, 12960000000000, 17280000000000, 21600000000000, 25920000000000],
                
                Labeler = x => new TimeSpan((long)x).ToString(@"d\d\ hh\h\ mm\m\ ss\s"),

            }
        ];
        public static Axis[] WeekHeatmapYAxis => [
            new Axis() {
                Labels = ["Mon","Tue","Wen","Thu","Fri","Sat","Sun"],
                //Position = LiveChartsCore.Measure.AxisPosition.End,
                LabelsRotation = -35,
                ShowSeparatorLines = true,
                Padding = new(8),
                LabelsPaint = LegendTextPaint,
                TextSize = 12,
                SeparatorsPaint = new SolidColorPaint(SKColor.Empty),
            }
        ];

        public static readonly Dictionary<UserFlag, BadgeModel> BadgesImage = new()
        {
            {UserFlag.STAFF,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/Staff.svg",new Uri("avares://DataPackageTool.UI"))},"Discord Staff") },
            {UserFlag.PARTNER,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/Partner.svg",new Uri("avares://DataPackageTool.UI"))}, "Partnered Server Owner") },
            {UserFlag.HYPESQUAD,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/Hypesquad.svg",new Uri("avares://DataPackageTool.UI"))}, "Hypesquad Events") },
            {UserFlag.BUG_HUNTER_LEVEL_1,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/BugHunter1.svg",new Uri("avares://DataPackageTool.UI"))}, "Normal Bug Hunter") },
            {UserFlag.HYPESQUAD_ONLINE_HOUSE_1,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/HypesquadBravery.svg",new Uri("avares://DataPackageTool.UI"))}, "HypeSquad Bravery") },
            {UserFlag.HYPESQUAD_ONLINE_HOUSE_2,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/HypesquadBrilliance.svg",new Uri("avares://DataPackageTool.UI"))}, "HypeSquad Brilliance") },
            {UserFlag.HYPESQUAD_ONLINE_HOUSE_3,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/HypesquadBalance.svg",new Uri("avares://DataPackageTool.UI"))}, "HypeSquad Balance") },
            {UserFlag.PREMIUM_EARLY_SUPPORTER,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/EarlySupporter.svg",new Uri("avares://DataPackageTool.UI"))}, "Early Supporter") },
            {UserFlag.BUG_HUNTER_LEVEL_2,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/BugHunter2.svg",new Uri("avares://DataPackageTool.UI"))}, "Gold Bug Hunter") },
            {UserFlag.VERIFIED_DEVELOPER,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/VerifiedDeveloper.svg",new Uri("avares://DataPackageTool.UI"))}, "Early Verified Bot Developer") },
            {UserFlag.ACTIVE_DEVELOPER,  new BadgeModel(new SvgImage() {Source = SvgSource.Load("/Assets/Discord/Badges/ActiveDeveloper.svg",new Uri("avares://DataPackageTool.UI"))}, "Active Developer") },
        };
        public static readonly Dictionary<ChannelType, StreamGeometry?> ChannelIcons = new()
        {
            {ChannelType.GUILD_TEXT, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.DM, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.GUILD_VOICE, Application.Current!.FindResource("VoiceChannelIcon") as StreamGeometry },
            {ChannelType.GROUP_DM, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.GUILD_ANNOUNCEMENT, Application.Current!.FindResource("AnnouncementChannelIcon") as StreamGeometry },
            {ChannelType.ANNOUNCEMENT_THREAD, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.PUBLIC_THREAD, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.PRIVATE_THREAD, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.GUILD_STAGE_VOICE, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.GUILD_DIRECTORY, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.GUILD_FORUM, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
            {ChannelType.GUILD_MEDIA, Application.Current!.FindResource("TextChannelIcon") as StreamGeometry },
        };
        public static readonly StreamGeometry? HomeIcon = Application.Current!.FindResource("HomeIcon") as StreamGeometry;
        public static readonly StreamGeometry? FriendsIcon = Application.Current!.FindResource("FriendsIcon") as StreamGeometry;
        public static readonly StreamGeometry? SearchIcon = Application.Current!.FindResource("SearchIcon") as StreamGeometry;
    }
}
