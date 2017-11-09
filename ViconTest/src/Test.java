import java.util.ArrayList;
import java.util.List;

import org.roboticsapi.core.world.Transformation;
import org.roboticsapi.device.vicon.tracker.javarcc.devices.JMulticastVicon;
import org.roboticsapi.device.vicon.tracker.javarcc.devices.JVicon;
import org.roboticsapi.device.vicon.tracker.javarcc.devices.JVicon.Subject;
import org.roboticsapi.facet.javarcc.devices.DeviceRegistry;
import org.roboticsapi.facet.javarcc.devices.DeviceRegistry.DeviceRegistryListener;
import org.roboticsapi.facet.javarcc.primitives.world.RPICalc;

public class Test {
	public static void main(String[] args) throws Exception {
		JVicon vicon = new JMulticastVicon(new DeviceRegistry(new DeviceRegistryListener() {
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
		vicon.start();

		Thread.sleep(2000);

		System.out.println("The following subjects exist: ");
		for (String name : vicon.getSubjectNames())
			System.out.println(name);
		
		List l = new ArrayList(100);
		l.add(1);
		l.add(2);
		l.add(3);
		l.remove(0);
		System.out.println(l);
		System.out.println(l.size());
		

		Subject kugel = vicon.getSubject("Kugel");
		Subject sensorBoard = vicon.getSubject("Sensorboard");
		Transformation transformationKugel = RPICalc.rpiToFrame(kugel.pos);
		Transformation transformationsensorBoard = RPICalc.rpiToFrame(sensorBoard.pos);
		System.out.println(transformationKugel);
		System.out.println(transformationsensorBoard);
		
		vicon.destroy();
	}
}
