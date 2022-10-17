using UnityEngine;
using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;

namespace QOLFixes
{
    public class QOLFixes : ModBehaviour
    {
        // Config vars
        public bool fastDialogueExitEnabled, noFreezeTimeAtEye, noAutoScoutEquip, screamEnabled;

        // Mod vars
        public static QOLFixes Instance;
        public OWAudioSource screamSource;
        AudioClip screamClip;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            fastDialogueExitEnabled = config.GetSettingsValue<bool>("Press Q to Exit Dialogue");
            noFreezeTimeAtEye = config.GetSettingsValue<bool>("Don't Freeze Time at a Certain End-Game Location");
            noAutoScoutEquip = config.GetSettingsValue<bool>("Disable Automatic Scout Equipping");
            screamEnabled = false;
        }

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Patches));

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                ModHelper.Events.Unity.FireInNUpdates(() =>
                {
                    if (loadScene != OWScene.SolarSystem) return;
                    screamSource = new GameObject(name = "QOL_ScreamSource").AddComponent<OWAudioSource>();
                    screamSource.transform.parent = FindObjectOfType<PlayerAudioController>().transform;
                    screamSource.transform.localPosition = Vector3.zero;
                    screamSource.clip = screamClip;
                    screamSource.playOnAwake = false;
                    screamSource.SetMaxVolume(0.03125f);
                }, 60);
                    

            };
            
        }
    }

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GhostBrain), nameof(GhostBrain.ChangeAction), typeof(GhostAction))]
        public static void GhostChangeAction(GhostBrain __instance)
        {
            if (QOLFixes.Instance.screamEnabled && __instance._currentAction.GetName() == GhostAction.Name.Chase)
            {
                QOLFixes.Instance.screamSource.Stop();
                QOLFixes.Instance.screamSource.PlayDelayed(0.5f);
            }
        }
    }
}