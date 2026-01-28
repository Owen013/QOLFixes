using HarmonyLib;

namespace QOLFixes.Handlers;

[HarmonyPatch]
public static class DialogueHandler
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
    public static void OnDialogueTreeUpdate(CharacterDialogueTree __instance)
    {
        if (ModMain.Instance.IsCancelDialogueEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
        {
            __instance.EndConversation();
            ModMain.Instance.ModHelper.Console.WriteLine($"Exited dialogue for {__instance._characterName}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void OnDialogueConversationStart(CharacterDialogueTree __instance)
    {
        if (ModMain.Instance.IsFreezeTimeAtEyeDisabled && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse && __instance._timeFrozen)
        {
            __instance._timeFrozen = false;
            OWTime.Unpause(OWTime.PauseType.Reading);
            ModMain.Instance.ModHelper.Console.WriteLine($"Canceled time freeze for {__instance._characterName} dialogue.");
        }
    }
}
