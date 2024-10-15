using OWML.Common;

namespace QOLFixes;

public static class Config
{
    public static string ReticleMode { get; private set; }

    public static bool IsAutoScoutEquipDisabled { get; private set; }

    public static bool IsFreezeTimeAtEyeDisabled { get; private set; }

    public static bool IsEyesAlwaysGlowEnabled { get; private set; }

    public static bool IsCancelDialogueEnabled { get; private set; }

    public static bool CanRemoveHelmet { get; private set; }

    public static bool CanManuallyEquipTranslator { get; private set; }

    public static void UpdateConfig(IModConfig config)
    {
        ReticleMode = config.GetSettingsValue<string>("ReticleMode");
        IsAutoScoutEquipDisabled = config.GetSettingsValue<bool>("DisableAutoScoutEquip");
        IsFreezeTimeAtEyeDisabled = config.GetSettingsValue<bool>("DisableFreezeTime");
        IsEyesAlwaysGlowEnabled = config.GetSettingsValue<bool>("EyesAlwaysGlow");
        IsCancelDialogueEnabled = config.GetSettingsValue<bool>("ExitDialogue");
        CanRemoveHelmet = config.GetSettingsValue<bool>("CanRemoveHelmet");
        CanManuallyEquipTranslator = config.GetSettingsValue<bool>("CanManuallyEquipTranslator");
    }
}