using HarmonyLib;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ResetChatSent
    {
        public static bool welcomesent = false;
        public static void Prefix()
        {
            // Welcome message
            if (PlayerControl.LocalPlayer != null && LobbyBehaviour.Instance && !welcomesent)
            {
                var message = $"Welcome to <color=#EE9D01>Town Of Us </color><b><color=#D91919>Unofficial</color></b> {PlayerControl.LocalPlayer.Data.PlayerName}!\n\nTo view the commands list, type <color=#D91919>/help</color>.";
                if (!string.IsNullOrWhiteSpace(message))
                {
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message, false);
                    welcomesent = true;
                }
            }
        }
    }
}