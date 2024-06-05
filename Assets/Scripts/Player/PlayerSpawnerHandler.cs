using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnerHandler : MonoBehaviour
{
    [SerializeField] private GameObject playerSpawnOne;

    [SerializeField] private GameObject playerSpawnTwo;
    [SerializeField] private GameObject player;

    public void OnPlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("PLAYER JOIN");
    }
}