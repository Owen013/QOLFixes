using HarmonyLib;

namespace QOLFixes;

[HarmonyPatch]
public static class DialogueHandler
{
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
}
