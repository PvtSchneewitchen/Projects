using MixedRealityNetworking;
using System;
using UnityEngine;

public class UdpFloatReciever : MonoBehaviour {
    public static string Desktop = "192.168.178.20";
    public static string Laptop = "192.168.178.29";
    public static string Hololens = "192.168.178.28";
    private static bool _debug = true;
    private static bool _answerMessageSend = false;
    private GameObject _cube;
    private static float _lastValue;
    private static long _lastMessageRecieved = 0;

    // Use this for initialization
    void Start()
    {
        Debug.Log("INIT UDPRECIEVER");
        //Inits
        Action<NetworkMessage> readOpponentReuestMessage = SubscriptionMessages.ReadOpponentRequestMessage;
        Action<NetworkMessage> readOpponentAnswerMessage = SubscriptionMessages.ReadOpponentAnswerMessage;
        Action<NetworkMessage> readmessageByte = SubscriptionMessages.ReadMessageByte;
        Action<NetworkMessage> readmessageInt = SubscriptionMessages.ReadMessageInt;
        Action<NetworkMessage> readmessageFloat = SubscriptionMessages.ReadMessageFloat;

        //Define Message Id Handling
        SocketClientManager.Subscribe(0, readOpponentReuestMessage);
        SocketClientManager.Subscribe(1, readOpponentAnswerMessage);
        SocketClientManager.Subscribe(10, readmessageByte);
        SocketClientManager.Subscribe(11, readmessageInt);
        SocketClientManager.Subscribe(12, readmessageFloat);

        //Set Message content
        GlobalDeclarations.SelfRequestMessage.Write(GlobalDeclarations.RequestContent);
        GlobalDeclarations.SelfAnswerMessage.Write(GlobalDeclarations.AnswerContent);

        //Set Connection Information
        SocketClientManager.Host = Desktop;
        SocketClientManager.Port = 6;
        SocketClientManager.VerboseMode = true;

        //Connect to Udp Opponent
        SocketClientManager.Connect();

        _cube = GameObject.Find("CameraCube");
        _lastValue = GlobalDeclarations.FloatValue;
    }

    // Update is called once per frame
    void Update()
    {
        long elapsedTicks = DateTime.Now.Ticks - _lastMessageRecieved;
        if (new TimeSpan(elapsedTicks).Seconds > 15)
        {
            GlobalDeclarations.OpponentAnswerMessageRecieved = false;
            _answerMessageSend = false;
            _debug = true;
        }
        //UnityEngine.Debug.Log("Update()");
        if (!GlobalDeclarations.OpponentAnswerMessageRecieved)
        {
            //UnityEngine.Debug.Log("Sending Answer Message");
            SocketClientManager.SendMessage(GlobalDeclarations.SelfAnswerMessage);
            _answerMessageSend = true;
        }
        else
        {
            if (_debug)
            {
                UnityEngine.Debug.Log("Connected!");
                _debug = false;
            }
            if (!_answerMessageSend)
            {
                _answerMessageSend = true;
                UnityEngine.Debug.Log("Sending Answer Message Once");
                SocketClientManager.SendMessage(GlobalDeclarations.SelfAnswerMessage);
            }


            if(_lastValue != GlobalDeclarations.FloatValue) { 
                //UnityEngine.Debug.Log("Global Float Value:" + GlobalDeclarations.FloatValue);
                _cube.transform.localScale = new Vector3(GlobalDeclarations.FloatValue/100.0f, GlobalDeclarations.FloatValue / 100.0f, GlobalDeclarations.FloatValue / 100.0f);
                _lastValue = GlobalDeclarations.FloatValue;
            }
        }
    }

    public static class GlobalDeclarations
    {
        public static NetworkMessage SelfRequestMessage = new NetworkMessage(0);
        public static NetworkMessage SelfAnswerMessage = new NetworkMessage(1);

        public static int RequestContent = 999;
        public static int AnswerContent = 111;

        public static int IntValue = -1;
        public static byte ByteValue = 255;
        public static float FloatValue = -1.0f;

        public static bool OpponentRequestMessageRecieved = false;
        public static bool OpponentAnswerMessageRecieved = false;

        public static bool SelfConnected = false;
        public static bool OpponentConnected = false;
    }

    public static class SubscriptionMessages
    {
        public static void ReadOpponentRequestMessage(NetworkMessage inputMessage)
        {
            //UnityEngine.Debug.Log("Potential request message recieved");
            int value = inputMessage.ReadInt();
            if (value == 999)
            {
                GlobalDeclarations.OpponentRequestMessageRecieved = true;
            }
            _lastMessageRecieved = DateTime.Now.Ticks;
        }
        public static void ReadOpponentAnswerMessage(NetworkMessage inputMessage)
        {
            //UnityEngine.Debug.Log("Potential answer message recieved");
            int value = inputMessage.ReadInt();
            if (value == 111)
            {
                //UnityEngine.Debug.Log("answer message valid");
                GlobalDeclarations.OpponentAnswerMessageRecieved = true;
            }
            else
            {
                UnityEngine.Debug.Log("answer message NOT valid");
            }
            _lastMessageRecieved = DateTime.Now.Ticks;
        }

        public static void ReadMessageByte(NetworkMessage message)
        {
            UnityEngine.Debug.Log("ReadMessage");
            byte byteValue = message.ReadByte();
            UnityEngine.Debug.Log("Value:" + byteValue);
            GlobalDeclarations.ByteValue = byteValue;

            _lastMessageRecieved = DateTime.Now.Ticks;
        }

        public static void ReadMessageInt(NetworkMessage message)
        {
            UnityEngine.Debug.Log("ReadMessage");
            int intValue = message.ReadInt();
            UnityEngine.Debug.Log("Value:" + intValue);
            GlobalDeclarations.IntValue = intValue;

            _lastMessageRecieved = DateTime.Now.Ticks;
        }

        public static void ReadMessageFloat(NetworkMessage message)
        {
            //UnityEngine.Debug.Log("ReadMessage");
            float floatValue = message.ReadFloat();
            //UnityEngine.Debug.Log("Value:" + floatValue);
            GlobalDeclarations.FloatValue = floatValue;

            _lastMessageRecieved = DateTime.Now.Ticks;
        }
    }
}
