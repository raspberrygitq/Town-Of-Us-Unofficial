using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.CaptainMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateZoomButton(__instance);
        }

        public static void UpdateZoomButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Captain)) return;
            var zoomButton = __instance.KillButton;

            var role = Role.GetRole<Captain>(PlayerControl.LocalPlayer);

            bool isBlinded = false;
            foreach (var eclipsalRole in Role.GetRoles(RoleEnum.Eclipsal))
            {
                var eclipsal = (Eclipsal)eclipsalRole;
                if (eclipsal.BlindPlayers.Contains(PlayerControl.LocalPlayer))
                {
                    isBlinded = true;
                    break;
                }
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(zoomButton.cooldownTimerText, zoomButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                if (role.UsesLeft > 0)
                {
                    role.UsesText.text = role.UsesLeft + "";
                    role.UsesText.gameObject.SetActive(true);
                }
                else
                {
                    role.UsesText.gameObject.SetActive(false);
                }
            }

            zoomButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && role.UsesLeft > 0);
            if (role.Zooming) zoomButton.SetCoolDown(role.TimeRemainingZoom, CustomGameOptions.ZoomDuration);
            else
            {
                zoomButton.SetCoolDown(role.ZoomTimer(), CustomGameOptions.ZoomCooldown);
            }

            if (role.Zooming && MeetingHud.Instance)
            {
                zoomButton.SetCoolDown(0f, CustomGameOptions.ZoomCooldown);
            }

            var renderer = zoomButton.graphic;
            if (role.Zooming || (!zoomButton.isCoolingDown && role.ButtonUsable && PlayerControl.LocalPlayer.moveable && !role.sabotageLightsZoom() && !isBlinded))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}