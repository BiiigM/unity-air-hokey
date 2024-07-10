using System;
using System.Linq;
using CustomDevices;
using UnityEngine;
using UnityEngine.InputSystem;
using MultiMouseDevice = CustomDevices.MultiMouseDevice;

public class PlayerInputHandler : MonoBehaviour
{
    //Map Orientations
    private GameObject _mapCenter;
    private GameObject _mapTopRight;

    private PlayerInput _playerInput;
    private PlayerMover _playerMover;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        var playerMovers = FindObjectsByType<PlayerMover>(FindObjectsSortMode.None);
        var index = _playerInput.playerIndex;
        _playerMover = playerMovers.FirstOrDefault(mover => mover.GetPlayerIndex() == index);

        _mapCenter = GameObject.Find("MapCenter");
        _mapTopRight = GameObject.Find("MapTopRight");
    }

    public void Reset(InputAction.CallbackContext context)
    {
        var resetScript = _playerMover.GetComponent<ResetScript>();
        if (resetScript != null) resetScript.ResetObject();
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_playerInput.devices[0].GetType() == typeof(HandTrackingDevice) ||
            _playerInput.devices[0].GetType() == typeof(MultiMouseDevice))
        {
            var vector2 = context.ReadValue<Vector2>();

            var handWorldPositionV2 = GetVector2FormMap(NormalizeHandPosition(vector2));
            var handWorldPosition = new Vector3(handWorldPositionV2.x, _mapCenter.transform.position.y,
                handWorldPositionV2.y);
            _playerMover.GoTo(handWorldPosition);
            return;
        }

        _playerMover.OnMove(context.ReadValue<Vector2>());
    }

    private Vector2 NormalizeHandPosition(Vector2 handPosition)
    {
        var width = 1280 / 2;
        var height = 720 / 2;

        var handX = handPosition.x;
        var handY = handPosition.y;

        return new Vector2((handX - width) / width, (handY - height) / height);
    }

    private Vector2 GetVector2FormMap(Vector2 normalizedHandPosition)
    {
        var vMapCenter = new Vector2(_mapCenter.transform.position.x, _mapCenter.transform.position.z);
        var vMapTopRight = new Vector2(_mapTopRight.transform.position.x, _mapTopRight.transform.position.z);

        var distance = new Vector2(Math.Abs(vMapTopRight.x) - Math.Abs(vMapCenter.x),
            Math.Abs(vMapTopRight.y) - Math.Abs(vMapCenter.y));
        return normalizedHandPosition * distance + vMapCenter;
    }
}