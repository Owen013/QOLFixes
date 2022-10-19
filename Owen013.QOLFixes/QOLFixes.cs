using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;

namespace QOLFixes
{
    public class QOLFixes : ModBehaviour
    {
        // Config vars
        public bool noFreezeTimeAtEye, noAutoScoutEquip, fastDialogueExitEnabled, eyesAlwaysGlow;

        // Mod vars
        public static QOLFixes Instance;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            noFreezeTimeAtEye = config.GetSettingsValue<bool>("Don't Freeze Time at a Certain End-Game Location");
            noAutoScoutEquip = config.GetSettingsValue<bool>("Disable Automatic Scout Equipping");
            fastDialogueExitEnabled = config.GetSettingsValue<bool>("Press Q to Exit Dialogue (Not Vanilla-Friendly)");
            eyesAlwaysGlow = config.GetSettingsValue<bool>("Eyes Always Glow (Not Vanilla-Friendly)");
        }

        public void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Patches));
        }

        public void Start()
        {
            PrintLog("Quality of Life Changes is ready to go!", MessageType.Success);
        }

        public void PrintLog(string message)
        {
            ModHelper.Console.WriteLine(message);
        }

        public void PrintLog(string message, MessageType messageType)
        {
            ModHelper.Console.WriteLine(message, messageType);
        }
    }
}