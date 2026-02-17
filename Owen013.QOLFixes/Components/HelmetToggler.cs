using QOLFixes.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace QOLFixes.Components;

public class HelmetToggler : MonoBehaviour
{
    public static HelmetToggler Instance { get; private set; }

    public bool IsHelmetRemoved { get; private set; }

    private PlayerSpacesuit _spacesuit;

    private PlayerCharacterController _playerController;

    private PlayerResources _resources;

    private NotificationDisplay _suitDisplay;

    private bool _wasOxygenPresent;

    public void PutOnHelmet()
    {
        if (IsHelmetRemoved && _spacesuit.IsWearingSuit() && !_spacesuit.IsWearingHelmet())
        {
            _spacesuit.PutOnHelmet();
        }

        IsHelmetRemoved = false;
    }

    public void RemoveHelmet()
    {
        if (!IsHelmetRemoved && _spacesuit.IsWearingSuit() && _spacesuit.IsWearingHelmet())
        {
            _spacesuit.RemoveHelmet();
        }

        IsHelmetRemoved = true;
    }

    internal static void AddToPlayerSpacesuit(PlayerSpacesuit spacesuit)
    {
        var helmetToggler = spacesuit.gameObject.AddComponent<HelmetToggler>();
        helmetToggler._spacesuit = spacesuit;
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(HelmetToggler)} added to {spacesuit.name}", OWML.Common.MessageType.Debug);
    }

    internal void OnUpdateOxygenPresence()
    {
        bool isOxygenPresent = _resources.IsOxygenPresent();
        if (_wasOxygenPresent && !isOxygenPresent)
            PutOnHelmet();

        _wasOxygenPresent = isOxygenPresent;
    }

    private void Awake()
    {
        Instance = this;
        _playerController = Locator.GetPlayerController();
        _resources = _playerController._playerResources;
        _suitDisplay = FindObjectOfType<SuitNotificationDisplay>();
        Config.OnConfigure += () =>
        {
            if (Config.HelmetTogglingMode == HelmetTogglingMode.Never)
                PutOnHelmet();
        };
    }

    private bool IsInputting()
    {
        bool canInput = OWInput.IsInputMode(InputMode.Character) && !_playerController._isMovementLocked;
        bool wasButtonPressed = Keyboard.current[Key.H].wasPressedThisFrame || Gamepad.current[GamepadButton.LeftStick].wasPressedThisFrame;
        return canInput && wasButtonPressed;
    }

    private void Update()
    {
        if (Config.HelmetTogglingMode != HelmetTogglingMode.Never)
        {
            if (!IsHelmetRemoved)
            {
                if (Config.HelmetTogglingMode == HelmetTogglingMode.Always || _resources.IsOxygenPresent())
                    RemoveHelmet();
                else
                    _suitDisplay.PushNotification(new NotificationData(NotificationTarget.Player, "CANNOT REMOVE HELMET — NO OXYGEN DETECTED", 3f));
            }
            else
                PutOnHelmet();
        }
    }
}