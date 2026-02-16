using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components;

[HarmonyPatch]
public class HelmetToggler : MonoBehaviour
{
    PlayerSpacesuit _spacesuit;

    private void Awake()
    {
        _spacesuit = GetComponent<PlayerSpacesuit>();
        Config.OnConfigure += () =>
        {
            if (!Config.EnableHelmetToggling && _spacesuit.IsWearingSuit() && !_spacesuit.IsWearingHelmet())
                _spacesuit.PutOnHelmet();
        };
    }

    private void Update()
    {
        if (Config.EnableHelmetToggling && _spacesuit.IsWearingSuit() && OWInput.GetInputMode() != InputMode.Menu && Keyboard.current[Key.H].wasPressedThisFrame)
        {
            if (_spacesuit.IsWearingHelmet())
                _spacesuit.RemoveHelmet();
            else if (_spacesuit.IsWearingSuit())
                _spacesuit.PutOnHelmet();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSpacesuit), nameof(PlayerSpacesuit.Start))]
    private static void OnSpacesuitStart(PlayerSpacesuit __instance)
    {
        __instance.gameObject.AddComponent<HelmetToggler>();
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(HelmetToggler)} added to {__instance.name}", OWML.Common.MessageType.Debug);
    }
}