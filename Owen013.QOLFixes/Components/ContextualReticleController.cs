using HarmonyLib;
using UnityEngine;

namespace QOLFixes.Components;

[HarmonyPatch]
public class ContextualReticleController : MonoBehaviour
{
    private NomaiTranslatorProp _translatorTool;

    private ProbeLauncher[] _probeLaunchers;

    private void Awake()
    {
        _translatorTool = FindObjectOfType<NomaiTranslatorProp>();
        _probeLaunchers = FindObjectsOfType<ProbeLauncher>();
    }

    private void Update()
    {
        if (ModMain.Instance.ReticleMode == "Vanilla")
        {
            ReticleController.s_hideReticle = false;
            return;
        }
        else if (ModMain.Instance.ReticleMode == "Contextual")
        {
            if (_translatorTool.enabled)
            {
                ReticleController.s_hideReticle = false;
                return;
            }

            for (int i = 0; i < _probeLaunchers.Length; i++)
            {
                if (_probeLaunchers[i].IsEquipped())
                {
                    ReticleController.s_hideReticle = false;
                    return;
                }
            }
        }
        ReticleController.s_hideReticle = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ReticleController), nameof(ReticleController.Awake))]
    private static void OnReticleControllerAwake(ReticleController __instance)
    {
        __instance.gameObject.AddComponent<ContextualReticleController>();
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(ContextualReticleController)} added to {__instance.name}", OWML.Common.MessageType.Debug);
    }
}