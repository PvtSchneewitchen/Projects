import java.io.Console;
import java.io.IOException;
import java.io.OutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.DatagramSocketImpl;
import java.net.InetAddress;
import java.net.Socket;
import java.sql.DatabaseMetaData;
import java.util.HashMap;
import java.util.Map;
import java.util.function.Consumer;

import javax.swing.DebugGraphics;

public class SocketClientManager {
	//Variables
	private static InetAddress host;
	private static int port;
	private static Map<Byte, Consumer<NetworkMessage>> callbackMethods = new HashMap<Byte, Consumer<NetworkMessage>>();
	private static Thread socketThread;
	private static DatagramSocket udpClient;
	private static boolean abortThread = false;
	private static boolean verboseMode = false;
	
	//getter setter
	public static void setHost(InetAddress ip){
		host=ip;
	}
	public static void setPort(int portId){
		port=portId;
	}
	public static void setVerboseMode(boolean bool){
		verboseMode=bool;
	}
	public static boolean getVerboseMode(){
		return verboseMode;
	}
	
	//Methods
	public static void Connect()
    { 
//        udpClient = new DatagramSocket(port, host);
//        socketThread = new Thread(SocketClientManager.Listen);
//        socketThread.Start();
		//TODO
    }
	
	public static void Subscribe(byte messageId, Consumer<NetworkMessage> callbackMethod)
    {
        // Check if not already subscribed
        if (callbackMethods.containsKey(messageId))
            throw new IllegalStateException("There is already a subscription to this message ID");

        callbackMethods.put(messageId, callbackMethod);
    }
	
	public static void SendMessage(NetworkMessage nm) throws IOException
    {
        // Write the data into a byte array
        byte[] byteArray = new byte[nm.getContent().length + 1];

        byteArray[0] = nm.getMessageId();

        // Write the content into the array
        byte[] messageContent = nm.getContent();
        
        for(int i=1; i<nm.getContent().length; ++i) {
        	byteArray[i] = messageContent[i-1];
        }

        System.out.println("Sending message");
        // Send it
        DatagramPacket packet = new DatagramPacket(byteArray, byteArray.length);
        udpClient.send(packet);
    }
	
	private static void Listen()
    {
        try
        {
        	byte[] messageBuf = new byte[1000000];
        	DatagramPacket receivedPacket = new DatagramPacket(messageBuf, messageBuf.length);

            // Keep looping as long as the thread isn't aborted
            while (!abortThread)
            {
                udpClient.receive(receivedPacket);
                messageBuf = receivedPacket.getData();
                // Data received, create a new NetworkMessage
                byte messageId = messageBuf[0];

                // Remove the message ID from the data
                byte[] message = new byte[messageBuf.length - 1];

                for (int i = 1; i < messageBuf.length; ++i)
                {
                    message[i - 1] = messageBuf[i];
                }

                // Call the correct callback method
                if (callbackMethods.containsKey(messageId))
                {
                    // Catch any exceptions and rethrow them,
                    // so a user gets a good exception instead of a 
                    // object disposed exception
                    try
                    {
                    	Consumer<NetworkMessage> callbackMethod = new Consumer<NetworkMessage>() {

							@Override
							public void accept(NetworkMessage arg0) {
								// TODO Auto-generated method stub
								
							}
						};
                    	
                    	callbackMethods.put(messageId, new NetworkMessage(messageId, message));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    PrintDebug("No known callback for message ID " + messageId.ToString());
                }
            }
        }
        catch (ThreadAbortException e)
        {
            // Thread aborted, do nothing
        }
        catch (SocketException e)
        {
            // Socket aborted, probably because we want to close it
            if (e.ErrorCode != 10004)
                throw e;
        }
        catch (Exception e)
        {
            // Rethrow exception
            throw e;

            PrintDebug(e.Message);
        }
        finally
        {
            // Make sure we always close the connection
            udpClient.Close();
        }
    }
	
	public interface Action<T>
	{
	    void call(T target) throws Exception;
	}
}
