using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components;

[HarmonyPatch]
public class ManualTranslatorEquipper : MonoBehaviour
{
    private ToolModeSwapper _toolModeSwapper;

    private void Awake()
    {
        _toolModeSwapper = GetComponent<ToolModeSwapper>();
    }

    private void Update()
    {
        if (ModMain.CanManuallyEquipTranslator && OWInput.IsInputMode(InputMode.Character) && Keyboard.current[Key.T].wasPressedThisFrame && !PlayerState.InDreamWorld())
        {
            _toolModeSwapper.EquipToolMode(ToolMode.Translator);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.Awake))]
    private static void OnToolModeSwapperAwake(ToolModeSwapper __instance)
    {
        __instance.gameObject.AddComponent<ManualTranslatorEquipper>();
        ModMain.Instance.Log($"{nameof(ManualTranslatorEquipper)} added to {__instance.name}", OWML.Common.MessageType.Debug);
    }
}