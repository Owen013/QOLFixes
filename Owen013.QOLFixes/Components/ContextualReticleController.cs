using QOLFixes.Enums;
using UnityEngine;

namespace QOLFixes.Components;

public class ContextualReticleController : MonoBehaviour
{
    private ToolModeSwapper _toolModeSwapper;

    internal static void AddToReticleController(ReticleController reticle)
    {
        reticle.gameObject.AddComponent<ContextualReticleController>();
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(ContextualReticleController)} added to {reticle.name}", OWML.Common.MessageType.Debug);
    }

    private void Awake()
    {
        _toolModeSwapper = Locator.GetToolModeSwapper();
    }

    private void Update()
    {
        ReticleController.s_hideReticle = Config.ReticleMode switch
        {
            ReticleMode.Contextual => !(_toolModeSwapper.IsInToolMode(ToolMode.Probe) || _toolModeSwapper.IsInToolMode(ToolMode.Translator)),
            ReticleMode.Hidden => true,
            _ => false
        };
    }
}