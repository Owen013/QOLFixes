using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Reflection;

namespace QOLFixes;

public class ModMain : ModBehaviour
{
    public static ModMain Instance { get; private set; }

    public static string ReticleMode { get; private set; }

    public static bool IsAutoScoutEquipDisabled { get; private set; }

    public static bool IsFreezeTimeAtEyeDisabled { get; private set; }

    public static bool IsEyesAlwaysGlowEnabled { get; private set; }

    public static bool IsCancelDialogueEnabled { get; private set; }

    public static bool CanRemoveHelmet { get; private set; }

    public static bool CanManuallyEquipTranslator { get; private set; }

    public delegate void ConfigureEvent();

    public event ConfigureEvent OnConfigure;

    public override void Configure(IModConfig config)
    {
        base.Configure(config);

        ReticleMode = config.GetSettingsValue<string>("ReticleMode");
        IsAutoScoutEquipDisabled = config.GetSettingsValue<bool>("DisableAutoScoutEquip");
        IsFreezeTimeAtEyeDisabled = config.GetSettingsValue<bool>("DisableFreezeTime");
        IsEyesAlwaysGlowEnabled = config.GetSettingsValue<bool>("EyesAlwaysGlow");
        IsCancelDialogueEnabled = config.GetSettingsValue<bool>("ExitDialogue");
        CanRemoveHelmet = config.GetSettingsValue<bool>("CanRemoveHelmet");
        CanManuallyEquipTranslator = config.GetSettingsValue<bool>("CanManuallyEquipTranslator");

        OnConfigure?.Invoke();
    }

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void Start()
    {
        Log("Quality of Life Changes is ready to go!", MessageType.Success);
    }

    public void Log(string text, MessageType type = MessageType.Message)
    {
        ModHelper.Console.WriteLine(text, type);
    }
}