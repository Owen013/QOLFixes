using HarmonyLib;

namespace QOLFixes;

[HarmonyPatch]
public static class GhostEyeGlowHandler
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GhostEffects), nameof(GhostEffects.SetEyeGlow))]
    public static void OnGhostSetEyeGlow(GhostEffects __instance)
    {
        if (ModMain.IsEyesAlwaysGlowEnabled)
        {
            __instance._eyeGlow = 1f;
        }
    }
}