using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components;

[HarmonyPatch]
public class TranslatorEquipper : MonoBehaviour
{
    private ToolModeSwapper _toolModeSwapper;

    private void Awake()
    {
        _toolModeSwapper = GetComponent<ToolModeSwapper>();
    }

    private void Update()
    {
        if (ModMain.Instance.CanManuallyEquipTranslator && OWInput.IsInputMode(InputMode.Character) && Keyboard.current[Key.T].wasPressedThisFrame && !PlayerState.InDreamWorld())
        {
            _toolModeSwapper.EquipToolMode(ToolMode.Translator);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.Awake))]
    private static void OnToolModeSwapperAwake(ToolModeSwapper __instance)
    {
        __instance.gameObject.AddComponent<TranslatorEquipper>();
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(TranslatorEquipper)} added to {__instance.name}", OWML.Common.MessageType.Debug);
    }
}