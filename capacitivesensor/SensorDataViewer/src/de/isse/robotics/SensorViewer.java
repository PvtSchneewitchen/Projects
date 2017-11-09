package de.isse.robotics;

//import jssc.SerialPortException;

public class SensorViewer {

	public static void main(String[] args) throws Exception {

		SensorModel model = new SensorModel();
		//new SensorViewerWindow(model).setVisible(true);
		SensorViewerWindow svwInstance = new SensorViewerWindow(model);
		svwInstance.setVisible(true);

		ViconAccess viconClass = new ViconAccess();
		viconClass.startVicon();
		viconClass.startLogger();
		while(true) {
			viconClass.processCapacityWithVicon(svwInstance.getMean());
			viconClass.computeCapacitiesMean(svwInstance.getMean());
		}
	}

}
