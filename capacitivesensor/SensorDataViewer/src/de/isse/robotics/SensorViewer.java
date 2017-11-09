package de.isse.robotics;

import java.net.DatagramSocket;
import java.net.InetAddress;

//import jssc.SerialPortException;

public class SensorViewer {
	private static String Host = "192.168.178.29";
	private static int Port = 6;
	

	public static void main(String[] args) throws Exception {
		HololensConnection hl = new HololensConnection();
		InetAddress host = InetAddress.getByName(Host);
		DatagramSocket serverSocket = new DatagramSocket(Port,host);
		
		if(hl.Connect(serverSocket)) {
		SensorModel model = new SensorModel();
		SensorViewerWindow svwInstance = new SensorViewerWindow(model);
		svwInstance.setVisible(true);

		ViconAccess viconClass = new ViconAccess();
		viconClass.startVicon();
		viconClass.startLogger();
		
		while(true) {
			viconClass.processCapacityWithVicon(svwInstance.getMean());
			viconClass.computeCapacitiesMean(svwInstance.getMean());
			hl.sendDataToHololens(serverSocket, viconClass.getDistance(), svwInstance.getMean());
		}
	}
	}
}
