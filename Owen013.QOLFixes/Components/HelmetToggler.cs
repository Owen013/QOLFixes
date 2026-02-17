using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

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
        IsHelmetRemoved = false;
        _spacesuit.PutOnHelmet();
    }

    public void RemoveHelmet()
    {
        IsHelmetRemoved = true;
        _spacesuit.RemoveHelmet();
    }

    internal static void AddToPlayerSpacesuit(PlayerSpacesuit spacesuit)
    {
        var helmetToggler = spacesuit.gameObject.AddComponent<HelmetToggler>();
        helmetToggler._spacesuit = spacesuit;
        ModMain.Instance.ModHelper.Console.WriteLine($"{nameof(HelmetToggler)} added to {spacesuit.name}", OWML.Common.MessageType.Debug);
    }

    internal void OnUpdateOxygenPresence()
    {
        if (!_spacesuit.IsWearingHelmet() && _wasOxygenPresent && !_resources.IsOxygenPresent())
            PutOnHelmet();

        _wasOxygenPresent = _resources.IsOxygenPresent();
    }

    private void Awake()
    {
        Instance = this;
        _playerController = Locator.GetPlayerController();
        _resources = _playerController._playerResources;
        _suitDisplay = FindObjectOfType<SuitNotificationDisplay>();
        Config.OnConfigure += () =>
        {
            if (Config.HelmetTogglingMode == "Never" && _spacesuit.IsWearingSuit() && !_spacesuit.IsWearingHelmet())
                PutOnHelmet();
        };
    }

    private bool IsInputting()
    {
        return OWInput.IsInputMode(InputMode.Character) && !_playerController._isMovementLocked && Keyboard.current[Key.H].wasPressedThisFrame;
    }

    private void Update()
    {
        if (Config.HelmetTogglingMode != "Never" && _spacesuit.IsWearingSuit() && IsInputting())
        {
            if (_spacesuit.IsWearingHelmet())
            {
                if (Config.HelmetTogglingMode == "Always" || _resources.IsOxygenPresent())
                    RemoveHelmet();
                else
                    _suitDisplay.PushNotification(new NotificationData(NotificationTarget.Player, "CANNOT REMOVE HELMET — NO OXYGEN DETECTED", 3f));
            }
            else if (_spacesuit.IsWearingSuit())
                PutOnHelmet();
        }
    }
}