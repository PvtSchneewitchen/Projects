package de.isse.robotics;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.UnknownHostException;
import java.util.Arrays;

public class HololensConnection {
	private MulticastSocket socket;
	private InetAddress group;

	public void InitMulticast(String ipAdress, int port) throws IOException {
		group = InetAddress.getByName(ipAdress);
		socket = new MulticastSocket(port);
		socket.setInterface(InetAddress.getLocalHost());
		socket.joinGroup(group);
	}

	public void multicast(String message) throws IOException, InterruptedException {
		byte[] bt = message.getBytes();	
		socket.send(new DatagramPacket(bt, bt.length, group, socket.getLocalPort()));
		System.out.println("sent: " + new String(bt));
		Thread.sleep(1 * 100);
	}

	public void listen() throws IOException {
		DatagramPacket packet = new DatagramPacket(new byte[256], 256);
		socket.receive(packet);
		System.out.println("Got message: " + new String(packet.getData()));
	}
}
