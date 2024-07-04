using System;
using System.Linq;
using CustomDevices;
using MultiMouseUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private float _cameraZDistance;
    private Camera _mainCamera;

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
        MultiMouseWrapper.OnLeftMouseButtonDown[index] += HideCursor;
        _mainCamera = Camera.main;
        _cameraZDistance = _mainCamera.WorldToScreenPoint(_playerMover.transform.position).z;

        _mapCenter = GameObject.Find("MapCenter");
        _mapTopRight = GameObject.Find("MapTopRight");
    }

    public void Reset(InputAction.CallbackContext context)
    {
        var resetScript = _playerMover.GetComponent<ResetScript>();
        if (resetScript != null) resetScript.ResetObject();
    }

    private void Update()
    {
        if (true) return;
        if (!MultiMouseWrapper.Instance.IsMouseActive(_playerInput.playerIndex)) return;
        var playerIndex = _playerInput.playerIndex;
        var playerPosition = _playerMover.transform.position;
        var mousePosition = MultiMouseWrapper.Instance.GetMousePosition(playerIndex);
        var mouseWorldPosition =
            _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                _cameraZDistance));
        var direction = (mouseWorldPosition - playerPosition).normalized;
        if (Vector3.Distance(mouseWorldPosition, playerPosition) < 0.3)
        {
            _playerMover.OnMove(Vector2.zero);
            return;
        }

        _playerMover.OnMove(new Vector2(direction.x, direction.z));
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_playerInput.devices[0].GetType() == typeof(HandTrackingDevice))
        {
            var playerPosition = _playerMover.transform.position;
            var vector2 = context.ReadValue<Vector2>();

            var handWorldPositionV2 = GetVector2FormMap(NormalizeHandPosition(vector2));
            var handWorldPosition = new Vector3(handWorldPositionV2.x, _mapCenter.transform.position.y,
                handWorldPositionV2.y);

            var handDirection = (handWorldPosition - playerPosition).normalized;

            _playerMover.OnMove(new Vector2(handDirection.x, handDirection.z) *
                                Vector3.Distance(handWorldPosition, playerPosition));
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