using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;

    [SerializeField] private float playerSprintMultiplier = 2f;

    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController _controller;

    private Vector2 _movementInput = Vector2.zero;
    private bool _sprint;

    private void Start()
    {
        _controller = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        var move = new Vector3(_movementInput.x, gravityValue * Time.deltaTime, _movementInput.y);
        var newSpeed = playerSpeed;

        if (_sprint) newSpeed = playerSpeed * playerSprintMultiplier;

        _controller.Move(move * Time.deltaTime * newSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _sprint = context.action.triggered;
    }
}