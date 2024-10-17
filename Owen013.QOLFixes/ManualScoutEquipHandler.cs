using HarmonyLib;

namespace QOLFixes;

[HarmonyPatch]
public static class ManualScoutEquipHandler
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
    public static bool OnProbePromptEnterExit()
    {
        if (ModMain.IsAutoScoutEquipDisabled)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}