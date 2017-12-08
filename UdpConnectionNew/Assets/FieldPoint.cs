using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPoint
{

	public GameObject goPoint;
	public readonly float fCapacity1;
	public readonly float fCapacity2;
	public readonly float fDistanceToOrigin;

	public FieldPoint (GameObject point, float distance, float capacity1, float capacity2)
	{
		goPoint = point;
		fDistanceToOrigin = distance;
		fCapacity1 = capacity1;
		fCapacity2 = capacity2;
	}
}
