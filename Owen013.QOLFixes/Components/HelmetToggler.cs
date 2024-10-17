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
        ModMain.Instance.OnConfigure += () =>
        {
            if (!ModMain.CanRemoveHelmet && _spacesuit.IsWearingSuit() && !_spacesuit.IsWearingHelmet())
            {
                _spacesuit.PutOnHelmet();
            }
        };
    }

    private void Update()
    {
        if (ModMain.CanRemoveHelmet && _spacesuit.IsWearingSuit() && OWInput.GetInputMode() != InputMode.Menu && Keyboard.current[Key.H].wasPressedThisFrame)
        {
            if (_spacesuit.IsWearingHelmet())
            {
                _spacesuit.RemoveHelmet();
            }
            else if (_spacesuit.IsWearingSuit())
            {
                _spacesuit.PutOnHelmet();
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSpacesuit), nameof(PlayerSpacesuit.Start))]
    private static void OnSpacesuitStart(PlayerSpacesuit __instance)
    {
        __instance.gameObject.AddComponent<HelmetToggler>();
        ModMain.Instance.Log($"{nameof(HelmetToggler)} added to {__instance.name}", OWML.Common.MessageType.Debug);
    }
}