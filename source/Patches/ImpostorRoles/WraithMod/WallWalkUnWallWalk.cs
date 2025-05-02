using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
   
    public class WallWalkUnWallWalk
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var wraith = (Wraith)role;
                if (wraith.Noclipped)
                    wraith.WallWalk();
                else if (wraith.Enabled) wraith.UnWallWalk();
            }
        }
    }
}