using Avalonia;
using DataPackageTool.UI.Extensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataPackageTool.UI.Models
{
    public class InviteCopyModel(string invite)
    {
        public string Invite { get; } = invite;
        public ICommand CopyInvite { get; } = ReactiveCommand.Create(() =>
        {
            Application.Current?.GetTopLevel()?.Clipboard?.SetTextAsync("https://discord.gg/" + invite);
        });
        /*public void CopyInvite()
        {
           
        }*/
    }
}
