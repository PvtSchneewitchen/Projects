package de.isse.robotics;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.sql.Time;
import java.util.Arrays;

//import jssc.SerialPortException;

public class SensorViewer {
	private static int port = 6;
	private static boolean sendDataToHololens = false;
	private static DatagramPacket requestMessage;

	public static void main(String[] args) throws Exception {
//		HololensConnection hlc = new HololensConnection();
//		// SensorModel model = new SensorModel();
//		// SensorViewerWindow svwInstance = new SensorViewerWindow(model);
//		// svwInstance.setVisible(true);
//
//		// ViconAccess viconClass = new ViconAccess();
//		// viconClass.startVicon();
//		// viconClass.startLogger();S
//
//		double capacity1;
//		double capacity2;
//
//		while (true) {
//			// capacity1 = svwInstance.getMean()[1];
//			// capacity2 = svwInstance.getMean()[2];
//			capacity1 = 1.234;
//			capacity2 = 5.678;
//
//			String message = String.valueOf(capacity1) + " " + String.valueOf(capacity2);
//
//			 //hlc.SendOverMulticast(message);
//			 //hlc.listen();
//			//hlc.multicast(message);
//			//(hlc.sendTest(message);
//			hlc.listenTest();
//		}
		
		
		
	    final InetAddress group = InetAddress.getByName("237.0.0.1");
	    final int port = 9000;

	    new Thread(new Runnable() {
	        @Override
	        public void run() {
	            try {
	                MulticastSocket socket = new MulticastSocket(port);
	                socket.setInterface(InetAddress.getLocalHost());
	                socket.joinGroup(group);

	                DatagramPacket packet = new DatagramPacket(new byte[100], 100);
	                while(true) {
	                    socket.receive(packet);
	                    System.out.println("Got packet " + 
	                            Arrays.toString(packet.getData()));
	                }
	            } catch (Exception e) {
	                e.printStackTrace();
	            }
	        }
	    }).start();

	    new Thread(new Runnable() {
	        @Override
	        public void run() {
	            try {
	                MulticastSocket socket = new MulticastSocket(port);
	                socket.setInterface(InetAddress.getLocalHost());
	                socket.joinGroup(group);

	                byte[] bt = new byte[100];
	                byte index = 0;
	                while(true) {
	                    Arrays.fill(bt, (byte) index++);
	                    socket.send(new DatagramPacket(bt, 100, group, port));
	                    System.out.println("sent 100 bytes");
	                    Thread.sleep(1*1000);
	                }
	            } catch (Exception e) {
	                e.printStackTrace();
	            }
	        }
	    }).start();

		// if(sendDataToHololens)
		// hlc.Connect(port);
		//
		// long startTime = System.currentTimeMillis();
		// double d1 = 0.6;
		// double d2 = 0.8;
		// double d3 = 1.0;
		// double incrementFactor = 0.01;
		// int counter = 0;
		//
		// while(true) {
		// if(sendDataToHololens)
		// requestMessage = hlc.WaitForRequestMessage();
		//
		// double distance = viconClass.processCapacityWithVicon(svwInstance.getMean());
		// viconClass.computeCapacitiesMean(svwInstance.getMean());//TODO call this
		// method in new thread outside while loop
		//
		// d1 -= incrementFactor;
		// if(d1 <= 0){
		// d1 = 0.6;
		// counter++;
		//
		// }
		// d2 -= incrementFactor;
		// if(d2 <= 0)
		// d2 = 0.8;
		// d3 -= incrementFactor;
		// if(d3 <= 0){
		// d3 = 1.0;
		// }
		//
		// if(sendDataToHololens)
		// hlc.SendDataToHololens(d1, d2, d3, requestMessage);
		// //hlc.SendDataToHololens(distance, svwInstance.getMean()[0],
		// svwInstance.getMean()[1], requestMessage);
		//
		// }
	}
}
