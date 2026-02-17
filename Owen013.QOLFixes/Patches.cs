using HarmonyLib;
using QOLFixes.Components;
using QOLFixes.Enums;

namespace QOLFixes;

[HarmonyPatch]
internal static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    private static void CharacterDialogueTree_StartConversation_Postfix(CharacterDialogueTree __instance)
    {
        if (Config.DisableFreezeTime && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse && __instance._timeFrozen)
        {
            __instance._timeFrozen = false;
            OWTime.Unpause(OWTime.PauseType.Reading);
            ModMain.Instance.ModHelper.Console.WriteLine($"Canceled time freeze for {__instance._characterName} dialogue.");
        }
    }

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
    [HarmonyPatch(typeof(GhostEffects), nameof(GhostEffects.SetEyeGlow))]
    private static void GhostEffects_SetEyeGlow_Prefix(GhostEffects __instance)
    {
        if (Config.MoreVisibleGhosts)
            __instance._eyeGlow = 1f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSpacesuit), nameof(PlayerSpacesuit.Start))]
    private static void PlayerSpacesuit_Start_Postfix(PlayerSpacesuit __instance) =>
        ModMain.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => HelmetToggler.AddToPlayerSpacesuit(__instance));

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerSpacesuit), nameof(PlayerSpacesuit.PutOnHelmet))]
    private static bool PlayerSpacesuit_PutOnHelmet_Prefix()
    {
        if (HelmetToggler.Instance == null) return true;
        return !HelmetToggler.Instance.IsHelmetRemoved;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerResources), nameof(PlayerResources.UpdateOxygen))]
    private static void PlayerResources_UpdateOxygen_Prefix()
    {
        if (Config.HelmetTogglingMode != HelmetTogglingMode.WhenSafe || HelmetToggler.Instance == null) return;
        HelmetToggler.Instance.OnUpdateOxygenPresence();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
    [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
    private static bool ProbePromptReceiver_GainFocus_LoseFocus_Prefix() =>
        !Config.DisableAutoScoutEquipping;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
    private static void ReticleController_Awake_Postfix(ReticleController __instance) =>
        ModMain.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => ContextualReticleController.AddToReticleController(__instance));

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.Awake))]
    private static void ToolModeSwapper_Awake_Postfix(ToolModeSwapper __instance) =>
        TranslatorEquipper.AddToToolModeSwapper(__instance);
}