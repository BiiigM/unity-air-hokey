using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerMover _playerMover;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        var playerMovers = FindObjectsByType<PlayerMover>(FindObjectsSortMode.None);
        var index = _playerInput.playerIndex;
        _playerMover = playerMovers.FirstOrDefault(mover => mover.GetPlayerIndex() == index);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _playerMover.OnMove(context.ReadValue<Vector2>());
    }
}