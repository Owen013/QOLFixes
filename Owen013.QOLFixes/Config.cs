using OWML.Common;
using System;

namespace QOLFixes;

public static class Config
{
    public static Enums.ReticleMode ReticleMode { get; private set; }

    public static Enums.HelmetTogglingMode HelmetTogglingMode { get; private set; }

    public static bool AutoPutOnHelmetWhenUnsafe { get; private set; }

    public static bool AutoRemoveHelmetWhenSafe { get; private set; }

    public static bool DisableAutoScoutEquipping { get; private set; }

    public static bool DisableFreezeTime { get; private set; }

    public static bool MoreVisibleGhosts { get; private set; }

    public static bool ManualTranslatorEquipping { get; private set; }

    public static bool EnableDialogueCancelling { get; private set; }

    public static event Action OnConfigure;

	internal static void Configure(IModConfig config)
    {
        ReticleMode = config.GetSettingsValue<string>("ReticleMode") switch
        {
            "Contextual" => Enums.ReticleMode.Contextual,
            "Hidden" => Enums.ReticleMode.Hidden,
            _ => Enums.ReticleMode.Default
        };

        HelmetTogglingMode = config.GetSettingsValue<string>("HelmetTogglingMode") switch
        {
            "When Safe" => Enums.HelmetTogglingMode.WhenSafe,
            "Always" => Enums.HelmetTogglingMode.Always,
            _ => Enums.HelmetTogglingMode.Never
        };
        AutoPutOnHelmetWhenUnsafe = config.GetSettingsValue<bool>("AutoPutOnHelmetWhenUnsafe");
        AutoRemoveHelmetWhenSafe = config.GetSettingsValue<bool>("AutoRemoveHelmetWhenSafe");

        DisableAutoScoutEquipping = config.GetSettingsValue<bool>("DisableAutoScoutEquipping");
        DisableFreezeTime = config.GetSettingsValue<bool>("DisableFreezeTime");
        MoreVisibleGhosts = config.GetSettingsValue<bool>("MoreVisibleGhosts");
        ManualTranslatorEquipping = config.GetSettingsValue<bool>("ManualTranslatorEquipping");
        EnableDialogueCancelling = config.GetSettingsValue<bool>("EnableDialogueCancelling");

        OnConfigure?.Invoke();
    }
}
