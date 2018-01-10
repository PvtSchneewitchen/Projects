
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
//using HoloToolkit.Unity;
using System.Collections.Generic;
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

	private static double dDistance = 1;
	private static double dCapacity1;
	private static double dCapacity2;

	public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action> ();


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
        //TODO check following variables
                String externalIP = externalIP_field;
                String externalPort = externalPort_field;

        message = "From " + socket.Information.LocalAddress.ToString() + " to " + externalIP + " at " + externalPort;
        using (var stream = await socket.GetOutputStreamAsync(new Windows.Networking.HostName(externalIP), externalPort))
        {
            using (var writer = new Windows.Storage.Streams.DataWriter(stream))
            {
                var data = Encoding.UTF8.GetBytes(message);

                writer.WriteBytes(data);
                await writer.StoreAsync();
                //Debug.Log("Sent: " + message);
            }
        }
    }

#else
    void Start ()
	{
		//TODO delete "simulation"
		dCapacity1 = 16.75f;
		dCapacity2 = 32.55f;
	}
#endif

    // Update is called once per frame
    void Update ()
	{
#if !UNITY_EDITOR
        //SendMessage("Request from " + socket.Information.LocalAddress.ToString());
#endif
		dCapacity1 += 0.0019;
		dCapacity2 += 0.0008;

		if (dCapacity1 >= 16.94f)
			dCapacity1 = 16.75f;
		if (dCapacity2 >= 32.63f)
			dCapacity2 = 32.55f;
	}

#if !UNITY_EDITOR
    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
        Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        StreamReader reader = new StreamReader(streamIn);
        string message = await reader.ReadLineAsync();

        //uint doublesToRead = args.GetDataReader().UnconsumedBufferLength / 8;

        //IInputStream inputStream = args.GetDataStream();
        //DataReader dataReader = new DataReader(inputStream);


        //uint chunkSize = await dataReader.LoadAsync(args.GetDataReader().UnconsumedBufferLength);
        //while (doublesToRead > 0)
        //{
        //    double actualDouble = dataReader.ReadDouble();
        //    if (doublesToRead == 1)
        //        dCapacity2 = actualDouble;
        //    else if (doublesToRead == 2)
        //        dCapacity1 = actualDouble;
        //    else if (doublesToRead == 3)
        //        dDistance = actualDouble;
        //    doublesToRead--;
        //}
    }
#endif

    public static float GetCapacity1 ()
	{
		return (float)dCapacity1;
	}

	public static float GetCapacity2 ()
	{
		return (float)dCapacity2;
	}
}