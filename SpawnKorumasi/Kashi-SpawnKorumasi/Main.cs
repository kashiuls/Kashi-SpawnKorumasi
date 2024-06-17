using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kashi_SpawnKorumasi
{
    public class Main : RocketPlugin<KashiSpawnKorumasiConfiguration>
    {
        public static Main Instance;
        private Dictionary<CSteamID, bool> playerProtectionStatus;
        private Dictionary<CSteamID, Coroutine> playerCoroutines;
        private Dictionary<CSteamID, Coroutine> playerEffectCoroutines;
        private Dictionary<CSteamID, Coroutine> playerProtectionValuesCoroutines;

        protected override void Load()
        {
            Instance = this;
            playerProtectionStatus = new Dictionary<CSteamID, bool>();
            playerCoroutines = new Dictionary<CSteamID, Coroutine>();
            playerEffectCoroutines = new Dictionary<CSteamID, Coroutine>();
            playerProtectionValuesCoroutines = new Dictionary<CSteamID, Coroutine>();
            U.Events.OnPlayerConnected += OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerRevive += OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnPlayerUpdateGesture;
            DamageTool.damagePlayerRequested += OnPlayerDamaged;
            DamageTool.damageAnimalRequested += OnAnimalDamaged;
            DamageTool.damageZombieRequested += OnZombieDamaged;
            UnturnedPlayerEvents.OnPlayerInventoryAdded += OnPlayerInventoryAdded;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerRevive -= OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= OnPlayerUpdateGesture;
            DamageTool.damagePlayerRequested -= OnPlayerDamaged;
            DamageTool.damageAnimalRequested -= OnAnimalDamaged;
            DamageTool.damageZombieRequested -= OnZombieDamaged;
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= OnPlayerInventoryAdded;
        }

        private void OnPlayerUpdateGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft || gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
            {
                CancelProtection(player, Configuration.Instance.Mesajlar.IptalMesajlari.MesajKorumaIptalYumruk);
            }
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (!playerProtectionStatus.ContainsKey(player.CSteamID))
            {
                playerProtectionStatus.Add(player.CSteamID, false);
            }

            StartProtectionCoroutine(player, Configuration.Instance.KorumaSuresi);
            CheckPlayerInventory(player);
        }

        private void OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            if (!playerProtectionStatus.ContainsKey(player.CSteamID))
            {
                playerProtectionStatus.Add(player.CSteamID, false);
            }
            StartProtectionCoroutine(player, Configuration.Instance.KorumaSuresi);
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            CancelProtection(player, Configuration.Instance.Mesajlar.IptalMesajlari.MesajKorumaIptalOlum);
        }

        private void StartProtectionCoroutine(UnturnedPlayer player, float korumaSuresi)
        {
            if (playerCoroutines.ContainsKey(player.CSteamID))
            {
                StopCoroutine(playerCoroutines[player.CSteamID]);
            }
            playerCoroutines[player.CSteamID] = StartCoroutine(GiveSpawnProtection(player, korumaSuresi));
        }

        private IEnumerator GiveSpawnProtection(UnturnedPlayer player, float korumaSuresi)
        {
            playerProtectionStatus[player.CSteamID] = true;
            UnturnedChat.Say(player, string.Format(Configuration.Instance.Mesajlar.Koruma.MesajKorumaAktif, korumaSuresi));
            player.Player.equipment.dequip();
            player.Player.life.askHeal(player.Player.life.health, false, false);
            player.Player.life.askDisinfect(100);
            player.Player.life.askEat(100);
            player.Player.life.askDrink(100);

            if (playerEffectCoroutines.ContainsKey(player.CSteamID))
            {
                StopCoroutine(playerEffectCoroutines[player.CSteamID]);
            }
            playerEffectCoroutines[player.CSteamID] = StartCoroutine(ShowEffect(player));

            if (playerProtectionValuesCoroutines.ContainsKey(player.CSteamID))
            {
                StopCoroutine(playerProtectionValuesCoroutines[player.CSteamID]);
            }
            playerProtectionValuesCoroutines[player.CSteamID] = StartCoroutine(MaintainProtectionValues(player));

            yield return new WaitForSeconds(korumaSuresi);

            UnturnedChat.Say(player, Configuration.Instance.Mesajlar.Koruma.MesajKorumaPasif);
            playerProtectionStatus[player.CSteamID] = false;
            if (playerEffectCoroutines.ContainsKey(player.CSteamID))
            {
                StopCoroutine(playerEffectCoroutines[player.CSteamID]);
                playerEffectCoroutines.Remove(player.CSteamID);
            }
            if (playerProtectionValuesCoroutines.ContainsKey(player.CSteamID))
            {
                StopCoroutine(playerProtectionValuesCoroutines[player.CSteamID]);
                playerProtectionValuesCoroutines.Remove(player.CSteamID);
            }
        }

        private IEnumerator ShowEffect(UnturnedPlayer player)
        {
            while (playerProtectionStatus.ContainsKey(player.CSteamID) && playerProtectionStatus[player.CSteamID])
            {
                EffectManager.sendEffect(Configuration.Instance.PartikulEfektiID, EffectManager.LARGE, player.Position);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator MaintainProtectionValues(UnturnedPlayer player)
        {
            while (playerProtectionStatus.ContainsKey(player.CSteamID) && playerProtectionStatus[player.CSteamID])
            {
                player.Player.life.askHeal(100, false, false);
                player.Player.life.serverModifyVirus(0);
                player.Player.life.askEat(100);
                player.Player.life.askDrink(100);
                player.Player.life.askRest(100);
                player.Player.life.askDisinfect(100);
                player.Player.life.askBreath(100);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void CancelProtection(UnturnedPlayer player, string reason)
        {
            if (playerProtectionStatus.ContainsKey(player.CSteamID) && playerProtectionStatus[player.CSteamID])
            {
                if (playerCoroutines.ContainsKey(player.CSteamID))
                {
                    StopCoroutine(playerCoroutines[player.CSteamID]);
                    playerCoroutines.Remove(player.CSteamID);
                }
                UnturnedChat.Say(player, reason);
                playerProtectionStatus[player.CSteamID] = false;
                if (playerEffectCoroutines.ContainsKey(player.CSteamID))
                {
                    StopCoroutine(playerEffectCoroutines[player.CSteamID]);
                    playerEffectCoroutines.Remove(player.CSteamID);
                }
                if (playerProtectionValuesCoroutines.ContainsKey(player.CSteamID))
                {
                    StopCoroutine(playerProtectionValuesCoroutines[player.CSteamID]);
                    playerProtectionValuesCoroutines.Remove(player.CSteamID);
                }
            }
        }

        private void OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(parameters.player);
            if (playerProtectionStatus.ContainsKey(player.CSteamID) && playerProtectionStatus[player.CSteamID])
            {
                shouldAllow = false;
                player.Player.life.serverModifyVirus(0);
                if (parameters.cause == EDeathCause.ZOMBIE)
                {
                    parameters.times = 0;
                }
                UnturnedChat.Say(player, Configuration.Instance.Mesajlar.Genel.MesajKorumaCanliHasar);
            }
        }

        private void OnAnimalDamaged(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            var instigator = parameters.instigator as Player;
            if (instigator != null)
            {
                var instigatorPlayer = UnturnedPlayer.FromPlayer(instigator);
                if (playerProtectionStatus.ContainsKey(instigatorPlayer.CSteamID) && playerProtectionStatus[instigatorPlayer.CSteamID])
                {
                    shouldAllow = false;
                    UnturnedChat.Say(instigatorPlayer, Configuration.Instance.Mesajlar.Genel.MesajKorumaCanliHasar);
                }
            }
        }

        private void OnZombieDamaged(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            var instigator = parameters.instigator as Player;
            if (instigator != null)
            {
                var instigatorPlayer = UnturnedPlayer.FromPlayer(instigator);
                if (playerProtectionStatus.ContainsKey(instigatorPlayer.CSteamID) && playerProtectionStatus[instigatorPlayer.CSteamID])
                {
                    shouldAllow = false;
                    UnturnedChat.Say(instigatorPlayer, Configuration.Instance.Mesajlar.Genel.MesajKorumaCanliHasar);
                }
            }
        }

        private void OnPlayerInventoryAdded(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            if (playerProtectionStatus.ContainsKey(player.CSteamID) && playerProtectionStatus[player.CSteamID] && Configuration.Instance.SilahID.Contains(P.item.id))
            {
                CancelProtection(player, Configuration.Instance.Mesajlar.IptalMesajlari.MesajKorumaIptalSilah);
            }
        }

        private void CheckPlayerInventory(UnturnedPlayer player)
        {
            foreach (var page in player.Inventory.items)
            {
                if (page == null) continue;
                foreach (var item in page.items)
                {
                    if (Configuration.Instance.SilahID.Contains(item.item.id))
                    {
                        playerProtectionStatus[player.CSteamID] = true;
                        return;
                    }
                }
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "SilahID_Ekle_Basarisiz", "ID girmeniz gerekmektedir!" },
            { "SilahID_Ekle_Basari", "Silah ekleme başarılı!" }
        };
    }
}
