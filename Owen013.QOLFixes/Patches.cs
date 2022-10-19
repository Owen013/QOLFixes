using HarmonyLib;

namespace QOLFixes
{
    public static class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
        public static void DialogueTreeUpdate(CharacterDialogueTree __instance)
        {
            if (QOLFixes.Instance.fastDialogueExitEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
            {
                __instance.EndConversation();
                QOLFixes.Instance.ModHelper.Console.WriteLine($"QOLFixes: Exited dialogue for {__instance._characterName}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
        public static void DialogueConversationStart(CharacterDialogueTree __instance)
        {
            if (QOLFixes.Instance.noFreezeTimeAtEye && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse)
            {
                __instance._timeFrozen = false;
                OWTime.Unpause(OWTime.PauseType.Reading);
                QOLFixes.Instance.ModHelper.Console.WriteLine($"QOLFixes: Canceled time freeze for {__instance._characterName} dialogue.");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
        [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
        public static bool ProbePromptEnterExit()
        {
            if (QOLFixes.Instance.noAutoScoutEquip)
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
            if (QOLFixes.Instance.eyesAlwaysGlow)
            {
                __instance._eyeGlow = 1f;
            }
        }
    }
}
