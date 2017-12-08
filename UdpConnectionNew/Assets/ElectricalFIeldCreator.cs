using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElectricalFieldCreator
{
	private float fMeasurePointScale = 0.01f;
	//1cm

	/// <summary>
	/// Creates the electrical field from the logs in the directorypath depending on the value of the "logname"
	/// </summary>
	/// <returns>The created Electrical Field</returns>
	/// <param name="directorypath">Directorypath.</param>
	/// <param name="logname">Logname -> name of the file or beginning of the files</param></param>
	public ElectricalField CreateElectricalFieldFromLogs (string directorypath, string logname)
	{
		Dictionary<int, List<LogBox>> LogObjectsAll = new Dictionary<int, List<LogBox>> ();
		Dictionary<int, List<FieldPoint>> FieldPoints = new Dictionary<int, List<FieldPoint>> ();

		//create a dictionary: key=log ID	value=one related record
		LogObjectsAll = LogReader.ReadLogs (directorypath, logname);

		//Convert those records into fieldpoints and Translate them to the unity coordinate system
		FieldPoints = ViconCsToUnityCs (CreateLogToFieldPoint (LogObjectsAll));

		//create origin gameobject depending on the mean origin position from all log records and translate its position to the unity coordinate system
		GameObject goOrigin = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		goOrigin.transform.localScale = new Vector3 (fMeasurePointScale, fMeasurePointScale, fMeasurePointScale);
		goOrigin.transform.position = GetMeanOriginPosition (LogObjectsAll);
		goOrigin = ViconCsToUnityCs (goOrigin);
		goOrigin.GetComponent<Renderer> ().material.color = Color.red;

		ElectricalField eField = new ElectricalField (goOrigin, FieldPoints);

		return eField;
	}

	/// <summary>
	/// Computes the average position of the origin from alle origins of the Logs
	/// </summary>
	/// <returns>The mean origin position.</returns>
	/// <param name="logObjects">All related Log records</param>
	private static Vector3 GetMeanOriginPosition (Dictionary<int, List<LogBox>> logObjects)
	{
		Vector3 mean = Vector3.zero;
		int sum = 0;
		for (int i = 0; i < logObjects.Count; i++)
		{
			for (int j = 0; j < logObjects [i].Count; j++)
			{
				mean += logObjects [i] [j].sensorBoard;
				sum++;
			}
		}
		return mean / sum;
	}

	/// <summary>
	/// Creates A Fieldpoint Dictionary from all Logfiles where the key is the Log ID and the value is a list of all related Fieldpoints to that Log
	/// </summary>
	/// <returns>The Fieldpoint Dictionary</returns>
	/// <param name="inpLogObjects">All related Log records</param>
	private Dictionary<int, List<FieldPoint>> CreateLogToFieldPoint (Dictionary<int, List<LogBox>> inpLogObjects)
	{
		Dictionary<int, List<FieldPoint>> FieldPoints = new Dictionary<int, List<FieldPoint>> ();

		for (int i = 0; i < inpLogObjects.Count; i++)
		{
			List<FieldPoint> fpoints = new List<FieldPoint> ();
			for (int j = 0; j < inpLogObjects [i].Count; j++)
			{
				GameObject goMeasurePoint = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				goMeasurePoint.name = "MeasurePoint";
				goMeasurePoint.transform.localScale = new Vector3 (fMeasurePointScale, fMeasurePointScale, fMeasurePointScale);
				goMeasurePoint.transform.position = new Vector3 (inpLogObjects [i] [j].kugel.x, inpLogObjects [i] [j].kugel.y, inpLogObjects [i] [j].kugel.z); //World Position
				goMeasurePoint.GetComponent<Renderer> ().material.color = Color.cyan;

				FieldPoint fpoint = new FieldPoint (goMeasurePoint,inpLogObjects [i] [j].fDistanceToOrigin ,inpLogObjects [i] [j].fCapacity_1, inpLogObjects [i] [j].fCapacity_2);
				fpoints.Add (fpoint);
			}
			FieldPoints.Add (i, fpoints);
		}
		return FieldPoints;
	}

	/// <summary>
	/// Rotates the position of the Fieldpoints by -90° around the world origin so that the points recorded with the vicon system are displayed correctly in unity
	/// </summary>
	/// <returns>The Rotated Points</returns>
	/// <param name="Points">All Fieldpoints</param>
	private Dictionary<int, List<FieldPoint>> ViconCsToUnityCs (Dictionary<int, List<FieldPoint>> Points)
	{
		for (int i = 0; i < Points.Count; i++)
		{
			for (int j = 0; j < Points [i].Count; j++)
			{
				Points [i] [j].goPoint.transform.RotateAround (Vector3.zero, Vector3.right, -90);
			}
		}
		return Points;
	}

	/// <summary>
	/// Rotates the position of the gameobject by -90° around the world origin so that the object recorded with the vicon system is displayed correctly in unity
	/// </summary>
	/// <returns>The Rotated Object</returns>
	/// <param name="Points">Object to be rotated</param>
	private GameObject ViconCsToUnityCs (GameObject Point)
	{
		Point.transform.RotateAround (Vector3.zero, Vector3.right, -90);
		return Point;
	}
}