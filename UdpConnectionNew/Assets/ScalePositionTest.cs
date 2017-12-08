using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePositionTest : MonoBehaviour {

	private GameObject parent;
	private GameObject child;
	private GameObject grandchild;

	// Use this for initialization
	void Start () {
		parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
		child = GameObject.CreatePrimitive(PrimitiveType.Cube);
		grandchild = GameObject.CreatePrimitive(PrimitiveType.Cube);

		parent.transform.name = "parent";
		child.transform.name = "child";
		grandchild.transform.name = "grandchild";

		parent.transform.name = "parent";
		child.transform.name = "child";
		grandchild.transform.name = "grandchild";

		parent.GetComponent<Renderer>().material.color = Color.red;
		child.GetComponent<Renderer>().material.color = Color.blue;
		grandchild.GetComponent<Renderer>().material.color = Color.green;

		child.transform.SetParent(parent.transform);
		grandchild.transform.SetParent(child.transform);

		child.transform.localPosition += new Vector3(1,1,1);
		grandchild.transform.localPosition += new Vector3(1,1,1); 

//		parent.transform.DetachChildren();
//		parent.transform.localScale /= 2;
//		child.transform.DetachChildren();
//		child.transform.localScale /= 2;
		grandchild.transform.localScale /= 2;
		child.transform.localScale /= 2;
	
	}
	
	// Update is called once per frame
	void Update () {
				
	}
}
