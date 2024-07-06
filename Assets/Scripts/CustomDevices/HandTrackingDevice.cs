using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

[StructLayout(LayoutKind.Sequential)]
public struct HandTrackingDeviceState : IInputStateTypeInfo
{
    public FourCC format => new('H', 'T', 'D');

    [InputControl(layout = "Vector2", displayName = "Left Position", name = "leftPosition")]
    public Vector2 leftPosition;

    [InputControl(layout = "Vector2", displayName = "Right Position", name = "rightPosition")]
    public Vector2 rightPosition;
}

namespace CustomDevices
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Hand Tracking Device", stateType = typeof(HandTrackingDeviceState))]
    public class HandTrackingDevice : InputDevice
    {
        public Vector2Control leftPosition { get; private set; }

        public Vector2Control rightPosition { get; private set; }

        public static HandTrackingDevice current { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            leftPosition = GetChildControl<Vector2Control>("leftPosition");
            rightPosition = GetChildControl<Vector2Control>("rightPosition");
        }

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }

        [MenuItem("Tools/Add hand tracking device")]
        public static void Initialize()
        {
            InputSystem.AddDevice<HandTrackingDevice>();
        }
    }
}