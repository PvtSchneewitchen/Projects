using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElectricalFieldHelper{

	/// <summary>
	/// Attachs origin to sensorboard 
	/// </summary>
	/// <param name="board">Sensorboard Gameobject</param>
	/// <param name="origin">Origin Gameobject</param>
	public static void AttachToSensorBoard (GameObject board, GameObject origin)
	{
		origin.transform.position = board.transform.position;
		origin.transform.SetParent (board.transform);
	}

	/// <summary>
	/// Sets the colors of the Field dependig on computed Colors and hides all points
	/// </summary>
	/// <param name="fieldpoints">Dictionary where the key is the ID of a log and the values are the referring points to that log </param>
	public static void SetColorsAndHide (Dictionary<int, List<FieldPoint>> fieldpoints)
	{
		List<Color> FieldLayerColors = CreateColors(fieldpoints);

		for (int i = 0; i < fieldpoints.Count; i++)
		{
			for (int j = 0; j < fieldpoints [i].Count; j++)
			{
				fieldpoints[i] [j].goPoint.GetComponent<Renderer> ().material.color = FieldLayerColors [i];
				fieldpoints [i] [j].goPoint.GetComponent<Renderer> ().enabled = false;
			}
		}
	}

	/// <summary>
	/// Creates the colors of the field depending on the distance to the origin from red to yellow to cyan
	/// </summary>
	/// <returns>A list of the created colors where the size is the same as the amount of logs</returns>
	/// <param name="fieldpoints">Dictionary where the key is the ID of a log and the values are the referring points to that log </param>
	public static List<Color> CreateColors (Dictionary<int, List<FieldPoint>> fieldpoints)
	{
		List<Color> colors = new List<Color> ();

		//from red to yellow
		for (int i = 1; i <= Mathf.Floor (fieldpoints.Count / 2); i++)
		{
			float value = i * (1 / Mathf.Floor (fieldpoints.Count / 2));

			colors.Add (new Color (1, value, 0, 1));
		}

		//from yellow to cyan
		for (int i = 1; i <= Mathf.Ceil (fieldpoints.Count / 2); i++)
		{
			float value = i * (1 / Mathf.Ceil (fieldpoints.Count / 2));

			colors.Add (new Color (0, 1, value, 1));
		}

		return colors;
	}

	/// <summary>
	/// Computes the new points to shown depending on the similarity to the given capacity sum
	/// </summary>
	/// <returns>The new Points to shown as an array of gameobjects</returns>
	/// <param name="cpapcitysum">Current Capacity sum from the sonsorboard</param>
	/// <param name="sortedbycapacitysum">List of all Fieldpoints sorted ascended by their capacity sum</param>
	/// <param name="size">The amount of points to show</param>
	public static GameObject[] UpdatePointsToShow (float cpapcitysum, List<KeyValuePair<float, GameObject>> sortedbycapacitysum, int size)
	{
		GameObject[] UpdatedPointsToShow = new GameObject[size];

		List<KeyValuePair<float, GameObject>> SortedByCapacityDifference = new List<KeyValuePair<float, GameObject>> ();

		for (int i = 0; i < sortedbycapacitysum.Count; i++)
		{
			SortedByCapacityDifference.Add (new KeyValuePair<float, GameObject> (Mathf.Abs (sortedbycapacitysum [i].Key - cpapcitysum), sortedbycapacitysum [i].Value));
		}

		SortedByCapacityDifference.Sort ((capI, capJ) => capI.Key.CompareTo (capJ.Key));

		for (int i = 0; i < size - 1; i++)
		{
			UpdatedPointsToShow [i] = SortedByCapacityDifference [i].Value;
		}

		return UpdatedPointsToShow;
	}

	/// <summary>
	/// Makes the new points visible.
	/// </summary>
	/// <param name="newpoints">Newpoints.</param>
	/// <param name="currentpoints">Currentpoints.</param>
	public static void MakeNewPointsVisible (GameObject[] newpoints, GameObject[] currentpoints)
	{
		//hide old points
		foreach (var point in currentpoints)
		{
			if (point != null)
			{
				point.GetComponent <Renderer> ().enabled = false;
			}
		}

		//show new points
		foreach (var point in newpoints)
		{
			if (point != null)
			{
				point.GetComponent <Renderer> ().enabled = true;
			}
		}
	}

	/// <summary>
	/// Gets Points with the highest distance to the origin
	/// </summary>
	/// <returns>The outermost points.</returns>
	/// <param name="fieldpoints">Dictionary where the key is the ID of a log and the values are the referring points to that log </param>
	/// <param name="size">the amount of outermost points to compute</param>
	public static GameObject[] GetOutermostPoints (Dictionary<int, List<FieldPoint>> fieldpoints, int size)
	{
		//Get Points with the highest distance index
		GameObject[] OutermostPoints = new GameObject[size];
		List<KeyValuePair<float, GameObject>> DistanceSortedList = GetDistanceSort (fieldpoints);

		for (int i = 0; i < OutermostPoints.Length; i++)
		{
			OutermostPoints [i] = DistanceSortedList [DistanceSortedList.Count - 1 - OutermostPoints.Length - 1 + i].Value;
		}

		return OutermostPoints;
	}

	/// <summary>
	/// Gets Points with the lowest distance to the origin
	/// </summary>
	/// <returns>The innermost points.</returns>
	/// <param name="fieldpoints">Dictionary where the key is the ID of a log and the values are the referring points to that log </param>
	/// <param name="size">the amount of innermost points to compute</param>
	public static GameObject[] GetInnermostPoints (Dictionary<int, List<FieldPoint>> fieldpoints, int size)
	{
		//Get Points with the lowest distance index
		GameObject[] InnermostPoints = new GameObject[size];
		List<KeyValuePair<float, GameObject>> DistanceSortedList = GetDistanceSort (fieldpoints);

		for (int i = 0; i < InnermostPoints.Length; i++)
		{
			InnermostPoints [i] = DistanceSortedList [i].Value;
		}


		return InnermostPoints;
	}

	/// <summary>
	/// Sorts the fieldpoints depending on the values of their capacity sums ascending
	/// </summary>
	/// <returns>The sorted capacity sum list.</returns>
	/// <param name="fieldpoints">Dictionary where the key is the ID of a log and the values are the referring points to that log .</param>
	public static List<KeyValuePair<float, GameObject>> CreateSortedCapacitySumList (Dictionary<int, List<FieldPoint>> fieldpoints)
	{
		List<KeyValuePair<float, GameObject>> CapacitySumList = new List<KeyValuePair<float, GameObject>> ();

		for (int i = 0; i < fieldpoints.Count; i++)
		{
			for (int j = 0; j < fieldpoints [i].Count; j++)
			{
				CapacitySumList.Add (new KeyValuePair<float, GameObject> (fieldpoints [i] [j].fCapacity1 + fieldpoints [i] [j].fCapacity2, fieldpoints [i] [j].goPoint)); 
			}
		}
		CapacitySumList.Sort ((capI, capJ) => capI.Key.CompareTo (capJ.Key));

		return CapacitySumList;
	}

	/// <summary>
	/// Sorts the fieldpoints depending on the values of their distance to the origin ascending
	/// </summary>
	/// <returns>The distance sort.</returns>
	/// <param name="fieldpoints">Dictionary where the key is the ID of a log and the values are the referring points to that log .</param>
	public static List<KeyValuePair<float, GameObject>> GetDistanceSort (Dictionary<int, List<FieldPoint>> fieldpoints)
	{
		List<KeyValuePair<float, GameObject>> DistanceSortedList = new List<KeyValuePair<float, GameObject>> ();

		for (int i = 0; i < fieldpoints.Count; i++)
		{
			for (int j = 0; j < fieldpoints [i].Count; j++)
			{
				DistanceSortedList.Add (new KeyValuePair<float, GameObject> (fieldpoints [i] [j].fDistanceToOrigin, fieldpoints [i] [j].goPoint)); 
			}
		}
		DistanceSortedList.Sort ((capI, capJ) => capI.Key.CompareTo (capJ.Key));

		return DistanceSortedList;
	}

//	private float GetMaxCapacitiy1 (Dictionary<int, List<FieldPoint>> fieldpoints)
//	{
//		List<float> fCapacities = new List<float> ();
//
//		for (int i = 0; i < fieldpoints.Count; i++)
//		{
//			for (int j = 0; j < fieldpoints [i].Count; j++)
//			{
//				fCapacities.Add (fieldpoints [i] [j].fCapacity1);
//			}
//		}
//
//		return fCapacities.Max ();
//	}
//
//	private float GetMaxCapacitiy2 (Dictionary<int, List<FieldPoint>> fieldpoints)
//	{
//		List<float> fCapacities = new List<float> ();
//
//		for (int i = 0; i < fieldpoints.Count; i++)
//		{
//			for (int j = 0; j < fieldpoints [i].Count; j++)
//			{
//				fCapacities.Add (fieldpoints [i] [j].fCapacity2);
//			}
//		}
//
//		return fCapacities.Max ();
//	}
//
//	private float GetMinCapacitiy1 (Dictionary<int, List<FieldPoint>> fieldpoints)
//	{
//		List<float> fCapacities = new List<float> ();
//
//		for (int i = 0; i < fieldpoints.Count; i++)
//		{
//			for (int j = 0; j < fieldpoints [i].Count; j++)
//			{
//				fCapacities.Add (fieldpoints [i] [j].fCapacity1);
//			}
//		}
//
//		return fCapacities.Min ();
//	}
//
//	private float GetMinCapacitiy2 (Dictionary<int, List<FieldPoint>> fieldpoints)
//	{
//		List<float> fCapacities = new List<float> ();
//
//		for (int i = 0; i < fieldpoints.Count; i++)
//		{
//			for (int j = 0; j < fieldpoints [i].Count; j++)
//			{
//				fCapacities.Add (fieldpoints [i] [j].fCapacity2);
//			}
//		}
//
//		return fCapacities.Min ();
//	}
}
