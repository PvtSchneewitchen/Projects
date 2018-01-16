using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using Windows.Networking;


public class MulticastSender : MonoBehaviour
{

    private string _multicastPort = "9000";
    private string _multicastAddress = "237.0.0.1";
    private DatagramSocket _socket;
    private IOutputStream outputStream;
    private DataWriter writer;
    private string message;

    // Use this for initialization
    void Start()
    {
       InitAsync();
        message = "Test123";
    }

    // Update is called once per frame
    void Update()
    {
        //MulticastAsync(message);
    }

    public async void InitAsync()
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

        print("received: " + recievedMessage);
    }
}
