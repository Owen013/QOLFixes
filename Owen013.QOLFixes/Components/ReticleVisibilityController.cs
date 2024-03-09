using OWML.Common;
using UnityEngine;

namespace QOLFixes.Components;

public class ContextualReticleController : MonoBehaviour
{
    private NomaiTranslatorProp _translatorTool;
    private ProbeLauncher[] _probeLaunchers;

    private void Awake()
    {
        Main.Instance.Log($"{nameof(ContextualReticleController)} added to {gameObject.name}", MessageType.Debug);
        _translatorTool = FindObjectOfType<NomaiTranslatorProp>();
        _probeLaunchers = FindObjectsOfType<ProbeLauncher>();
    }

    private void Update()
    {
        if (Config.ReticleMode == "Vanilla")
        {
            ReticleController.s_hideReticle = false;
            return;
        }
        else if (Config.ReticleMode == "Contextual")
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
}