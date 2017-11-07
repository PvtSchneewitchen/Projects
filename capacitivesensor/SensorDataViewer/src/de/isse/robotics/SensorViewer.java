package de.isse.robotics;

import jssc.SerialPortException;

public class SensorViewer {

	public static void main(String[] args) throws SerialPortException, InterruptedException {
		
		SensorModel model = new SensorModel();
		new SensorViewerWindow(model).setVisible(true);
		
	}

}
