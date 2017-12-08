using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalField
{

	public GameObject goOrigin;
	public Dictionary<int, List<FieldPoint>> FieldPoints;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricalField"/> class and sets the origin as parent to all fieldpoints.
	/// </summary>
	/// <param name="origin">Origin.</param>
	/// <param name="fieldpoints">Fieldpoints.</param>
	public ElectricalField (GameObject origin, Dictionary<int, List<FieldPoint>> fieldpoints)
	{
		goOrigin = origin;
		FieldPoints = fieldpoints; 

		for (int i = 0; i < fieldpoints.Count; i++)
		{
			for (int j = 0; j < fieldpoints[i].Count; j++) 
			{
				fieldpoints[i][j].goPoint.transform.SetParent(goOrigin.transform);
			}
		}
	}

}
