using System.Reflection;
using UnityEngine;
using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;

namespace QOLFixes
{
    public class QOLFixes : ModBehaviour
    {
        // Config vars
        public bool fastDialogueExitEnabled, noFreezeTimeAtEye, screamEnabled;

        // Mod vars
        public static QOLFixes Instance;
        public OWAudioSource screamSource;
        AudioClip screamClip;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            fastDialogueExitEnabled = config.GetSettingsValue<bool>("Press Q to Exit Dialogue");
            noFreezeTimeAtEye = config.GetSettingsValue<bool>("Don't Freeze Time at a Certain End-Game Location");
            screamEnabled = false;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ModHelper.HarmonyHelper.AddPostfix<CharacterDialogueTree>("Update", typeof(Patches), nameof(Patches.DialogueTreeUpdate));
            ModHelper.HarmonyHelper.AddPostfix<CharacterDialogueTree>("StartConversation", typeof(Patches), nameof(Patches.DialogueConversationStart));
            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            screamClip = ModHelper.Assets.GetAudio("Assets/AAAAAAAAA.wav");

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                ModHelper.Events.Unity.FireInNUpdates(() =>
                {
                    screamSource = new GameObject(name = "QOL_ScreamSource").AddComponent<OWAudioSource>();
                    screamSource.clip = screamClip;
                    screamSource.playOnAwake = false;
                }, 60);
                    

            };
            
        }
    }

    public static class Patches
    {
        public static void DialogueTreeUpdate(CharacterDialogueTree __instance)
        {
            if (QOLFixes.Instance.fastDialogueExitEnabled && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Dialogue))
            {
                __instance.EndConversation();
                QOLFixes.Instance.ModHelper.Console.WriteLine($"QOLFixes: Exited dialogue for {__instance._characterName}");
            }
        }

        public static void DialogueConversationStart(CharacterDialogueTree __instance)
        {
            if (QOLFixes.Instance.noFreezeTimeAtEye && LoadManager.s_currentScene == OWScene.EyeOfTheUniverse)
            {
                __instance._timeFrozen = false;
                OWTime.Unpause(OWTime.PauseType.Reading);
                QOLFixes.Instance.ModHelper.Console.WriteLine($"QOLFixes: Canceled time freeze for {__instance._characterName} dialogue.");
            }
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(GhostBrain), nameof(GhostBrain.ChangeAction), typeof(GhostAction))]
        //public static void GhostChangeAction(GhostBrain __instance)
        //{
        //    QOLFixes.Instance.ModHelper.Console.WriteLine(__instance._currentAction.GetName());
        //    if (QOLFixes.Instance.screamEnabled && __instance._currentAction.GetName() == GhostAction.Name.Chase)
        //    {
        //        QOLFixes.Instance.screamSource.Stop();
        //        QOLFixes.Instance.screamSource.Play();
        //    }
        //}
    }
}