using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches {
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch {
        [HarmonyPostfix]
        public static void Postfix() {
            // Fix Killed When Zooming As Captain
            FixScreen.UnZoomFix();
            // Fix Grenadier blind in lobby
            ((Renderer)HudManager.Instance.FullScreen).gameObject.active = false;
        }
    }
    public class FixScreen
    {
        public static void UnZoomFix()
        {
            var size = 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }
    }

}
