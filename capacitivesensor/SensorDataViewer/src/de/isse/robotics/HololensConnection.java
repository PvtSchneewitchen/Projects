package de.isse.robotics;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.SocketAddress;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.util.Arrays;

public class HololensConnection {
	private DatagramSocket _datagramSocket;
	private MulticastSocket _multicastSocket;
	private InetAddress _group;
	private int _port;

	public void Multicast_Init(String ipAdress, int port) throws IOException {
		
		_port = port;
		_group = InetAddress.getByName(ipAdress);
		
		_multicastSocket = new MulticastSocket(_port);
		_multicastSocket.setInterface(InetAddress.getLocalHost());
		_multicastSocket.joinGroup(_group);
	}

	public void Multicast_Send(String message) throws IOException, InterruptedException {
		
		byte[] bt = message.getBytes();	
		_multicastSocket.send(new DatagramPacket(bt, bt.length, _group, _multicastSocket.getLocalPort()));
		
		System.out.println("sent: " + new String(bt));
		Thread.sleep(1 * 100);
	}

	public void Multicast_Listen() throws IOException {
		
		DatagramPacket packet = new DatagramPacket(new byte[256], 256);
		_multicastSocket.receive(packet);
		
		System.out.println("Got message: " + new String(packet.getData()));
	}
	
	public void DatagramSocket_Init(String ipAdress, int port) throws  SocketException, UnknownHostException
	{
		_port = port;
		_group = InetAddress.getByName(ipAdress);
		
		_datagramSocket = new DatagramSocket(_port);
		
	}
	
	public void DatagramSocket_Send(String message) throws IOException, InterruptedException
	{
		byte[] buf = message.getBytes();
		
		DatagramPacket packet = new DatagramPacket(buf, buf.length, _group, _port);
		_datagramSocket.send(packet);
		
		System.out.println("sent: " + new String(buf));
		Thread.sleep(1 * 100);
	}
	
	public void DatagramSocket_Listen() throws IOException
	{
		byte[] buf = new byte[256];
		DatagramPacket packet = new DatagramPacket(buf, buf.length);
		_datagramSocket.receive(packet);
		System.out.println("Got message: " + new String(packet.getData()));
	}
}
