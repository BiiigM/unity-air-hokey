using System.Linq;
using MultiMouseUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private float _cameraZDistance;
    private PlayerInput _playerInput;
    private PlayerMover _playerMover;
    private Camera mainCamera;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        var playerMovers = FindObjectsByType<PlayerMover>(FindObjectsSortMode.None);
        var index = _playerInput.playerIndex;
        _playerMover = playerMovers.FirstOrDefault(mover => mover.GetPlayerIndex() == index);
        MultiMouseWrapper.OnLeftMouseButtonDown[index] += HideCursor;
        mainCamera = Camera.main;
        _cameraZDistance = mainCamera.WorldToScreenPoint(_playerMover.transform.position).z;
    }

    private void Update()
    {
        if (!MultiMouseWrapper.Instance.IsMouseActive(_playerInput.playerIndex)) return;
        var playerIndex = _playerInput.playerIndex;
        var playerPosition = _playerMover.transform.position;
        var mousePosition = MultiMouseWrapper.Instance.GetMousePosition(playerIndex);
        var mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
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
        Debug.Log(context.ReadValue<Vector2>());
        _playerMover.OnMove(context.ReadValue<Vector2>());
    }
}