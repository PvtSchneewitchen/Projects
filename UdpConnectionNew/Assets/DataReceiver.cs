
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
//using HoloToolkit.Unity;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;

#endif
public class DataReceiver : MonoBehaviour
{
    public string port = "6";
    public string externalIP_field = "192.168.178.20";
    public string externalPort_field = "6";

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();


#if !UNITY_EDITOR
    DatagramSocket socket;
#endif
    // use this for initialization
#if !UNITY_EDITOR
    async void Start()
    {
        Debug.Log("Waiting for a connection...");

        socket = new DatagramSocket();
        socket.MessageReceived += Socket_MessageReceived;

        HostName localIP = null;
        try
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            localIP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
            .SingleOrDefault(
                hn =>
                    hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                    == icp.NetworkAdapter.NetworkAdapterId);
            await socket.BindEndpointAsync(localIP, port);
            Debug.Log("bind");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            return;
        }
        
        await SendMessage("trigger from " + socket.Information.LocalAddress.ToString());

        Debug.Log("exit start");
    }

    private async System.Threading.Tasks.Task SendMessage(string message)
    {
        String externalIP = "192.168.178.20";
        String externalPort = "6";
        message = "From " + socket.Information.LocalAddress.ToString() + " to " + externalIP + " at " + externalPort;
        using (var stream = await socket.GetOutputStreamAsync(new Windows.Networking.HostName(externalIP), externalPort))
        {
            using (var writer = new Windows.Storage.Streams.DataWriter(stream))
            {
                var data = Encoding.UTF8.GetBytes(message);

                writer.WriteBytes(data);
                await writer.StoreAsync();
                Debug.Log("Sent: " + message);
            }
        }
    }
#else
    void Start()
    {

    }
#endif

    // Update is called once per frame
    void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            SendMessage(" mainthread trigger from " + socket.Information.LocalAddress.ToString());
        }
    }

#if !UNITY_EDITOR
    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
        Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        
        Debug.Log("GOT MESSAGE: " + "bufferl " + args.GetDataReader().UnconsumedBufferLength);
        //Read the message that was received from the UDP echo client.

        DataReader dr = args.GetDataReader();
        dr.ByteOrder = ByteOrder.LittleEndian;
        IBuffer buffer = dr.ReadBuffer(args.GetDataReader().UnconsumedBufferLength);
        
        double d1 = DataReader.FromBuffer(buffer).ReadDouble();
        double d2 = DataReader.FromBuffer(buffer).ReadDouble();
        
        byte[] bytes = new byte[16];
        bytes = buffer.ToArray(0, 16);

        Debug.Log("1: " + bytes[0]);
        Debug.Log("2: " + bytes[1]);
        Debug.Log("3: " + bytes[2]);
        Debug.Log("4: " + bytes[3]);
        Debug.Log("5: " + bytes[4]);
        Debug.Log("6: " + bytes[5]);
        Debug.Log("7: " + bytes[6]);
        Debug.Log("8: " + bytes[7]);

        double d3 = BitConverter.ToDouble(bytes, 0);
        double d4 = BitConverter.ToDouble(bytes, 8);

        //Debug.Log("Distance: " + d1);
        //Debug.Log("cap: " + d2);
        /*Debug.Log("Distance2: " + d3);
        Debug.Log("cap2: " + d4);*/

        if (ExecuteOnMainThread.Count == 0)
        {
            ExecuteOnMainThread.Enqueue(() =>
            {
                Debug.Log("ExecuteOnMainThread");
                //Thermostat.Temperature = float.Parse(message);
            });
        }
    }
#endif
}