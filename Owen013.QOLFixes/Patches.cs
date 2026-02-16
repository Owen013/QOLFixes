using HarmonyLib;
using QOLFixes.Components;

namespace QOLFixes;

[HarmonyPatch]
internal static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
    private static void ReticleController_Awake_Postfix(ReticleController __instance) =>
        ModMain.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => ContextualReticleController.AddToReticleController(__instance));

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.Awake))]
    private static void ToolModeSwapper_Awake_Postfix(ToolModeSwapper __instance) =>
        TranslatorEquipper.AddToToolModeSwapper(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
    private static void CharacterDialogueTree_Update_Postfix(CharacterDialogueTree __instance)
    {
        if (Config.EnableDialogueCancelling && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
        {
            __instance.EndConversation();
            ModMain.Instance.ModHelper.Console.WriteLine($"Exited dialogue for {__instance._characterName}");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
    private static bool ProbePromptReceiver_GainFocus_LoseFocus_Prefix()
    {
        return !Config.DisableAutoScoutEquipping;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    private static void OnDialogueConversationStart(CharacterDialogueTree __instance)
    {
        if (Config.DisableFreezeTime && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse && __instance._timeFrozen)
        {
            __instance._timeFrozen = false;
            OWTime.Unpause(OWTime.PauseType.Reading);
            ModMain.Instance.ModHelper.Console.WriteLine($"Canceled time freeze for {__instance._characterName} dialogue.");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GhostEffects), nameof(GhostEffects.SetEyeGlow))]
    private static void OnGhostSetEyeGlow(GhostEffects __instance)
    {
        if (Config.EnableEyesAlwaysGlow)
            __instance._eyeGlow = 1f;
    }
}