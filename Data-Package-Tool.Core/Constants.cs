using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Text.Json;

namespace DataPackageTool.Core
{
    public static class Constants
    {
        public const string DuplicateDMWarning = "You have multiple dm channels with this user! This is likely due to a bug which allowed opening multiple dms when messaging someone for the first time.\n\nSince Discord only saves a single dm channel as the default dm, there is no guarantee that it will open the right one.";
        public const string InvalidTokenError = "Entered token is invalid or doesn't belong to the same account!";
        public const string MissingTokenError = "You must enter your account token in the Settings tab to use this function.";
        public const string MissingBotTokenError = "You must enter a bot token in the Settings tab to use this function.";
        public const string InvalidBotTokenError = "Entered token is invalid!";
        public const string WrongTokenType = "Entered bot token belongs to your own account!";
        public const string UnknownDeletedUserId = "The user id of this deleted user is unknown. Use Fetch Info first.";

        public const string DeletedUserId = "456226577798135808";

        public static readonly List<int> BoostLevels = [1,2,3,6,9,12,15,18,24];

        public const string APIEndpoint = "https://discordapp.com/api/v9/";
        public const string CDNEndpoint = "https://cdn.discordapp.com/";
    }
}
