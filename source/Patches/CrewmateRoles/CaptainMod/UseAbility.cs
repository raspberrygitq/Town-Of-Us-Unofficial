using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.CaptainMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class UseAbility
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Captain))
            {
                var cap = (Captain)role;

                bool isBlinded = false;
                foreach (var eclipsalRole in Role.GetRoles(RoleEnum.Eclipsal))
                {
                    var eclipsal = (Eclipsal)eclipsalRole;
                    if (eclipsal.BlindPlayers.Contains(cap.Player))
                    {
                        isBlinded = true;
                        break;
                    }
                }

                if (cap.Zooming)
                {
                    if (isBlinded)
                    {
                        cap.UnZoomAbility();
                        cap.TimeRemainingZoom = 0f;
                        cap.ZoomEnabled = false;
                        cap.IsZooming = false;
                    }
                    else
                    {
                        cap.ZoomAbility();
                    }
                }
                else if (cap.ZoomEnabled)
                {
                    cap.UnZoomAbility();
                }
            }
        }
    }
}