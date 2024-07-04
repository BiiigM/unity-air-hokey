using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CustomDevices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReceiveHandData : MonoBehaviour
{
    public static Action<string> deviceAdded;
    public static Action<string> deviceRemoved;
    [SerializeField] private int portHandLeft = 5052;
    [SerializeField] private int portHandRight = 5053;
    [SerializeField] private bool startReceiving = true;
    [SerializeField] private bool printToConsole;
    public string handDataLeft;
    public string handDataRight;
    private readonly List<Thread> _threads = new();

    public void Start()
    {
        _threads.Add(new Thread(
            () => ReceiveData(portHandLeft, ref handDataLeft, s => print("Hand one: " + s)))
        {
            IsBackground = true
        });
        _threads.Add(new Thread(
            () => ReceiveData(portHandRight, ref handDataRight, s => print("Hand two: " + s)))
        {
            IsBackground = true
        });
        _threads.ForEach(thread => thread.Start());
        deviceAdded("HandCamera");
    }

    public void Update()
    {
        var vHandDataLeft = StringToVector(handDataLeft);
        var vHandDataRight = StringToVector(handDataRight);

        if (vHandDataLeft.HasValue && !vHandDataRight.HasValue)
            InputSystem.QueueStateEvent(HandTrackingDevice.current, new HandTrackingDeviceState
            {
                leftPosition = vHandDataLeft.Value
            });
        if (vHandDataRight.HasValue && !vHandDataLeft.HasValue)
            InputSystem.QueueStateEvent(HandTrackingDevice.current, new HandTrackingDeviceState
            {
                rightPosition = vHandDataRight.Value
            });
        if (vHandDataRight.HasValue && vHandDataLeft.HasValue)
            InputSystem.QueueStateEvent(HandTrackingDevice.current, new HandTrackingDeviceState
            {
                leftPosition = vHandDataLeft.Value,
                rightPosition = vHandDataRight.Value
            });
    }

    public void OnDisable()
    {
        _threads.ForEach(thread => thread.Abort());
        if (deviceRemoved != null)
            deviceRemoved("HandCamera");
        else
            Debug.LogError("Device Remover is null");
    }

    private Vector2? StringToVector(string vectorString)
    {
        if (vectorString == null || !Regex.Match(vectorString, @"\(\d+, \d+\)", RegexOptions.None).Success) return null;
        var removeBrackets = vectorString.Replace("(", "").Replace(")", "");
        var splitString = removeBrackets.Split(", ");
        return new Vector2(int.Parse(splitString[0]), 720 - int.Parse(splitString[1]));
    }

    private void ReceiveData(int handPort, ref string handData, Action<string> debugFunction)
    {
        var client = new UdpClient(handPort);
        while (startReceiving)
            try
            {
                var anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), handPort);
                var dataByte = client.Receive(ref anyIP);
                handData = Encoding.UTF8.GetString(dataByte);

                if (printToConsole) debugFunction(handData);
            }
            catch (Exception error)
            {
                print(error);
            }
    }
}