using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

[StructLayout(LayoutKind.Sequential)]
public struct MultiMouseDeviceState : IInputStateTypeInfo
{
    public FourCC format => new('H', 'T', 'D');

    [InputControl(layout = "Vector2", displayName = "Mouse One Position", name = "mouseOnePosition")]
    public Vector2 mouseOnePosition;

    [InputControl(layout = "Vector2", displayName = "Mouse Two Position", name = "mouseTwoPosition")]
    public Vector2 mouseTwoPosition;
}

namespace CustomDevices
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Multi Mouse Device", stateType = typeof(MultiMouseDeviceState))]
    public class MultiMouseDevice : InputDevice
    {
        public Vector2Control mouseOnePosition { get; private set; }

        public Vector2Control mouseTwoPosition { get; private set; }

        public static MultiMouseDevice current { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            mouseOnePosition = GetChildControl<Vector2Control>("mouseOnePosition");
            mouseTwoPosition = GetChildControl<Vector2Control>("mouseTwoPosition");
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

        [MenuItem("Tools/Add multi mouse device")]
        public static void Initialize()
        {
            InputSystem.AddDevice<MultiMouseDevice>();
        }
    }
}