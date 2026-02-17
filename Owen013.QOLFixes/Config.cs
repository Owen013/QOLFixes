using OWML.Common;
using System;

namespace QOLFixes;

public static class Config
{
    public static string ReticleMode { get; private set; }

    public static bool DisableAutoScoutEquipping { get; private set; }

    public static bool DisableFreezeTime { get; private set; }

    public static bool EnableEyesAlwaysGlow { get; private set; }

    public static bool EnableDialogueCancelling { get; private set; }

    public static string HelmetTogglingMode { get; private set; }

    public static bool EnableManualTranslatorEquipping { get; private set; }

    public static event Action OnConfigure;

    internal static void Configure(IModConfig config)
    {
        ReticleMode = config.GetSettingsValue<string>("ReticleMode");
        DisableAutoScoutEquipping = config.GetSettingsValue<bool>("DisableAutoScoutEquipping");
        DisableFreezeTime = config.GetSettingsValue<bool>("DisableFreezeTime");
        EnableEyesAlwaysGlow = config.GetSettingsValue<bool>("EnableEyesAlwaysGlow");
        EnableDialogueCancelling = config.GetSettingsValue<bool>("EnableDialogueCancelling");
        HelmetTogglingMode = config.GetSettingsValue<string>("HelmetTogglingMode");
        EnableManualTranslatorEquipping = config.GetSettingsValue<bool>("EnableManualTranslatorEquipping");

        OnConfigure?.Invoke();
    }
}
