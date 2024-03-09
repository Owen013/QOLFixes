using HarmonyLib;
using QOLFixes.Components;

namespace QOLFixes;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
    private static void ReticleControllerAwake(ReticleController __instance)
    {
        __instance.gameObject.AddComponent<ContextualReticleController>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.Awake))]
    private static void ToolModeSwapperAwake(ToolModeSwapper __instance)
    {
        __instance.gameObject.AddComponent<ManualTranslatorEquipper>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
    public static void DialogueTreeUpdate(CharacterDialogueTree __instance)
    {
        if (Config.IsCancelDialogueEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
        {
            __instance.EndConversation();
            Main.Instance.Log($"QOLFixes: Exited dialogue for {__instance._characterName}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void DialogueConversationStart(CharacterDialogueTree __instance)
    {
        if (Config.IsFreezeTimeAtEyeDisabled && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse)
        {
            __instance._timeFrozen = false;
            OWTime.Unpause(OWTime.PauseType.Reading);
            Main.Instance.Log($"QOLFixes: Canceled time freeze for {__instance._characterName} dialogue.");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
    public static bool ProbePromptEnterExit()
    {
        if (Config.IsAutoScoutEquipDisabled)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GhostEffects), nameof(GhostEffects.SetEyeGlow))]
    public static void Ghost_SetEyeGlow(GhostEffects __instance)
    {
        if (Config.IsEyesAlwaysGlowEnabled)
        {
            __instance._eyeGlow = 1f;
        }
    }
}
