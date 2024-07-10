using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private int playerIndex;

    private bool _goTo;

    private Vector2 _movementInput = Vector2.zero;
    private Vector3 _moveToPoint = Vector2.zero;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_goTo)
        {
            var move = new Vector3(_movementInput.x, 0, _movementInput.y);
            _rb.velocity = move * playerSpeed;
        }
        else
        {
            var handDirection = (_moveToPoint - transform.position).normalized;
            var distance = Vector3.Distance(_moveToPoint, transform.position);
            _rb.velocity = (handDirection * distance) * playerSpeed;
        }
    }

    public void OnMove(Vector2 movementVector)
    {
        _movementInput = movementVector;
    }

    public void GoTo(Vector3 moveToPoint)
    {
        _goTo = true;
        _moveToPoint = moveToPoint;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }
}