using OWML.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components;

public class ManualTranslatorEquipper : MonoBehaviour
{
    private ToolModeSwapper _toolModeSwapper;

    private void Awake()
    {
        Main.Instance.Log($"{nameof(ManualTranslatorEquipper)} added to {gameObject.name}", MessageType.Debug);
        _toolModeSwapper = GetComponent<ToolModeSwapper>();
    }

    private void Update()
    {
        if (Config.CanManuallyEquipTranslator && Keyboard.current[Key.T].wasPressedThisFrame)
        {
            _toolModeSwapper.EquipToolMode(ToolMode.Translator);
        }
    }
}