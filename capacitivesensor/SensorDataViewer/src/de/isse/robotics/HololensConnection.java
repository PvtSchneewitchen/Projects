package de.isse.robotics;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;

public class HololensConnection {

	private String standardAddress = "239.255.255.250";
	private final int multicastPort = 1900;

	public void listen() throws IOException {
		InetAddress multicastAddress = InetAddress.getByName(standardAddress);
		MulticastSocket socket = new MulticastSocket(multicastPort);
		socket.setReuseAddress(true);
		socket.setSoTimeout(1500);
		socket.joinGroup(multicastAddress);

		byte[] rxbuf = new byte[8192];
		DatagramPacket packet = new DatagramPacket(rxbuf, rxbuf.length);
		socket.receive(packet);

		System.out.println("Message Content: " + new String(packet.getData()));
	}

	public void multicast(String message) throws IOException {
		try {
			InetAddress multicastAddress = InetAddress.getByName(standardAddress);
			MulticastSocket socket = new MulticastSocket(multicastPort);
			socket.setReuseAddress(true);
			socket.setSoTimeout(1500);
			socket.joinGroup(multicastAddress);
			// send discover
			byte[] txbuf = message.getBytes();
			DatagramPacket hi = new DatagramPacket(txbuf, txbuf.length, multicastAddress, multicastPort);
			socket.send(hi);
			System.out.println("SSDP discover sent");
			do {
				byte[] rxbuf = new byte[8192];
				DatagramPacket packet = new DatagramPacket(rxbuf, rxbuf.length);
				socket.receive(packet);

				System.out.println("Message Content: " + new String(packet.getData()));
				if (new String(packet.getData()).equals(message)) {
					System.out.println("Right message content");
				}
				// dumpPacket(packet);
			} while (true); // should leave loop by SocketTimeoutException
		} catch (SocketTimeoutException e) {
			System.out.println("Timeout");
		}
	}

	private void dumpPacket(DatagramPacket packet) throws IOException {
		InetAddress addr = packet.getAddress();
		System.out.println("Response from: " + addr);
		ByteArrayInputStream in = new ByteArrayInputStream(packet.getData(), 0, packet.getLength());
		copyStream(in, System.out);
	}

	private void copyStream(InputStream in, OutputStream out) throws IOException {
		BufferedInputStream bin = new BufferedInputStream(in);
		BufferedOutputStream bout = new BufferedOutputStream(out);
		int c = bin.read();
		while (c != -1) {
			out.write((char) c);
			c = bin.read();
		}
		bout.flush();
	}

	private final static String DISCOVER_MESSAGE_ROOTDEVICE = "M-SEARCH * HTTP/1.1\r\n" + "ST: upnp:rootdevice\r\n"
			+ "MX: 3\r\n" + "MAN: `ssdp:discover`\r\n".replace('`', '"') + "HOST: 239.255.255.250:1900\r\n\r\n";

	// static DatagramSocket serverSocket;
	// static DatagramPacket message;
	// DatagramPacket rMessage = new DatagramPacket(new byte[32], 32);
	//
	// public void Connect(int port){
	// try
	// {
	// serverSocket = new DatagramSocket(port);
	// }
	// catch( Exception ex )
	// {
	// System.out.println("Problem creating socket on port: " + port);
	// }
	// }
	//
	// public DatagramPacket WaitForRequestMessage() throws IOException{
	// System.out.println("Sensorviewer is wating for request message");
	// serverSocket.receive(rMessage);
	// String message = new String(rMessage.getData(), 0, rMessage.getLength());
	//
	// System.out.println("Request received from: " + rMessage.getAddress () + " : "
	// + rMessage.getPort ());
	// System.out.println("Request Message: " + new String(message));
	//
	// return rMessage;
	// }
	//
	// @SuppressWarnings("static-access")
	// public void SendDataToHololens(double distance, double capacity1, double
	// capcity2, DatagramPacket requestMessage) throws IOException{
	// byte[] dataBytes = new byte[24];
	// ByteBuffer bf = ByteBuffer.allocate(dataBytes.length);
	// bf.order(ByteOrder.LITTLE_ENDIAN).wrap(dataBytes).putDouble(0, distance);
	// bf.order(ByteOrder.LITTLE_ENDIAN).wrap(dataBytes).putDouble(8, capacity1);
	// bf.order(ByteOrder.LITTLE_ENDIAN).wrap(dataBytes).putDouble(16, capcity2);
	//
	// DatagramPacket dataMessage = new DatagramPacket(dataBytes, dataBytes.length,
	// requestMessage.getAddress() ,requestMessage.getPort());
	//
	// serverSocket.send(dataMessage);
	// System.out.println("Data message sent");
	// }

}
