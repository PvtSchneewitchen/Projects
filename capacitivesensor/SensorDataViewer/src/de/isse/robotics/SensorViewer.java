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
	static String message;

	public static void main(String[] args) throws Exception {
		HololensConnection hlc = new HololensConnection();
//		 SensorModel model = new SensorModel();
//		 SensorViewerWindow svwInstance = new SensorViewerWindow(model);
		// svwInstance.setVisible(true);

		// ViconAccess viconClass = new ViconAccess();
		// viconClass.startVicon();
		// viconClass.startLogger();

		capacity1 = 32.55f;
		capacity2 = 16.75f;

		hlc.Multicast_Init(address, port);
		
		while(true)
		{
			SimulateCapacities();
//			capacity1 = svwInstance.getMean()[0];
//			capacity2 = svwInstance.getMean()[1];
			
			message = String.valueOf(capacity1) + " " + String.valueOf(capacity2);
			
			hlc.Multicast_Send(message);
			//hlc.Multicast_Listen();
			
		}
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
