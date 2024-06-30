using Avalonia.Controls;
using DataPackageTool.Core;
using DataPackageTool.Core.Enums;
using System;

namespace DataPackageTool.UI.Views.Wizard
{
    public class DataSourceConfig
    {
        public DataSourceUsability InviteMode { get; set; }
        public DataSourceUsability SelfBotMode { get; set; }
        public DataSourceUsability BotMode { get; set; }
        public string? UserToken { get; set; }
        public string? BotToken { get; set; }
    }
    public partial class DataSourcePage : UserControl
    {
        const int MIN_TOKEN_LENGTH = 60; // Length of example bot token in Discord's docs

        public event EventHandler<DataSourceConfig>? DataSourceConfigLoaded;
        public DataSourceConfig? Config { get; set; }


        public DataSourcePage()
        {
            InitializeComponent();

            NextButton.Click += NextButton_Click;
        }

        private void HasInvalidData()
        {
            InvalidDataText.IsVisible = true;
        }
        private void NextButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Config == null)
            {
                DataSourceUsability inviteMode = (DataSourceUsability)((InviteDataMode2.IsChecked ?? false) ? 2 : (InviteDataMode1.IsChecked ?? false) ? 1 : 0);
                DataSourceUsability selfBotMode = (DataSourceUsability)((SelfBotMode2.IsChecked ?? false) ? 2 : (SelfBotMode1.IsChecked ?? false) ? 1 : 0);
                DataSourceUsability botMode = (DataSourceUsability)((BotMode2.IsChecked ?? false) ? 2 : (BotMode1.IsChecked ?? false) ? 1 : 0);

                string userToken = UserTokenBox.Text ?? "";
                string botToken = BotTokenBox.Text ?? "";

                if (selfBotMode > 0 && userToken.Length < MIN_TOKEN_LENGTH)
                {
                    UserTokenBox.Classes.Add("Invalid");
                    HasInvalidData();
                    return;
                }
                if (botMode > 0 && botToken.Length < MIN_TOKEN_LENGTH)
                {
                    BotTokenBox.Classes.Add("Invalid");
                    HasInvalidData();
                    return;
                }

                DRequest.Contexts.Add(DRequestContext.User, userToken);
                DRequest.Contexts.Add(DRequestContext.Bot, botToken);

                Config = new DataSourceConfig()
                {
                    InviteMode = inviteMode,
                    SelfBotMode = selfBotMode,
                    BotMode = botMode,
                    UserToken = userToken,
                    BotToken = botToken
                };

                DataSourceConfigLoaded?.Invoke(this, Config);
            }
        }
    }
}