using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components;

public class TranslatorEquipper : MonoBehaviour
{
    private ToolModeSwapper _toolModeSwapper;

    internal static void AddToToolModeSwapper(ToolModeSwapper toolModeSwapper)
    {
        toolModeSwapper.gameObject.AddComponent<TranslatorEquipper>();
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(TranslatorEquipper)} added to {toolModeSwapper.name}", OWML.Common.MessageType.Debug);
    }

    private void Awake()
    {
        _toolModeSwapper = GetComponent<ToolModeSwapper>();
    }

    private void Update()
    {
        if (Config.ManualTranslatorEquipping && OWInput.IsInputMode(InputMode.Character) && Keyboard.current != null && Keyboard.current[Key.T].wasPressedThisFrame && !PlayerState.InDreamWorld())
        {
            if (_toolModeSwapper.GetToolMode() == ToolMode.Translator)
                _toolModeSwapper.EquipToolMode(ToolMode.None);
            else
                _toolModeSwapper.EquipToolMode(ToolMode.Translator);
        }
    }
}