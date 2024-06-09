using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private int playerIndex;

    private Vector2 _movementInput = Vector2.zero;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var move = new Vector3(_movementInput.x, 0, _movementInput.y);

        _rb.velocity = move * playerSpeed;
    }

    public void OnMove(Vector2 movementVector)
    {
        _movementInput = movementVector;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }
}