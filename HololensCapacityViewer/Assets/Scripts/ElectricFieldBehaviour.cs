using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//using NUnit.Framework.Constraints;
using System.Dynamic;

public class ElectricFieldBehaviour : MonoBehaviour
{
	private string sDirectoryPath = "C:\\Users\\gruepazu\\Documents\\Github\\Projects\\capacitivesensor\\SensorDataViewer";
	private string sFileNames = "log_distance_border_*.txt";
	public static bool bShowField = true;

	private int iFieldPointsToShow;
	private ElectricalField eField;
	private List<KeyValuePair<float, GameObject>> PointSortedByCapacitysum = new List<KeyValuePair<float, GameObject>> ();
	private GameObject[] CurrentPointsShown = new GameObject[0];
	private GameObject SensorBoard;

	void Start ()
	{
		//Create an Efield from the logs in the directorypath
		ElectricalFieldCreator efc = new ElectricalFieldCreator ();
		eField = efc.CreateElectricalFieldFromLogs (sDirectoryPath, sFileNames);

		//set the maximum points to show to the amount of points from the log with the least distance to the origin 
		iFieldPointsToShow = eField.FieldPoints.First ().Value.Count;
		//initialize the array for the points that are currently shown with the maximum points to shown from above
		CurrentPointsShown = new GameObject[iFieldPointsToShow];

		//Give all points from the field a color depending on the distance to the origin and then hide all points
		ElectricalFieldHelper.SetColorsAndHide (eField.FieldPoints);

		//create a list of capacity sums and referring points and sort that list ascending by the value of this sums
		PointSortedByCapacitysum = ElectricalFieldHelper.CreateSortedCapacitySumList (eField.FieldPoints);

		//hide the origin point
		eField.goOrigin.GetComponent<Renderer> ().enabled = false;

		//initialize the sensorboard object 
		SensorBoard = GameObject.Find ("SensorBoard");

		//TODO delete
		SensorBoard.GetComponent<Renderer> ().material.color = new Color (1 / 142, 1 / 104, 0);

		//Atach the origin (parent of all fieldpoints) to the sonsorboard
		ElectricalFieldHelper.AttachToSensorBoard (SensorBoard, eField.goOrigin);
	}

	void Update ()
	{
		if (bShowField)
		{
			//sensorboard was found by hololens so bShowField was set to true by trackable event handler from sensorboard gameobject

			//initialize the array for the new computed points to show
			GameObject[] UpdatedPointsToShow = new GameObject[iFieldPointsToShow];

			//get the capacity from the datareciever
			float fCapacitySum = DataReceiver.GetCapacity1 () + DataReceiver.GetCapacity2 (); 


			if (fCapacitySum >= PointSortedByCapacitysum.Last ().Key)
			{
				//Capacity sum is bigger than the highest value from the fieldpoints so all innermost points has to be shown
				UpdatedPointsToShow = ElectricalFieldHelper.GetInnermostPoints (eField.FieldPoints, iFieldPointsToShow);
			}
			else if (fCapacitySum <= PointSortedByCapacitysum.First ().Key)
			{
				//Capacity sum is smaller than the smallest value from the fieldpoints so all outermost points has to be shown
				UpdatedPointsToShow = ElectricalFieldHelper.GetOutermostPoints (eField.FieldPoints, iFieldPointsToShow);
			}
			else
			{
				//Capacity sum is between biggest and smallest value so new points to show have to be computed
				UpdatedPointsToShow = ElectricalFieldHelper.UpdatePointsToShow (fCapacitySum, PointSortedByCapacitysum, iFieldPointsToShow);	
			}

			//make the new points visible and set them as current shown points
			ElectricalFieldHelper.MakeNewPointsVisible (UpdatedPointsToShow, CurrentPointsShown);
			CurrentPointsShown = UpdatedPointsToShow;
		}
		else
		{
			//sensorboard was lost or not found by hololens

			//Hide the shown fieldpoints
			foreach (var point in CurrentPointsShown)
			{
				if (point != null)
				{
					point.GetComponent <Renderer> ().enabled = false;
				}
			}
		}
	}

}
