using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Kashi_SpawnKorumasi
{
    public class CommandSilahSil : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "silahsil";
        public string Help => "Silah ID'si siler";
        public string Syntax => "/silahsil [id]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "KashiSP.silahsil" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDSilBasarisiz, Color.red);
                return;
            }

            if (ushort.TryParse(command[0], out ushort silahID))
            {
                if (Main.Instance.Configuration.Instance.SilahID.Contains(silahID))
                {
                    Main.Instance.Configuration.Instance.SilahID.Remove(silahID);
                    Main.Instance.Configuration.Save();
                    UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDSilBasari, Color.green);
                }
                else
                {
                    UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDSilMevcutDegil, Color.yellow);
                }
            }
            else
            {
                UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDSilBasarisiz, Color.red);
            }
        }
    }
}
