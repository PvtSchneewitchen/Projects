using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogStruct{

	public readonly Vector3 kugel;
	public readonly Vector3 sensorBoard;
	public readonly Vector3 kugelOrientation;
	public readonly Vector3 sensorBoardOrientation;
	public readonly float distance;
	public readonly float capacity_1;
	public readonly float capacity_2;

	public LogStruct(Vector3 kugelInp, Vector3 sensorBoardInp, Vector3 kugelOrientationInp, Vector3 sensorBoardOrientationInp, float distanceInp, float capacity_1Inp, float capacity_2Inp)
	{
		kugel = kugelInp;
		sensorBoard = sensorBoardInp;
		kugelOrientation = kugelOrientationInp;
		sensorBoardOrientation = sensorBoardOrientationInp;
		distance = distanceInp;
		capacity_1 = capacity_1Inp;
		capacity_2 = capacity_2Inp;
	}
}
