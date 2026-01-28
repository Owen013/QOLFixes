using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Reflection;

namespace QOLFixes;

public class ModMain : ModBehaviour
{
    public static ModMain Instance { get; private set; }

    public string ReticleMode { get; private set; }

    public bool IsAutoScoutEquipDisabled { get; private set; }

    public bool IsFreezeTimeAtEyeDisabled { get; private set; }

    public bool IsEyesAlwaysGlowEnabled { get; private set; }

    public bool IsCancelDialogueEnabled { get; private set; }

    public bool CanRemoveHelmet { get; private set; }

    public bool CanManuallyEquipTranslator { get; private set; }

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
        ModHelper.Console.WriteLine("Quality of Life Changes is ready to go!", MessageType.Success);
    }
}