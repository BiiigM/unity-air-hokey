using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ReceiveHandData : MonoBehaviour
{
    [SerializeField] private int portHandLeft = 5052;
    [SerializeField] private int portHandRight = 5053;
    [SerializeField] private bool startReceiving = true;
    [SerializeField] private bool printToConsole;
    public string handDataLeft;
    public string handDataRight;

    public void Start()
    {
        new Thread(
            () => ReceiveData(portHandLeft, ref handDataLeft, s => print("Hand one: " + s)))
        {
            IsBackground = true
        }.Start();
        new Thread(
            () => ReceiveData(portHandRight, ref handDataRight, s => print("Hand two: " + s)))
        {
            IsBackground = true
        }.Start();
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