package de.isse.robotics;

import java.io.IOException;
import java.util.logging.FileHandler;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

import org.roboticsapi.core.world.Transformation;
import org.roboticsapi.device.vicon.tracker.javarcc.devices.JMulticastVicon;
import org.roboticsapi.device.vicon.tracker.javarcc.devices.JVicon;
import org.roboticsapi.device.vicon.tracker.javarcc.devices.JVicon.Subject;
import org.roboticsapi.facet.javarcc.devices.DeviceRegistry;
import org.roboticsapi.facet.javarcc.devices.DeviceRegistry.DeviceRegistryListener;
import org.roboticsapi.facet.javarcc.primitives.world.RPICalc;

public class ViconAccess {
	private static JVicon vicon = new JMulticastVicon(new DeviceRegistry(new DeviceRegistryListener() {
		@Override
		public void deviceRemoved(String arg0) {
		}

		@Override
		public void deviceChanged(String arg0) {
		}

		@Override
		public void deviceAdded(String arg0) {
		}
	}));
	private static Logger logger = Logger.getLogger("MyLog");  
	private static FileHandler fileHandler;
	private static long startTime = System.currentTimeMillis();

	public void startVicon() throws Exception{
		System.out.println("Starting Vicon");
		vicon.start();
		Thread.sleep(2000);
		System.out.println("The following subjects exist: ");
		for (String name : vicon.getSubjectNames())
			System.out.println(name);
	}

	public void startLogger() {
		try {  
			System.out.println("Starting Logger");
			fileHandler = new FileHandler("C:\\Users\\gruenepa\\Documents\\GithubRepos\\Projects\\capacitivesensor\\SensorDataViewer\\log.txt"); 
			logger.addHandler(fileHandler);
			SimpleFormatter formatter = new SimpleFormatter();  
			fileHandler.setFormatter(formatter);
			logger.info("Initial TestLog");  
		} catch (SecurityException e) {  
			e.printStackTrace();  
		} catch (IOException e) {  
			e.printStackTrace();  
		} 
	}

	public void processCapacityWithVicon(double[] capacities) throws Exception{
		float updateLogInSeconds = 1.0f;
		try {
			Subject kugel = vicon.getSubject("Kugel");
			Subject sensorBoard = vicon.getSubject("Sensorboard");
			Transformation transformationKugel = RPICalc.rpiToFrame(kugel.pos);
			Transformation transformationsensorBoard = RPICalc.rpiToFrame(sensorBoard.pos);
			double distance = Math.sqrt(Math.pow((transformationKugel.getX()-transformationsensorBoard.getX()), 2)+
					Math.pow((transformationKugel.getY()-transformationsensorBoard.getY()), 2)+
					Math.pow((transformationKugel.getZ()-transformationsensorBoard.getZ()), 2));

			long actualTime = System.currentTimeMillis();
			if(actualTime - startTime > updateLogInSeconds*1000) {
				startTime = actualTime;
				Object[] obj = 	writeTransformations(new Transformation(1, 2, 3, Math.toRadians(4), Math.toRadians(5), Math.toRadians(6)), 
						new Transformation(11, 22, 33, Math.toRadians(44), Math.toRadians(55), Math.toRadians(66)), 
						capacities, distance);

				logger.log(Level.SEVERE,"\nTransformation 1:  X:{0}	Y:{1}	Z:{2}	||	A:{3}	B:{4}	C:{5}\n"
						+ "Transformation 2:  X:{6}	Y:{7}	Z:{8}	||	A:{9}	B:{10}	C:{11}\n"
						+ "Distance between Trans1 and Trans2: {12}\n"
						+ "Capacity Sensor 1: {13}	||	Capacity Sensor 2: {14}\n"
						+ "-------------------------------------------------------------------------", obj);
			}
		}catch(NullPointerException e) {
			long actualTime = System.currentTimeMillis();
			if(actualTime - startTime > updateLogInSeconds*1000) {
				logger.info("At least one demanded subject not available");
			}
		}
	}

	private Object[] writeTransformations(Transformation t1, Transformation t2, double[] capacities, double distance) {
		int capacityInsert = 13;
		Object[] obj = new Object[capacityInsert + capacities.length];
		obj[0] = t1.getX();
		obj[1] = t1.getY();
		obj[2] = t1.getZ();
		obj[3] = Math.toDegrees(t1.getA());
		obj[4] = Math.toDegrees(t1.getB());
		obj[5] = Math.toDegrees(t1.getC());
		obj[6] = t2.getX();
		obj[7] = t2.getY();
		obj[8] = t2.getZ();
		obj[9] = Math.toDegrees(t2.getA());
		obj[10] = Math.toDegrees(t2.getB());
		obj[11] = Math.toDegrees(t2.getC());
		obj[12] = distance;
		for(int i = capacityInsert; capacities.length+capacityInsert>i; i++) {
			obj[i] = capacities[i-capacityInsert];
		}
		return obj;
	}
}

