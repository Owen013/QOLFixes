using OWML.ModHelper;

namespace QOLFixes
{
    public class QOLFixes : ModBehaviour
    {
        private void Start()
        {
            ModHelper.HarmonyHelper.AddPostfix<CharacterDialogueTree>(
                "Update", typeof(Patches), nameof(Patches.DialogueTreeUpdate));
        }
    }

    public static class Patches
    {
        public static void DialogueTreeUpdate(CharacterDialogueTree __instance)
        {
            if (OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue)) __instance.EndConversation();
        }
    }
}