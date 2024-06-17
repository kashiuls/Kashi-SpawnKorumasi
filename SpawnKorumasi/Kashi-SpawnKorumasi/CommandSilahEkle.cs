using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Kashi_SpawnKorumasi
{
    public class CommandSilahEkle : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "silahekle";
        public string Help => "Silah ID'si ekler";
        public string Syntax => "/silahekle [id]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "KashiSP.silahekle" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDEkleBasarisiz, Color.red);
                return;
            }

            if (ushort.TryParse(command[0], out ushort silahID))
            {
                if (!Main.Instance.Configuration.Instance.SilahID.Contains(silahID))
                {
                    Main.Instance.Configuration.Instance.SilahID.Add(silahID);
                    Main.Instance.Configuration.Save();
                    UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDEkleBasari, Color.green);
                }
                else
                {
                    UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDMevcut, Color.yellow);
                }
            }
            else
            {
                UnturnedChat.Say(caller, Main.Instance.Configuration.Instance.Mesajlar.KomutMesajlari.MesajSilahIDEkleBasarisiz, Color.red);
            }
        }
    }
}
