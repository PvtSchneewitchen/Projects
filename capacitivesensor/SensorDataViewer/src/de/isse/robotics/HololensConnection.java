package de.isse.robotics;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;

public class HololensConnection{
	
	private DatagramSocket socket;
	private InetAddress multicastAddress;
    private byte[] buf;
    
    
    private String standardAddress = "230.0.0.0";
    private int multicastPort = 4446;
	
	public void SendOverMulticast(String message) throws IOException
	{
		socket = new DatagramSocket();
		multicastAddress = InetAddress.getByName(standardAddress);
		buf = message.getBytes();
		
		DatagramPacket packet = new DatagramPacket(buf, buf.length, multicastAddress, multicastPort);
		
		socket.send(packet);
		socket.close();
	}
    
//	static DatagramSocket serverSocket;
//	static DatagramPacket message;
//	DatagramPacket rMessage = new DatagramPacket(new byte[32], 32);
//	
//	public void Connect(int port){
//		try
//        {
//			serverSocket = new DatagramSocket(port);
//        }
//        catch( Exception ex )
//        {
//            System.out.println("Problem creating socket on port: " + port);
//        }
//	}
//	
//	public DatagramPacket WaitForRequestMessage() throws IOException{
//		System.out.println("Sensorviewer is wating for request message");
//		serverSocket.receive(rMessage);
//		String message = new String(rMessage.getData(), 0, rMessage.getLength());
//		
//		System.out.println("Request received from: " + rMessage.getAddress () + " : " + rMessage.getPort ());
//		System.out.println("Request Message: " + new String(message));
//		
//		return rMessage;
//	}
//	
//	@SuppressWarnings("static-access")
//	public void SendDataToHololens(double distance, double capacity1, double capcity2, DatagramPacket requestMessage) throws IOException{
//		byte[] dataBytes = new byte[24];
//		ByteBuffer bf = ByteBuffer.allocate(dataBytes.length);
//		bf.order(ByteOrder.LITTLE_ENDIAN).wrap(dataBytes).putDouble(0, distance);
//		bf.order(ByteOrder.LITTLE_ENDIAN).wrap(dataBytes).putDouble(8, capacity1);
//		bf.order(ByteOrder.LITTLE_ENDIAN).wrap(dataBytes).putDouble(16, capcity2);
//		
//		DatagramPacket dataMessage = new DatagramPacket(dataBytes, dataBytes.length, requestMessage.getAddress() ,requestMessage.getPort());
//		
//		serverSocket.send(dataMessage);
//		System.out.println("Data message sent");
//	}
	
}
