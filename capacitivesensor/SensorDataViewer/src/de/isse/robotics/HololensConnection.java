package de.isse.robotics;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.util.Arrays;

public class HololensConnection {

	final String defaultAdress = "237.0.0.1";
	//
	final int defaultPort = 9000;

	

	public void multicast(String message) throws IOException {
		try {
			final InetAddress group = InetAddress.getByName(defaultAdress);
			MulticastSocket socket = new MulticastSocket(defaultPort);
			socket.setInterface(InetAddress.getLocalHost());
			socket.joinGroup(group);

			byte[] buf = message.getBytes();

			socket.send(new DatagramPacket(buf, buf.length, group, defaultPort));
			System.out.println("Following message sent: " + new String(buf));
			socket.close();

		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public void listen() throws IOException {

		try {
			final InetAddress group = InetAddress.getByName(defaultAdress);
			MulticastSocket socket = new MulticastSocket(defaultPort);
			socket.setInterface(InetAddress.getLocalHost());
			socket.joinGroup(group);

			byte[] buf = new byte[256];
			DatagramPacket packet = new DatagramPacket(buf, buf.length);

			socket.receive(packet);
			System.out.println("Got message: " + new String(packet.getData()));
			socket.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
}
