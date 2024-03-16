using UnityEngine;
using UnityEngine.InputSystem;

namespace QOLFixes.Components
{
    public class HelmetToggler : MonoBehaviour
    {
        PlayerSpacesuit _spacesuit;

        private void Awake()
        {
            _spacesuit = GetComponent<PlayerSpacesuit>();
            Main.Instance.OnConfigure += () =>
            {
                if (!Config.CanRemoveHelmet && _spacesuit.IsWearingSuit() && !_spacesuit.IsWearingHelmet())
                {
                    _spacesuit.PutOnHelmet();
                }
            };
        }

        private void Update()
        {
            if (!Config.CanRemoveHelmet) return;

            if (_spacesuit.IsWearingSuit() && OWInput.GetInputMode() != InputMode.Menu && Keyboard.current[Key.H].wasPressedThisFrame)
            {
                if (_spacesuit.IsWearingHelmet())
                {
                    _spacesuit.RemoveHelmet();
                }
                else
                {
                    _spacesuit.PutOnHelmet();
                }
            }
        }
    }
}