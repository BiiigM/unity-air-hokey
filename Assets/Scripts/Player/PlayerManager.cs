using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;

        private void FixedUpdate()
        {
            // Workaround for splitting Keyboard
            if (Input.GetKeyDown(KeyCode.O) &&
                !PlayerInput.all.Select(input => input.currentControlScheme).Any("KeyboardP2".Contains))
                PlayerInput.Instantiate(playerPrefab, controlScheme: "KeyboardP2",
                    pairWithDevice: Keyboard.current);
            if (Input.GetKeyDown(KeyCode.E) &&
                !PlayerInput.all.Select(input => input.currentControlScheme).Any("KeyboardP1".Contains))
                PlayerInput.Instantiate(playerPrefab, controlScheme: "KeyboardP1",
                    pairWithDevice: Keyboard.current);
        }
    }
}