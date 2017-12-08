using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class LogReader : MonoBehaviour
{

	private static int maxLinesToSkip = 13;
	private static int actualLineOfBox = 1;
	private static int linesOfBlock = 9;

	private static Vector3 kugel;
	private static Vector3 sensorBoard;
	private static Vector3 kugelOrientation;
	private static Vector3 sensorBoardOrientation;
	private static float distance;
	private static float capacity_1;
	private static float capacity_2;               

	/// <summary>
	/// Reads out the values from the logs that are necessary to create a logbox 
	/// </summary>
	/// <returns>A Dictionary with each seperate logfile with all logboxes in it</returns>
	/// <param name="directoryPath">Directory path.</param>
	/// <param name="fileSelectionString">File selection string.</param>
	public static  Dictionary<int, List<LogBox>>  ReadLogs (string directoryPath, string fileSelectionString)
	{
		var logFiles = Directory.GetFiles (directoryPath, fileSelectionString);
		Dictionary<int, List<LogBox>> allLogFiles = new Dictionary<int, List<LogBox>>  ();
		int linesToSkip = maxLinesToSkip;
		int fileCounter = 0;

		foreach (string log in logFiles)
		{
			List<LogBox> singleLogFile = new List<LogBox>();
			var logParser = new StreamReader (log);
			while (!logParser.EndOfStream)
			{
				var line = logParser.ReadLine ();

				//skip the first lines of a logfile until the first line of a logbox
				if (linesToSkip <= 0)
				{
					var lineValues = line.Split ('	');
					if (actualLineOfBox == 3)
					{
						//in the third line of a logbox is the position and orientation of the kugel
						kugel.x = float.Parse (lineValues [0].Substring (21));
						kugel.y = float.Parse (lineValues [2].Substring (2));
						kugel.z = float.Parse (lineValues [3].Substring (2));
						kugelOrientation.x = float.Parse (lineValues [5].Substring (2));
						kugelOrientation.y = float.Parse (lineValues [6].Substring (2));
						kugelOrientation.z = float.Parse (lineValues [7].Substring (2));
					}
					else if (actualLineOfBox == 4)
					{
						//in the fourth line of a logbox is the position and orientation of the sensorboard
						sensorBoard.x = float.Parse (lineValues [0].Substring (21));
						sensorBoard.y = float.Parse (lineValues [2].Substring (2));
						sensorBoard.z = float.Parse (lineValues [3].Substring (2));
						sensorBoardOrientation.x = float.Parse (lineValues [5].Substring (2));
						sensorBoardOrientation.y = float.Parse (lineValues [6].Substring (2));
						sensorBoardOrientation.z = float.Parse (lineValues [7].Substring (2));
					}
					else if (actualLineOfBox == 5)
					{
						//in the fifth line of a logbox is the distance between the logged position of the kugel and the sensorboard
						distance = float.Parse (lineValues [0].Substring (36));
					}
					else if (actualLineOfBox == 7)
					{
						//in the seventh line of a logbox are the two capacity values
						capacity_1 = float.Parse (lineValues [0].Substring (19));
						capacity_2 = float.Parse (lineValues [2].Substring (19));
					}

					actualLineOfBox++;
					if (actualLineOfBox == linesOfBlock)
					{
						singleLogFile.Add(new LogBox(kugel, sensorBoard, distance, capacity_1, capacity_2));

						actualLineOfBox = 1;
					}
						
				}
				linesToSkip--;
			}
			allLogFiles.Add(fileCounter, singleLogFile);
			fileCounter++;
			linesToSkip = maxLinesToSkip;
			actualLineOfBox = 1;
		}
		return allLogFiles;
	}
}
