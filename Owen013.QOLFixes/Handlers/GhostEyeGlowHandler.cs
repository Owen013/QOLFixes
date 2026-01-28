using HarmonyLib;

namespace QOLFixes.Handlers;

[HarmonyPatch]
public static class GhostEyeGlowHandler
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GhostEffects), nameof(GhostEffects.SetEyeGlow))]
    public static void OnGhostSetEyeGlow(GhostEffects __instance)
    {
        if (ModMain.Instance.IsEyesAlwaysGlowEnabled)
        {
            __instance._eyeGlow = 1f;
        }
    }
}