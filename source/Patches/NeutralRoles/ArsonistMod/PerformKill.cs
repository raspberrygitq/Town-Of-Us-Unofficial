using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            if (role.DouseTimer() > 0) return false;

            if (__instance == role.IgniteButton)
            {
                if (role.CanIgnite)
                {
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    role.LastDoused = DateTime.UtcNow;
                    role.Ignite();
                }
                return false;
            }

            
            if (__instance != HudManager.Instance.KillButton) return true;
            if (role.ClosestPlayer == null) return false;

            float dist = (float)Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            float killDist = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (dist >= killDist) return false;

            if (role.DousedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                int aliveDousedCount = role.DousedPlayers.Count(id =>
                {
                    var p = Utils.PlayerById(id);
                    return p != null && p.Data != null && !p.Data.IsDead;
                });

                if (aliveDousedCount < CustomGameOptions.MaxDoused)
                {
                    role.DousedPlayers.Add(role.ClosestPlayer.PlayerId);
                    role.LastDoused = DateTime.UtcNow;
                    Utils.Rpc(CustomRPC.Douse, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                }
                return false;
            }

            if (interact[0] == true)
            {
                role.LastDoused = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.TempSaveCdReset - CustomGameOptions.DouseCd);
                return false;
            }
            else if (interact[3] == true)
            {
                return false;
            }

            return false;
        }
    }
}
