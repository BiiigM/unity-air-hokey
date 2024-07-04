using System.Linq;
using CustomDevices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private ReceiveHandData _handData;

        private void Start()
        {
            _handData = GetComponent<ReceiveHandData>();
        }

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

            // Hand tracking
            if (_handData && !string.IsNullOrEmpty(_handData.handDataLeft) && !PlayerInput.all
                    .Select(input => input.currentControlScheme)
                    .Any("HandTrackingP1".Contains))
                PlayerInput.Instantiate(playerPrefab, controlScheme: "HandTrackingP1",
                    pairWithDevice: HandTrackingDevice.current);
            if (_handData && !string.IsNullOrEmpty(_handData.handDataRight) && !PlayerInput.all
                    .Select(input => input.currentControlScheme)
                    .Any("HandTrackingP2".Contains))
                PlayerInput.Instantiate(playerPrefab, controlScheme: "HandTrackingP2",
                    pairWithDevice: HandTrackingDevice.current);
        }
    }
}