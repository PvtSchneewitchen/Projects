package de.isse.robotics;

import java.awt.List;
import java.io.IOException;
import java.lang.reflect.Array;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.Formatter;
import java.util.LinkedList;
import java.util.Queue;
import java.util.Vector;
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
	private static long startTimeCatchNullPointerExc = System.currentTimeMillis();
	private static long startTimeLogDistanceDependency = System.currentTimeMillis();
	private static long startTimeLogCapacityDependency1 = System.currentTimeMillis();
	private static long startTimeLogCapacityDependency2 = System.currentTimeMillis();
	private long startTimeInitialLog = System.currentTimeMillis();
	private int initialReferenceLog = 10;
	private boolean initialReferenceLogEnd = false;
	private double[] initialCapacitiesMean1 = new double[10];
	private double[] initialCapacitiesMean2 = new double[10];
	private static double distance = 0;
	private double[] initialCapacities = new double[2];
	private double[] capacitiesMean = new double[2];
 	private ArrayList<Double> capacities1Mean = new ArrayList<Double>();
 	private ArrayList<Double> capacities2Mean = new ArrayList<Double>();

 	public double getDistance(){
		return distance;
	}
	
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
			
			DecimalFormat df = new DecimalFormat("#0.00000");
			double d = 1.655206963075406;
			Object o = d;
			logger.log(Level.SEVERE, "1.655206963075406 : {0}", df.format(o));
		} catch (SecurityException e) {  
			e.printStackTrace();  
		} catch (IOException e) {  
			e.printStackTrace();  
		} 
	}

	public void processCapacityWithVicon(double[] capacities) throws Exception{
		try {
			Subject kugel = vicon.getSubject("Kugel");
			Subject sensorBoard = vicon.getSubject("Sensorboard");
			Transformation transformationKugel = RPICalc.rpiToFrame(kugel.pos);
			Transformation transformationSensorBoard = RPICalc.rpiToFrame(sensorBoard.pos);
			distance = Math.sqrt(Math.pow((transformationKugel.getX()-transformationSensorBoard.getX()), 2)+
					Math.pow((transformationKugel.getY()-transformationSensorBoard.getY()), 2)+
					Math.pow((transformationKugel.getZ()-transformationSensorBoard.getZ()), 2));

			if(0 < initialReferenceLog && !Double.isInfinite(capacities[0]) && !Double.isInfinite(capacities[1])
					&& !Double.isNaN(capacities[0]) && !Double.isNaN(capacities[0])
					&& capacities[0] != 0 && capacities[1] != 0) {
				
				long actualTime = System.currentTimeMillis();
				if(actualTime - startTimeInitialLog > 100) {
					initialCapacitiesMean1[initialReferenceLog-1] = capacities[0];
					initialCapacitiesMean2[initialReferenceLog-1] = capacities[1];
					initialReferenceLog--;
				}
			}
			else if(initialReferenceLog <=0 && !initialReferenceLogEnd && !Double.isInfinite(capacities[0]) && !Double.isInfinite(capacities[1])
					&& !Double.isNaN(capacities[0]) && !Double.isNaN(capacities[0])
					&& capacities[0] != 0 && capacities[1] != 0) {
				initialReferenceLogEnd = true;
				initialCapacities[0] = doubleArrayAvg(initialCapacitiesMean1);
				initialCapacities[1] = doubleArrayAvg(initialCapacitiesMean2);
				
				Object[] obj = 	writeTransformations(transformationKugel, transformationSensorBoard, 
						initialCapacities, distance);
				logger.log(Level.SEVERE,"\nINITIAL REFERENCE LOG:\n"
						+ "Transformation 1:  X:{0}	Y:{1}	Z:{2}	||	A:{3}	B:{4}	C:{5}\n"
						+ "Transformation 2:  X:{6}	Y:{7}	Z:{8}	||	A:{9}	B:{10}	C:{11}\n"
						+ "Distance between Trans1 and Trans2: {12}\n"
						+ "Capacity Mean 1: {13}	||	Capacity Mean 2: {14}\n"
						+ "Capacity Sensor 1: {15}	||	Capacity Sensor 2: {16}\n"
						+ "-------------------------------------------------------------------------", obj);
			}
			//change logging here
			
			//logDependingOnDistance(transformationKugel, transformationSensorBoard, capacities, distance);
			logDependingOnCapacity(transformationKugel, transformationSensorBoard, capacities, distance);
			//
		}catch(NullPointerException e) {
			long actualTime = System.currentTimeMillis();
			if(actualTime - startTimeCatchNullPointerExc > 1000) {
				logger.info("At least one demanded subject not available");
				startTimeCatchNullPointerExc = actualTime;
			}
		}
	}

	private void logDependingOnDistance(Transformation kugel, Transformation sensorBoard, double[] capacities, double distance) {
		float timeBetweenLogs_s= 4.0f;
		double distanceBorder_m = 0.1;

		if(distanceBorder_m > distance) {
			long actualTime = System.currentTimeMillis();
			if(actualTime - startTimeLogDistanceDependency > timeBetweenLogs_s*1000) {
				startTimeLogDistanceDependency = actualTime;
				Object[] obj = 	writeTransformations(kugel, sensorBoard, 
						capacities, distance);

				logger.log(Level.SEVERE,"\n"
						+ "Transformation 1:  X:{0}		Y:{1}	Z:{2}	||	A:{3}	B:{4}	C:{5}\n"
						+ "Transformation 2:  X:{6}		Y:{7}	Z:{8}	||	A:{9}	B:{10}	C:{11}\n"
						+ "Distance between Trans1 and Trans2: {12}\n"
						+ "Capacity Mean 1: {13}	||	Capacity Mean 2: {14}\n"
						+ "Capacity Sensor 1: {15}	||	Capacity Sensor 2: {16}\n"
						+ "-------------------------------------------------------------------------", obj);
			}
		}
	}


	private void logDependingOnCapacity(Transformation kugel, Transformation sensorBoard, double[] capacities, double distance) {
		float timeBetweenLogs_s= 4.0f;
		double capacityBorder_m = 0.03;

		if(capacities[0] - capacitiesMean[0] > capacityBorder_m) {
			
			long actualTime = System.currentTimeMillis();
			if(actualTime - startTimeLogCapacityDependency1 > timeBetweenLogs_s*1000) {
				startTimeLogCapacityDependency1 = actualTime;
				Object[] obj = 	writeTransformations(kugel, sensorBoard, 
						capacities, distance);

				logger.log(Level.SEVERE,"\nSensor 1 Undercut\n"
						+ "Transformation 1:  X:{0}		Y:{1}	Z:{2}	||	A:{3}	B:{4}	C:{5}\n"
						+ "Transformation 2:  X:{6}		Y:{7}	Z:{8}	||	A:{9}	B:{10}	C:{11}\n"
						+ "Distance between Trans1 and Trans2: {12}\n"
						+ "Capacity Mean 1: {13}	||	Capacity Mean 2: {14}\n"
						+ "Capacity Sensor 1: {15}	||	Capacity Sensor 2: {16}\n"
						+ "-------------------------------------------------------------------------", obj);
			}
		}
		if(capacities[1] - capacitiesMean[1] > capacityBorder_m) {
			long actualTime = System.currentTimeMillis();
			if(actualTime - startTimeLogCapacityDependency2 > timeBetweenLogs_s*1000) {
				startTimeLogCapacityDependency2 = actualTime;
				Object[] obj = 	writeTransformations(kugel, sensorBoard, 
						capacities, distance);

				logger.log(Level.SEVERE,"\nSensor 2 Undercut\n"
						+ "Transformation 1:  X:{0}		Y:{1}	Z:{2}	||	A:{3}	B:{4}	C:{5}\n"
						+ "Transformation 2:  X:{6}		Y:{7}	Z:{8}	||	A:{9}	B:{10}	C:{11}\n"
						+ "Distance between Trans1 and Trans2: {12}\n"
						+ "Capacity Sensor 1: {13}	||	Capacity Sensor 2: {14}\n"
						+ "-------------------------------------------------------------------------", obj);
			}
		}
	}

	void computeCapacitiesMean(double[] capacities){
		double sum1 = 0;
		double sum2 = 0;
		capacities1Mean.add(capacities[0]);
		capacities2Mean.add(capacities[1]);
		if(capacities1Mean.size() >= 100000) {
			capacities1Mean.remove(0);
			capacities2Mean.remove(0);
		}
		for(int i = 0; capacities1Mean.size() > i; i++) {
			sum1 += capacities1Mean.get(i);
			sum2 += capacities2Mean.get(i);
		}
		capacitiesMean[0] = sum1/capacities1Mean.size();
		capacitiesMean[1] = sum2/capacities2Mean.size();
	}

	private Object[] writeTransformations(Transformation t1, Transformation t2, double[] capacities, double distance) {
		int capacityInsert = 15;
		DecimalFormat df = new DecimalFormat("#0.00000");
		Object[] obj = new Object[capacityInsert + capacities.length];
		obj[0] = df.format(t1.getX());
		obj[1] = df.format(t1.getY());
		obj[2] = df.format(t1.getZ());
		obj[3] = df.format(Math.toDegrees(t1.getA()));
		obj[4] = df.format(Math.toDegrees(t1.getB()));
		obj[5] = df.format(Math.toDegrees(t1.getC()));
		obj[6] = df.format(t2.getX());
		obj[7] = df.format(t2.getY());
		obj[8] = df.format(t2.getZ());
		obj[9] = df.format(Math.toDegrees(t2.getA()));
		obj[10] = df.format(Math.toDegrees(t2.getB()));
		obj[11] = df.format(Math.toDegrees(t2.getC()));
		obj[12] = distance;
		obj[13] = df.format(capacitiesMean[0]);
		obj[14] = df.format(capacitiesMean[1]);
		for(int i = capacityInsert; capacities.length+capacityInsert>i; i++) {
			obj[i] = df.format(capacities[i-capacityInsert]);
		}
		return obj;
	}
	
	
	public static double doubleArrayAvg(double[] array){
		double sum = 0;
		for(int i=0; array.length>i; i++) {
			sum += array[i];
		}
	return  sum/array.length;	
	}
	
//	public static Object[] formatObjArray(Object[] obj){
//		DecimalFormat df = new DecimalFormat("#0.00000");
//		for(int i=0; obj.length>i; i++) {
//			obj[i] = df.format(obj[i]);
//		}
//		return obj;
	}


