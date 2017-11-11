import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class Main {

	private static String Host = "192.168.178.29";
	private static int Port = 6;
	static DatagramSocket serverSocket;
	static DatagramPacket message;
	

	public static void main(String[] args) throws Exception {
		HololensConnection hl = new HololensConnection();
		InetAddress host = InetAddress.getByName(Host);

			double d = 1.11;
			double c1 = 2.22222222;
			double c2 = 3.33333333;
			int doublesToSend = 3;
			
			try
	        {
				serverSocket = new DatagramSocket(Port);
	        }
	        catch( Exception ex )
	        {
	            System.out.println("Problem creating socket on port: " + Port );
	        }
			
			
//			byte[] b = new byte[1024];
//			DatagramPacket message2 = new DatagramPacket(b, b.length, null, serverSocket.getLocalPort());
//			serverSocket.receive(message2);
//			System.out.println(message2.getData());
			DatagramPacket rmessage = new DatagramPacket(new byte[8], 8);
			System.out.println("waiting for message");
			while(true) {
				serverSocket.receive(rmessage);
				System.out.println("Received from: " + rmessage.getAddress () + " : " +
						rmessage.getPort ());
				System.out.println(new String(rmessage.getData()));
				
				byte[] distanceMessage = new byte[doublesToSend*8];
				//ByteBuffer.wrap(distanceMessage).put((byte)1);
				ByteBuffer bf = ByteBuffer.allocate(distanceMessage.length);
				bf.order(ByteOrder.LITTLE_ENDIAN).wrap(distanceMessage).putDouble(0, d);
				bf.order(ByteOrder.LITTLE_ENDIAN).wrap(distanceMessage).putDouble(8, c1);
				bf.order(ByteOrder.LITTLE_ENDIAN).wrap(distanceMessage).putDouble(16, c2);
				
				DatagramPacket smessage = new DatagramPacket(distanceMessage, distanceMessage.length, rmessage.getAddress() ,rmessage.getPort());
				
				serverSocket.send(smessage);
				System.out.println("sending message");
//				hl.sendCapacity1ToHololens(serverSocket, c1);
//				hl.sendCapacity2ToHololens(serverSocket, c2);
//				hl.sendDistanceToHololens(serverSocket, d);
			}
		}
	}

