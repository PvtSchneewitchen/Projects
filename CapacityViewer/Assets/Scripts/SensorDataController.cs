using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using Windows.Storage.Streams;

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking;
#endif


public class SensorDataController : MonoBehaviour
{

    public float _fCapacity1 { get; private set; }
    public float _fCapacity2 { get; private set; }

    private string _multicastPort = "9000";
    private string _multicastAddress = "237.0.0.1";
    private string message;
#if !UNITY_EDITOR
    private DatagramSocket _socket;
    private IOutputStream outputStream;
    private DataWriter writer;
#endif

    private bool _bUseUdpData = true;

    void Start()
    {
        _fCapacity1 = 0.0f;
        _fCapacity2 = 0.0f;

        if (_bUseUdpData)
        {
#if !UNITY_EDITOR
            InitConnection();
#else
            Debug.Log("No UDP communication with editor");
#endif
        }

        if (!_bUseUdpData)
        {
            _fCapacity1 = 32.55f;
            _fCapacity2 = 16.75f;
        }
    }



    void Update()
    {
        if (!_bUseUdpData)
        {
            _fCapacity1 += 0.0019f;
            _fCapacity2 += 0.0008f;

            if (_fCapacity1 >= 32.63f)
                _fCapacity1 = 32.55f;
            if (_fCapacity2 >= 16.94f)
                _fCapacity2 = 16.75f;
        }

        MulticastAsync("Hello From Emulator!");
    }

#if !UNITY_EDITOR
    private async void InitConnection()
    {
        print("Initializing socket...");

        _socket = new DatagramSocket();
        _socket.Control.MulticastOnly = true;
        _socket.MessageReceived += Socket_MessageReceived;

        print("Listening enabled!");

        await _socket.BindServiceNameAsync(_multicastPort);
        _socket.JoinMulticastGroup(new HostName(_multicastAddress));
        outputStream = await _socket.GetOutputStreamAsync(new HostName(_multicastAddress), _multicastPort);
        writer = new DataWriter(outputStream);

        print("Socket initialized!");

        MulticastAsync("This is a TestMessage");

        print("Test message sent!");
    }

    public async void MulticastAsync(string message)
    {
        writer.WriteString(message);
        await writer.StoreAsync();
        print("sent: " + message);
    }


    private async void Socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
    {
        Stream inputStream = args.GetDataStream().AsStreamForRead();
        StreamReader streamReader = new StreamReader(inputStream);
        string recievedMessage = await streamReader.ReadLineAsync();

        //todo do THings with message
        print("received: " + recievedMessage);

        //var values = recievedMessage.Split(' ');
        //float temp1 = float.Parse(values[0]);
        //float temp2 = float.Parse(values[1]);
        //print(temp1);
        //print(temp2);
    }

#endif
}
