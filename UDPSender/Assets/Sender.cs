using System;
using MixedRealityNetworking;
using UnityEngine;

public class Sender : MonoBehaviour
{
	private readonly string Desktop = "192.168.178.20";
	private readonly string Laptop = "192.168.178.29";
	private readonly string Hololens = "192.168.178.28";
	private static bool _debug = true;
    private static bool _answerMessageSend = false;
    public static float StartTime;

    // Use this for initialization
    private void Start()
	{
		Action<NetworkMessage> readRequestMessage = SubscriptionMessages.ReadOpponentRequestMessage;
		Action<NetworkMessage> readAnswerMessage = SubscriptionMessages.ReadOpponentAnswerMessage;

		SocketClientManager.Subscribe(0, readRequestMessage);
		SocketClientManager.Subscribe(1, readAnswerMessage);

		GlobalDeclarations.SelfRequestMessage.Write(GlobalDeclarations.RequestContent);
		GlobalDeclarations.SelfAnswerMessage.Write(GlobalDeclarations.AnswerContent);

		SocketClientManager.Host = Hololens;
		SocketClientManager.Port = 6;
		SocketClientManager.VerboseMode = true;

		SocketClientManager.Connect();

	    StartTime = Time.time;
        Debug.Log("Init finished");
	}

	// Update is called once per frame
	private void Update()
	{
	    //UnityEngine.Debug.Log("Update()");
        if (!GlobalDeclarations.OpponentAnswerMessageRecieved)
		{
		    UnityEngine.Debug.Log("Sending Answer Message");
            SocketClientManager.SendMessage(GlobalDeclarations.SelfAnswerMessage);
		    _answerMessageSend = true;
        }
		else
		{
			if (_debug)
			{
				Debug.Log("CONNECTED!");
				_debug = false;
			}
		    if (!_answerMessageSend)
		    {
		        _answerMessageSend = true;
		        UnityEngine.Debug.Log("Sending Answer Message Once");
		        SocketClientManager.SendMessage(GlobalDeclarations.SelfAnswerMessage);
		    }
		    float actualTime = Time.time;
		    if (actualTime - StartTime > 0.1f)
		    {
		        //Send Float Values
		        NetworkMessage floatMessageLocal = new NetworkMessage(12);
		        floatMessageLocal.Write(GlobalDeclarations.TestFloat);
		        SocketClientManager.SendMessage(floatMessageLocal);
		        GlobalDeclarations.TestFloat--;
		        GlobalDeclarations.CheckValueBoundaries();
		        StartTime = Time.time;
		    }
            
        }
	}

	public static class GlobalDeclarations
	{
		public static NetworkMessage SelfRequestMessage = new NetworkMessage(0);
		public static NetworkMessage SelfAnswerMessage = new NetworkMessage(1);
		public static NetworkMessage ByteMessage = new NetworkMessage(10);
		public static NetworkMessage IntMessage = new NetworkMessage(11);
		public static NetworkMessage FloatMessage = new NetworkMessage(12);

		public static int RequestContent = 999;
		public static int AnswerContent = 111;
		public static byte TestByte = 254;
		public static int TestInt = 200;
		public static float TestFloat = 100.0f;

		public static bool OpponentRequestMessageRecieved = false;
		public static bool OpponentAnswerMessageRecieved = false;

		public static bool SelfConnected = false;
		public static bool OpponentConnected = false;

		public static void CheckValueBoundaries()
		{
			if (TestByte == 0) TestByte = 254;
			if (TestInt == 0) TestInt = 200;
			if (TestFloat < 0.5f) TestFloat = 100.0f;
		}
	}

	public static class SubscriptionMessages
	{
		public static void ReadOpponentRequestMessage(NetworkMessage inputmessMessage)
		{
			UnityEngine.Debug.Log("Potential request message recieved");
			var value = inputmessMessage.ReadInt();
			if (value == 999)
				GlobalDeclarations.OpponentRequestMessageRecieved = true;
		}

		public static void ReadOpponentAnswerMessage(NetworkMessage inputmessMessage)
		{
			//UnityEngine.Debug.Log("Potential answer message recieved");
			var value = inputmessMessage.ReadInt();
		    if (value == 111)
		    {
		        GlobalDeclarations.OpponentAnswerMessageRecieved = true;
		        //UnityEngine.Debug.Log("answer message valid");
            }
		    else
		    {
		      UnityEngine.Debug.Log("answer message not valid");
            }
		}
	}
}


/*void Start()
{
SocketClientManager.Host = Laptop;
SocketClientManager.Port = 6;
SocketClientManager.Connect();

UnityEngine.Debug.Log("Init finished");

GlobalDeclarations.SelfRequestMessage.Write(GlobalDeclarations.RequestContent);
SocketClientManager.SendMessage(GlobalDeclarations.SelfRequestMessage);

UnityEngine.Debug.Log("Testmessage send");
}

// Update is called once per frame
void Update()
{
UnityEngine.Debug.Log("Update entry");
GlobalDeclarations.ByteMessage.Write(GlobalDeclarations.TestByte);
GlobalDeclarations.IntMessage.Write(GlobalDeclarations.TestInt);
GlobalDeclarations.FloatMessage.Write(GlobalDeclarations.TestFloat);

SocketClientManager.SendMessage(GlobalDeclarations.ByteMessage);
SocketClientManager.SendMessage(GlobalDeclarations.IntMessage);
SocketClientManager.SendMessage(GlobalDeclarations.FloatMessage);

UnityEngine.Debug.Log("Messages send");

GlobalDeclarations.TestByte--;
GlobalDeclarations.TestInt--;
GlobalDeclarations.TestFloat--;

GlobalDeclarations.CheckValueBoundaries();
}*/

/*    //if (!GlobalDeclarations.SelfConnected)
{
	if (!GlobalDeclarations.OpponentAnswerMessageRecieved)
	{
		UnityEngine.Debug.Log("Sending Request");
		SocketClientManager.SendMessage(GlobalDeclarations.SelfRequestMessage);
	}
	else
	{
		UnityEngine.Debug.Log("ANSWER RECIEVED");
		GlobalDeclarations.SelfConnected = true;

	}
}
//if (!GlobalDeclarations.OpponentConnected)
{
	if (!GlobalDeclarations.OpponentRequestMessageRecieved)
	{
		UnityEngine.Debug.Log("Waiting for request");
	}
	else
	{
		UnityEngine.Debug.Log("Request recieved, sending answer message");
		SocketClientManager.SendMessage(GlobalDeclarations.SelfAnswerMessage);
		GlobalDeclarations.OpponentConnected = true;

	}
}
if (GlobalDeclarations.SelfConnected && GlobalDeclarations.OpponentConnected)
{
	UnityEngine.Debug.Log("CONNECTED!!!");
	//TODO connected behavior
}
*/