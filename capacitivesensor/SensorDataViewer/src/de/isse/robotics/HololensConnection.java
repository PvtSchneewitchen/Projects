package de.isse.robotics;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.nio.ByteBuffer;
import java.util.ArrayList;

public class HololensConnection{
	private int messageIdDistance = 1;
	private int messageIdCapacity1 = 2;
	private int messageIdCapacity2 = 3;
	
	public boolean Connect(DatagramSocket receiveSocket) throws IOException{
		byte[] receiveData = new byte[32];
		DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
		receiveSocket.receive(receivePacket);
		String receivedData = new String(receivePacket.getData());
		System.out.println("ANSWERMESSAGE: " + receivedData);
		return true;
	}
	
	public void sendDistanceToHololens(DatagramSocket sendSocket, double distance) throws IOException {
		byte[] distanceMessage = new byte[9];
		byte[] distanceInBytes = new byte[8];
		distanceInBytes = toByteArray(distance).clone();
		distanceMessage[0] = (byte) messageIdDistance;
		for(int i=1;distanceInBytes.length>i;i++) {
			distanceMessage[i] = distanceInBytes[i-1];
		}
		DatagramPacket message = new DatagramPacket(distanceMessage, distanceMessage.length);
		sendSocket.send(message);
	}
	
	public void sendCapacity1ToHololens(DatagramSocket sendSocket, double capacity1) throws IOException {
		byte[] capacity1Message = new byte[9];
		byte[] capacity1InBytes = new byte[8];
		capacity1InBytes = toByteArray(capacity1).clone();
		capacity1Message[0] = (byte) messageIdCapacity1;
		for(int i=1;capacity1InBytes.length>i;i++) {
			capacity1Message[i] = capacity1InBytes[i-1];
		}
		DatagramPacket message = new DatagramPacket(capacity1Message, capacity1Message.length);
		sendSocket.send(message);
	}
	
	public void sendCapacity2ToHololens(DatagramSocket sendSocket, double capacity2) throws IOException {
		byte[] capacity2Message = new byte[9];
		byte[] capacity2InBytes = new byte[8];
		capacity2InBytes = toByteArray(capacity2).clone();
		capacity2Message[0] = (byte) messageIdCapacity2;
		for(int i=1;capacity2InBytes.length>i;i++) {
			capacity2Message[i] = capacity2InBytes[i-1];
		}
		DatagramPacket message = new DatagramPacket(capacity2Message, capacity2Message.length);
		sendSocket.send(message);
	}
	
	public static byte[] toByteArray(double value) {
	    byte[] bytes = new byte[8];
	    ByteBuffer.wrap(bytes).putDouble(value);
	    return bytes;
	}
	
}
