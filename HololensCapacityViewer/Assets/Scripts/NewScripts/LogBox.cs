using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBox{

	public Vector3 kugel;
	public Vector3 sensorBoard;
	public readonly float fDistanceToOrigin;
	public readonly float fCapacity_1;
	public readonly float fCapacity_2;

	public LogBox(Vector3 kugelInp, Vector3 sensorBoardInp, float distanceInp, float capacity_1Inp, float capacity_2Inp)
	{
		kugel = kugelInp;
		sensorBoard = sensorBoardInp;
		fDistanceToOrigin = distanceInp;
		fCapacity_1 = capacity_1Inp;
		fCapacity_2 = capacity_2Inp;
	}
}