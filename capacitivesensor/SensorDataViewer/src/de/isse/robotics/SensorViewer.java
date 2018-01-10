package de.isse.robotics;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.util.Arrays;

//import jssc.SerialPortException;

public class SensorViewer {

	final static String address = "237.0.0.1";
	final static int port = 9000;

	static double capacity1;
	static double capacity2;

	public static void main(String[] args) throws Exception {
		// SensorModel model = new SensorModel();
		// SensorViewerWindow svwInstance = new SensorViewerWindow(model);
		// svwInstance.setVisible(true);

		// ViconAccess viconClass = new ViconAccess();
		// viconClass.startVicon();
		// viconClass.startLogger();

		capacity1 = 32.55f;
		capacity2 = 16.75f;

		final InetAddress group = InetAddress.getByName(address);

		// sending thread
		new Thread(new Runnable() {
			@Override
			public void run() {
				try {
					@SuppressWarnings("resource")
					MulticastSocket socket = new MulticastSocket(port);
					socket.setInterface(InetAddress.getLocalHost());
					socket.joinGroup(group);

					while (true) {
						// either
						SimulateCapacities();
						//or
						// capacity1 = svwInstance.getMean()[1];
						// capacity2 = svwInstance.getMean()[2];

						String message = String.valueOf(capacity1) + " " + String.valueOf(capacity2);
						byte[] bt = message.getBytes();

						socket.send(new DatagramPacket(bt, bt.length, group, port));
						System.out.println("sent: " + new String(bt));
						Thread.sleep(1 * 1000);
					}
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		}).start();

		// listening thread
		new Thread(new Runnable() {
			@Override
			public void run() {
				try {
					@SuppressWarnings("resource")
					MulticastSocket socket = new MulticastSocket(port);
					socket.setInterface(InetAddress.getLocalHost());
					socket.joinGroup(group);

					DatagramPacket packet = new DatagramPacket(new byte[256], 256);
					while (true) {
						socket.receive(packet);
						System.out.println("Got message: " + new String(packet.getData()));
					}
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		}).start();
	}

	public static void SimulateCapacities() {
		capacity1 += 0.0019f;
		capacity2 += 0.0008f;

		if (capacity1 >= 32.63f)
			capacity1 = 32.55f;
		if (capacity2 >= 16.94f)
			capacity2 = 16.75f;
	}
}
