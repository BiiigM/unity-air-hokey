using System.Linq;
using CustomDevices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace HandTracking
{
    [ExecuteInEditMode]
    public class HandTrackingSupport : MonoBehaviour
    {
        protected void Awake()
        {
            InputSystem.RegisterLayout<HandTrackingDevice>(
                matches: new InputDeviceMatcher()
                    .WithInterface("CameraBigM"));
        }

        protected void OnEnable()
        {
            ReceiveHandData.deviceAdded += OnDeviceAdded;
            ReceiveHandData.deviceRemoved += OnDeviceRemoved;
        }

        protected void OnDisable()
        {
            ReceiveHandData.deviceAdded -= OnDeviceAdded;
            ReceiveHandData.deviceRemoved -= OnDeviceRemoved;
        }

        private void OnDeviceAdded(string name)
        {
            InputSystem.AddDevice(
                new InputDeviceDescription
                {
                    interfaceName = "CameraBigM",
                    product = name
                });
        }

        private void OnDeviceRemoved(string name)
        {
            var device = InputSystem.devices.FirstOrDefault(
                x => x.description == new InputDeviceDescription
                {
                    interfaceName = "CameraBigM",
                    product = name
                });

            if (device != null)
                InputSystem.RemoveDevice(device);
        }
    }
}