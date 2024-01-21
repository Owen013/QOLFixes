using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Collections.Generic;
using QOLFixes.Components;

namespace QOLFixes
{
    public class Main : ModBehaviour
    {
        // Mod vars
        public static Main Instance;
        public delegate void ConfigureEvent();
        public event ConfigureEvent OnConfigure;

        // config
        public bool IsFreezeTimeAtEyeDisabled;
        public bool DisableAutoScoutEquip;
        public bool IsCancelDialogueEnabled;
        public bool IsEyesAlwaysGlowEnabled;
        public string IsReticleDisabled;
        public bool IsDebugLogEnabled;

        public void DebugLog(string text, MessageType type = MessageType.Message, bool forceMessage = false)
        {
            if (!IsDebugLogEnabled && !forceMessage) return;
            ModHelper.Console.WriteLine(text, type);
        }

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            IsFreezeTimeAtEyeDisabled = config.GetSettingsValue<bool>("DisableFreezeTime");
            DisableAutoScoutEquip = config.GetSettingsValue<bool>("DisableAutoScoutEquip");
            IsCancelDialogueEnabled = config.GetSettingsValue<bool>("ExitDialogue");
            IsEyesAlwaysGlowEnabled = config.GetSettingsValue<bool>("EyesAlwaysGlow");
            IsReticleDisabled = config.GetSettingsValue<string>("DisableReticle");

            if (OnConfigure != null)
            {
                OnConfigure();
            }
        }

        private void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Main));
        }

        private void Start()
        {
            DebugLog("Quality of Life Changes is ready to go!", MessageType.Success);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
        private static void ReticleControllerAwake(ReticleController __instance)
        {
            __instance.gameObject.AddComponent<ReticleVisibilityController>();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
        public static void DialogueTreeUpdate(CharacterDialogueTree __instance)
        {
            if (Instance.IsCancelDialogueEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
            {
                __instance.EndConversation();
                Instance.ModHelper.Console.WriteLine($"QOLFixes: Exited dialogue for {__instance._characterName}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
        public static void DialogueConversationStart(CharacterDialogueTree __instance)
        {
            if (Instance.IsFreezeTimeAtEyeDisabled && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse)
            {
                __instance._timeFrozen = false;
                OWTime.Unpause(OWTime.PauseType.Reading);
                Instance.ModHelper.Console.WriteLine($"QOLFixes: Canceled time freeze for {__instance._characterName} dialogue.");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.GainFocus))]
        [HarmonyPatch(typeof(ProbePromptReceiver), nameof(ProbePromptReceiver.LoseFocus))]
        public static bool ProbePromptEnterExit()
        {
            if (Instance.DisableAutoScoutEquip)
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
            if (Instance.IsEyesAlwaysGlowEnabled)
            {
                __instance._eyeGlow = 1f;
            }
        }
    }
}