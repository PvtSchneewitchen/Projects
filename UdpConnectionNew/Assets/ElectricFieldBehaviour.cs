using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFieldBehaviour : MonoBehaviour
{
    private static readonly int numberOfSpheresOnLevel = 16;
    private static readonly float sphereScale = 0.025f;
    private GameObject[] level0 = new GameObject[numberOfSpheresOnLevel];
    private GameObject[] level1 = new GameObject[numberOfSpheresOnLevel];
    private GameObject[] level2 = new GameObject[numberOfSpheresOnLevel];
    private GameObject[] level3 = new GameObject[numberOfSpheresOnLevel];
    private GameObject[] level4 = new GameObject[numberOfSpheresOnLevel];

    // Use this for initialization
    void Start ()
    {
        createFieldSpheres();
        createFieldGridHorizontal();
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void createFieldSpheres()
    {
        float angle;
        float radius;
        Vector3 levelGap;
        Vector3 fieldOrigin = Camera.main.transform.localPosition + new Vector3(0.0f, 0.0f, 3.0f);

        angle = 0.0f;
        radius = 2.0f;
        for (int i = 0; i < level0.Length; i++)
        {
            level0[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            level0[i].transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
            level0[i].transform.localPosition = fieldOrigin + new Vector3(radius * Mathf.Cos(angle), 0.0f, radius * Mathf.Sin(angle));

            angle += 360.0f / numberOfSpheresOnLevel;
        }

        angle = 0.0f;
        radius = 1.7f;
        levelGap = new Vector3(0.0f, 0.4f, 0.0f);
        for (int i = 0; i < level1.Length; i++)
        {
            level1[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            level1[i].transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
            level1[i].transform.localPosition = fieldOrigin + levelGap + new Vector3(radius * Mathf.Cos(angle), 0.0f, radius * Mathf.Sin(angle));

            angle += 360.0f / numberOfSpheresOnLevel;
        }

        angle = 0.0f;
        radius = 1.2f;
        levelGap = new Vector3(0.0f, 0.7f, 0.0f);
        for (int i = 0; i < level2.Length; i++)
        {
            level2[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            level2[i].transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
            level2[i].transform.localPosition = fieldOrigin + levelGap + new Vector3(radius * Mathf.Cos(angle), 0.0f, radius * Mathf.Sin(angle));

            angle += 360.0f / numberOfSpheresOnLevel;
        }

        angle = 0.0f;
        radius = 0.6f;
        levelGap = new Vector3(0.0f, 0.9f, 0.0f);
        for (int i = 0; i < level3.Length; i++)
        {
            level3[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            level3[i].transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
            level3[i].transform.localPosition = fieldOrigin + levelGap + new Vector3(radius * Mathf.Cos(angle), 0.0f, radius * Mathf.Sin(angle));

            angle += 360.0f / numberOfSpheresOnLevel;
        }

        angle = 0.0f;
        radius = 0.15f;
        levelGap = new Vector3(0.0f, 1.05f, 0.0f);
        for (int i = 0; i < level4.Length; i++)
        {
            level4[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            level4[i].transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
            level4[i].transform.localPosition = fieldOrigin + levelGap + new Vector3(radius * Mathf.Cos(angle), 0.0f, radius * Mathf.Sin(angle));

            angle += 360.0f / numberOfSpheresOnLevel;
        }
    }

    private void createFieldGrid()
    {
        
    }

    private void createFieldGridHorizontal()
    {
        LineRenderer line = new LineRenderer();
        line.startWidth = sphereScale;
        line.endWidth = sphereScale;
        line.alignment = LineAlignment.Local;
        line.startColor = Color.white;

        line.SetPosition(0, level0[0].transform.localPosition);
        line.SetPosition(1, level0[1].transform.localPosition);

        line.enabled = true;

        /*        for (int i = 0; i < level0.Length; i++)
                {
                    line.SetPosition(0, level0[i].transform.localPosition);
                    line.SetPosition(1, level0[i+1].transform.localPosition);
                }*/
    }

    private void createFielGridVertical()
    {

    }
}
