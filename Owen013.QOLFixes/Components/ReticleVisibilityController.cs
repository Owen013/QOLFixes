using UnityEngine;

namespace QOLFixes.Components;

public class ReticleVisibilityController : MonoBehaviour
{
    private NomaiTranslatorProp _translatorTool;
    private ProbeLauncher[] _probeLaunchers;

    private void Start()
    {
        _translatorTool = FindObjectOfType<NomaiTranslatorProp>();
        _probeLaunchers = FindObjectsOfType<ProbeLauncher>();
    }

    private void Update()
    {
        if (Main.Instance.IsReticleDisabled == "No")
        {
            ReticleController.s_hideReticle = false;
            return;
        }
        else if (Main.Instance.IsReticleDisabled == "Unless using Tool")
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