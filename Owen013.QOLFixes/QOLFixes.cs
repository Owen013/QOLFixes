using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;

namespace QOLFixes
{
    public class QOLFixesController : ModBehaviour
    {
        // Config vars
        public bool _noFreezeTimeAtEye;
        public bool _noAutoScoutEquip;
        public bool _fastDialogueExitEnabled;
        public bool _eyesAlwaysGlow;
        string _disableReticule;
        ReticleController _reticule;

        // Mod vars
        public static QOLFixesController Instance;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            _noFreezeTimeAtEye = config.GetSettingsValue<bool>("Don't Freeze Time at a Certain End-Game Location");
            _noAutoScoutEquip = config.GetSettingsValue<bool>("Disable Automatic Scout Equipping");
            _fastDialogueExitEnabled = config.GetSettingsValue<bool>("Press Q to Exit Dialogue");
            _eyesAlwaysGlow = config.GetSettingsValue<bool>("Eyes Always Glow");
            _disableReticule = config.GetSettingsValue<string>("Disable Reticule");

            UpdateReticule();
        }

        public void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(QOLFixesController));
        }

        public void Start()
        {
            PrintLog("Quality of Life Changes is ready to go!", MessageType.Success);
        }

        public void UpdateReticule()
        {
            if (_reticule == null) return;
            ProbeLauncher probe = FindObjectOfType<ProbeLauncher>();
            if (_disableReticule == "No" || (_disableReticule == "Unless using Scout") && probe.IsEquipped()) _reticule.gameObject.SetActive(true);
            else _reticule.gameObject.SetActive(false);
        }

        public void PrintLog(string message)
        {
            ModHelper.Console.WriteLine(message);
        }

        public void PrintLog(string message, MessageType messageType)
        {
            ModHelper.Console.WriteLine(message, messageType);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Update))]
        public static void DialogueTreeUpdate(CharacterDialogueTree __instance)
        {
            if (Instance._fastDialogueExitEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
            {
                __instance.EndConversation();
                Instance.ModHelper.Console.WriteLine($"QOLFixes: Exited dialogue for {__instance._characterName}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
        public static void DialogueConversationStart(CharacterDialogueTree __instance)
        {
            if (Instance._noFreezeTimeAtEye && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse)
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
            if (Instance._noAutoScoutEquip)
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
            if (Instance._eyesAlwaysGlow)
            {
                __instance._eyeGlow = 1f;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerCharacterController), nameof(PlayerCharacterController.Start))]
        [HarmonyPatch(typeof(ProbeLauncher), nameof(ProbeLauncher.EquipTool))]
        [HarmonyPatch(typeof(ProbeLauncher), nameof(ProbeLauncher.UnequipTool))]
        public static void ShowOrHideReticule()
        {
            Instance.UpdateReticule();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
        public static void GetReticule(ReticleController __instance)
        {
            Instance._reticule = __instance;
        }
    }
}