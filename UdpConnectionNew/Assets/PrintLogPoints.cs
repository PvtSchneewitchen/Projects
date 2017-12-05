using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintLogPoints : MonoBehaviour {

	public string directoryPath = "C:\\Users\\gruepazu\\Documents\\Neuer Ordner\\Projects\\capacitivesensor\\SensorDataViewer";
	public float pointSize = 0.001f;
	public float sizeFactor = 1;

	// Use this for initialization
	void Start () {
		List<LogStruct> logElements = new List<LogStruct>();
		logElements = LogReader.GetLogPoints(directoryPath);


		foreach (var element in logElements)
		{
			GameObject fieldPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			fieldPoint.name = "point";
			fieldPoint.transform.localScale = new Vector3(pointSize,pointSize,pointSize);
			fieldPoint.transform.position = new Vector3(element.kugel.x*sizeFactor, element.kugel.y*sizeFactor, element.kugel.z*sizeFactor);

			GameObject origin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			origin.name = "origin";
			origin.transform.localScale = new Vector3(pointSize,pointSize,pointSize);
			origin.transform.position = new Vector3(element.sensorBoard.x*sizeFactor, element.sensorBoard.y*sizeFactor, element.sensorBoard.z*sizeFactor);
			origin.GetComponent<Renderer>().material.color = Color.red;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
