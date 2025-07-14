using HarmonyLib;

namespace QOLFixes.Handlers;

[HarmonyPatch]
public static class DialogueHandler
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
    public static void OnDialogueTreeUpdate(CharacterDialogueTree __instance)
    {
        if (ModMain.IsCancelDialogueEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
        {
            __instance.EndConversation();
            ModMain.Instance.Log($"Exited dialogue for {__instance._characterName}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void OnDialogueConversationStart(CharacterDialogueTree __instance)
    {
        if (ModMain.IsFreezeTimeAtEyeDisabled && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse && __instance._timeFrozen)
        {
            __instance._timeFrozen = false;
            OWTime.Unpause(OWTime.PauseType.Reading);
            ModMain.Instance.Log($"Canceled time freeze for {__instance._characterName} dialogue.");
        }
    }
}
