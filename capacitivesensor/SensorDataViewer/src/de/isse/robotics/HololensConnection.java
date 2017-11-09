package de.isse.robotics;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;

public class HololensConnection{
	private int messageIdDouble = 1;
	
	public boolean Connect(DatagramSocket receiveSocket) throws IOException{
		byte[] receiveData = new byte[32];
		DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
		receiveSocket.receive(receivePacket);
		String receivedData = new String(receivePacket.getData());
		System.out.println("ANSWERMESSAGE: " + receivedData);
		return true;
	}
	
	public void sendDataToHololens(DatagramSocket sendSocket, double distance, double[] capacities) throws IOException {
		Object[] messageData = new Object[capacities.length+1];
		messageData[0] = messageIdDouble;
		for(int i=1;i<capacities.length;i++) {
			messageData[i] = capacities[i+1];
		}
		DatagramPacket message = new DatagramPacket(serializeObject(messageData), serializeObject(messageData).length);
		sendSocket.send(message);
	}
	
	public static byte[] serializeObject(Object obj) throws IOException
	{
	    ByteArrayOutputStream bytesOut = new ByteArrayOutputStream();
	    ObjectOutputStream oos = new ObjectOutputStream(bytesOut);
	    oos.writeObject(obj);
	    oos.flush();
	    byte[] bytes = bytesOut.toByteArray();
	    bytesOut.close();
	    oos.close();
	    return bytes;
	}
}
