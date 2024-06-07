using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;

    [SerializeField] private float playerSprintMultiplier = 2f;

    private Vector2 _movementInput = Vector2.zero;

    private Rigidbody _rb;
    private bool _sprint;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var move = new Vector3(_movementInput.x, 0, _movementInput.y);
        var newSpeed = playerSpeed;

        if (_sprint) newSpeed = playerSpeed * playerSprintMultiplier;

        _rb.velocity = move * newSpeed;
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