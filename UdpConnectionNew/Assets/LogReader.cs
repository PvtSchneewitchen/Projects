using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class LogReader : MonoBehaviour
{

	private static int maxLinesToSkip = 13;
	private static int actualLineOfBlock = 1;
	private static int linesOfBlock = 9;

	private static Vector3 kugel;
	private static Vector3 sensorBoard;
	private static Vector3 kugelOrientation;
	private static Vector3 sensorBoardOrientation;
	private static float distance;
	private static float capacity_1;
	private static float capacity_2;

	public static  List<LogStruct> GetLogPoints (string directoryPath)
	{
		var logFiles = Directory.GetFiles (directoryPath, "log_distance_border_l*.txt");
		List<LogStruct> logPoints = new List<LogStruct> ();
		int linesToSkip = maxLinesToSkip;

		foreach (string log in logFiles)
		{
			var logParser = new StreamReader (log);
			while (!logParser.EndOfStream)
			{
				var line = logParser.ReadLine ();
				if (linesToSkip <= 0)
				{
					var lineValues = line.Split ('	');
					if (actualLineOfBlock == 3)
					{
						kugel.x = float.Parse (lineValues [0].Substring (21));
						kugel.y = float.Parse (lineValues [2].Substring (2));
						kugel.z = float.Parse (lineValues [3].Substring (2));
						kugelOrientation.x = float.Parse (lineValues [5].Substring (2));
						kugelOrientation.y = float.Parse (lineValues [6].Substring (2));
						kugelOrientation.z = float.Parse (lineValues [7].Substring (2));
					}
					else if (actualLineOfBlock == 4)
					{
						sensorBoard.x = float.Parse (lineValues [0].Substring (21));
						sensorBoard.y = float.Parse (lineValues [2].Substring (2));
						sensorBoard.z = float.Parse (lineValues [3].Substring (2));
						sensorBoardOrientation.x = float.Parse (lineValues [5].Substring (2));
						sensorBoardOrientation.y = float.Parse (lineValues [6].Substring (2));
						sensorBoardOrientation.z = float.Parse (lineValues [7].Substring (2));
					}
					else if (actualLineOfBlock == 5)
					{
						distance = float.Parse (lineValues [0].Substring (36));
					}
					else if (actualLineOfBlock == 7)
					{
						capacity_1 = float.Parse (lineValues [0].Substring (19));
						capacity_2 = float.Parse (lineValues [2].Substring (19));
					}

					actualLineOfBlock++;
					if (actualLineOfBlock == linesOfBlock)
					{
						logPoints.Add(new LogStruct(kugel, sensorBoard, kugelOrientation, sensorBoardOrientation, distance, capacity_1, capacity_2));
						actualLineOfBlock = 1;
					}
						
				}
				linesToSkip--;
			}
			linesToSkip = maxLinesToSkip;
			actualLineOfBlock = 1;
		}
		return logPoints;
	}
}
