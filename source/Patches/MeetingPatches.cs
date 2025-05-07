using System;
using AmongUs.Data;
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.Patches;

[HarmonyPatch()]
public static class MeetingPatches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
    [HarmonyPriority(Priority.First)]
    private static class PlayerVoteArea_SetCosmetics
    {
        private static void Postfix(PlayerVoteArea __instance, ref NetworkedPlayerInfo playerInfo)
        {
            if (TownOfUs.DarkMode.Value)
                __instance.Background.color = new Color(0.1f, 0.1f, 0.1f);
        }
    }
}