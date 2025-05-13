using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CrewmateRoles.CaptainMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Captain);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Captain>(PlayerControl.LocalPlayer);

            foreach (var eclipsalRole in Role.GetRoles(RoleEnum.Eclipsal))
            {
                var eclipsal = (Eclipsal)eclipsalRole;
                if (eclipsal.BlindPlayers.Contains(PlayerControl.LocalPlayer))
                {
                    return false;
                }
            }

            if (!role.ButtonUsable) return false;
            if (role.sabotageLightsZoom()) return false;
            var zoomButton = HudManager.Instance.KillButton;
            if (__instance == zoomButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.Cooldown > 0) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.TimeRemainingZoom = CustomGameOptions.ZoomDuration;
                role.ZoomAbility();
                role.UsesLeft--;
                return false;
            }

            return true;
        }
    }
}