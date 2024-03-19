using HarmonyLib;
using QOLFixes.Components;

namespace QOLFixes;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
    private static void OnReticleControllerAwake(ReticleController __instance)
    {
        __instance.gameObject.AddComponent<ContextualReticleController>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.Awake))]
    private static void OnToolModeSwapperAwake(ToolModeSwapper __instance)
    {
        __instance.gameObject.AddComponent<ManualTranslatorEquipper>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
    public static void OnDialogueTreeUpdate(CharacterDialogueTree __instance)
    {
        if (Config.IsCancelDialogueEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
        {
            __instance.EndConversation();
            Main.Instance.Log($"Exited dialogue for {__instance._characterName}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void OnDialogueConversationStart(CharacterDialogueTree __instance)
    {
        if (Config.IsFreezeTimeAtEyeDisabled && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse && __instance._timeFrozen)
        {
            __instance._timeFrozen = false;
            OWTime.Unpause(OWTime.PauseType.Reading);
            Main.Instance.Log($"Canceled time freeze for {__instance._characterName} dialogue.");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
    public static bool OnProbePromptEnterExit()
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
    public static void OnGhostSetEyeGlow(GhostEffects __instance)
    {
        if (Config.IsEyesAlwaysGlowEnabled)
        {
            __instance._eyeGlow = 1f;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSpacesuit), nameof(PlayerSpacesuit.Start))]
    private static void OnSpacesuitStart(PlayerSpacesuit __instance)
    {
        __instance.gameObject.AddComponent<HelmetToggler>();
    }
}
