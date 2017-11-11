
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
        SendMessage("Request from " + socket.Information.LocalAddress.ToString());
    }

#if !UNITY_EDITOR
    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
        Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        uint doublesToRead = args.GetDataReader().UnconsumedBufferLength/8;
        double distance = 99;
        double capacity1 = 99;
        double capacity2 = 99;
        IInputStream inputStream = args.GetDataStream();
        DataReader dataReader = new DataReader(inputStream);

        uint chunkSize = await dataReader.LoadAsync(args.GetDataReader().UnconsumedBufferLength);
        while (doublesToRead > 0)
        {
            double actualDouble = dataReader.ReadDouble();
            if (doublesToRead == 1)
                capacity2 = actualDouble;
            else if (doublesToRead == 2)
                capacity1 = actualDouble;
            else if (doublesToRead == 3)
                distance = actualDouble;
            doublesToRead--; 
        }
        UpdateElectricField(distance, capacity1, capacity2);
    }
#endif
    private static void UpdateElectricField(double distance, double capacity1, double capacity2)
    {
        Debug.Log("d: " + distance);
        Debug.Log("c1: " + capacity1);
        Debug.Log("c2: " + capacity2);
    }
}