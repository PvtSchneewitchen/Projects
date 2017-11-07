package de.isse.robotics;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import jssc.SerialPort;
import jssc.SerialPortException;

public class SensorModel {

	interface Listener {
		void valuesReceived(double[] values);
	}

	private List<Listener> listeners = new ArrayList<>();

	public SensorModel() throws SerialPortException, InterruptedException {
		String[] ports = jssc.SerialPortList.getPortNames();
		for (String portname : ports) {
			SerialPort port = new SerialPort(portname);
			System.out.println("Trying " + portname);
			port.openPort();
			Thread.sleep(200);
			String line = port.readString();
			if (line != null) {
				System.out.println("Choosing " + portname);
				new Thread(new Runnable() {
					@Override
					public void run() {
						try {
							while (true) {
								int read = System.in.read();
								port.writeByte((byte) read);
							}
						} catch (IOException e) {
							e.printStackTrace();
						} catch (SerialPortException e) {
							e.printStackTrace();
						}
					}
				}).start();

				new Thread(new Runnable() {
					public void run() {
						StringBuffer sb = new StringBuffer();
						while (true) {
							try {
								String line = null;
								byte[] bytes = port.readBytes();
								if (bytes == null)
									continue;
								for (byte b : bytes) {
									if (b == '\r') {
										line = sb.toString();
										parseLine(line);
										sb = new StringBuffer();
									} else if (b == '\n') {
									} else {
										sb.append((char) b);
									}
								}
							} catch (SerialPortException e) {
								e.printStackTrace();
							}
						}
					};
				}).start();
				return;
			}
		}
		System.out.println("Giving up.");
		throw new SerialPortException("No Sensor found", "SensorModel", "NotFoundException");
	}

	protected void parseLine(String line) {
		if (line == null || line.trim().isEmpty())
			return;
		String[] parts = line.substring(1).split("\t");
		try {
			double[] values = Arrays.asList(parts).stream().mapToInt(Integer::parseInt)
					.mapToDouble(SensorModel::calculateCapacitance).toArray();
			for (Listener listener : listeners)
				listener.valuesReceived(values);
		} catch (NumberFormatException ex) {
			System.err.println(line);
		}
	}

	public void addListener(Listener listener) {
		listeners.add(listener);
	}

	public static double calculateCapacitance(int data) {
		int FIN_SEL = 2;
		double L = 18e-6; // 18uH
		double C = 33e-12; // 33pF
		double fREF = 43.4e6; // 43.4MHz
		double fSENSOR = FIN_SEL * fREF * data / (1 << 28);
		double cSENSOR = 1 / (L * (2 * Math.PI * fSENSOR) * (2 * Math.PI * fSENSOR)) - C;
		return cSENSOR * 1e12; // pF
	}
}
