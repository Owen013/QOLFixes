using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components;

public class ManualTranslatorEquipper : MonoBehaviour
{
    private ToolModeSwapper _toolModeSwapper;

    private void Awake()
    {
        Main.Instance.Log($"{GetType().Name} added to {gameObject.name}", OWML.Common.MessageType.Debug);
        _toolModeSwapper = GetComponent<ToolModeSwapper>();
    }

    private void Update()
    {
        if (Config.CanManuallyEquipTranslator && OWInput.GetInputMode() != InputMode.Menu && Keyboard.current[Key.T].wasPressedThisFrame)
        {
            _toolModeSwapper.EquipToolMode(ToolMode.Translator);
        }
    }
}