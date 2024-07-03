using System.Linq;
using CustomDevices;
using MultiMouseUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private float _cameraZDistance;
    private ReceiveHandData _handData;
    private Vector2 _handPosition = Vector2.zero;
    private Camera _mainCamera;
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
        _handData = FindObjectsByType<ReceiveHandData>(FindObjectsSortMode.None)[0];
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

            _handPosition = new Vector2(
                Mathf.Clamp(vector2.x, 0, Screen.width),
                Mathf.Clamp(vector2.y, 0, Screen.height));

            var handWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(_handPosition.x, _handPosition.y,
                _cameraZDistance));
            var handDirection = (handWorldPosition - playerPosition).normalized;
            if (Vector3.Distance(handWorldPosition, playerPosition) < 0.15)
            {
                _playerMover.OnMove(Vector2.zero);
                return;
            }

            _playerMover.OnMove(new Vector2(handDirection.x, handDirection.z));
            return;
        }

        _playerMover.OnMove(context.ReadValue<Vector2>());
    }
}