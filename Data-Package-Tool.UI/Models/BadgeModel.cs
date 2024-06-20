using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using DataPackageTool.Core;
using DataPackageTool.Core.Models;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.UI.Models
{
    public class BadgeModel
    {
        public IImage Image { get; set; }
        public string Tooltip { get; set; }
        public BadgeModel(IImage image, string tooltip)
        {
            Image = image;
            Tooltip = tooltip;
        }

        public static List<BadgeModel> GetUserBadges(User user, DateTime packageDate)
        {
            List<BadgeModel> list = Constants.BadgesImage.Where(x=> user.Flags.HasFlag(x.Key)).Select(x=>x.Value).ToList(); // Get badges from user flags

            // Nitro
            if (user.ProfileMetadata?.NitroStartedAt.HasValue ?? false)
                list.Add(new BadgeModel(new SvgImage() { Source = SvgSource.Load("/Assets/Discord/Badges/Nitro.svg", new Uri("avares://DataPackageTool.UI")) }, user.ProfileMetadata.NitroStartedAt.Value.ToString("'Subscriber since 'MM/dd/yyyy' at 'hh:mm:ss tt")));

            // Boost
            int boostingLevel = user.ProfileMetadata?.GetBoostingLevel(packageDate) ?? 0;
            Debug.WriteLine($"Boosting level {boostingLevel} since {user.ProfileMetadata?.BoosingStartedAt!.Value.ToString()}");
            if (boostingLevel > 0)
                list.Add(new BadgeModel(new SvgImage() { Source = SvgSource.Load($"/Assets/Discord/Badges/Boost{boostingLevel}.svg", new Uri("avares://DataPackageTool.UI")) }, user.ProfileMetadata?.BoosingStartedAt!.Value.ToString("'Server boosting since 'MM/dd/yyyy' at 'hh:mm:ss tt")?? ""));

            // Legacy Username
            if (user.ProfileMetadata?.LegacyUsername != null)
                list.Add(new BadgeModel(new Bitmap(AssetLoader.Open(new Uri("avares://DataPackageTool.UI/Assets/Discord/Badges/LegacyUsername.png"))), $"Originally known as {user.ProfileMetadata.LegacyUsername}"));

            return list;
        }
    }
}
