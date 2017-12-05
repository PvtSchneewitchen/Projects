using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricFieldBehaviour : MonoBehaviour
{
    private static int numberOfSpheresOnLevel = 16;
    private static float sphereScale = 0.0025f;
    private static int numberofLevels = 5;
    private static float maxRadius = 0.3f;
    private static Vector3 maxleveGap = new Vector3(0, maxRadius, 0);
    private static Dictionary<int, GameObject[]> sphereOnLevel = new Dictionary<int, GameObject[]>();

    // Use this for initialization
    void Start ()
    {
        createFieldSpheres();
        createFieldGridHorizontal();
        createFielGridVertical();

    }
	
	// Update is called once per frame
	void Update ()
	{

	}

    private void createFieldSpheres()
    {
        float angle = 0.0f;
        float radius = maxRadius;
        Vector3 levelGap = new Vector3(0,0,0);
        Vector3 fieldOrigin = Camera.main.transform.position + new Vector3(0.0f, 0.0f, 1.0f);
        

        float[] customRadius = new[] {maxRadius,0.27f, 0.2f, 0.1f, 0.02f};
        float[] customLeveGaps = new[] { 0.03f, 0.07f, 0.11f, 0.14f, 0.16f};

        for (int i = 0; i < numberofLevels; i++)
        {
            sphereOnLevel.Add(i, new GameObject[numberOfSpheresOnLevel]);

            //radius = (i == 0) ? maxRadius : maxRadius - maxRadius;
            radius = customRadius[i];
            for (int j = 0; j < numberOfSpheresOnLevel; j++)
            {
                sphereOnLevel[i][j] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereOnLevel[i][j].transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
                if(i==0)
                    sphereOnLevel[i][j].transform.position = fieldOrigin + new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angle), 0.0f, radius * Mathf.Sin(Mathf.Deg2Rad * angle));
                else
                    sphereOnLevel[i][j].transform.position = fieldOrigin + new Vector3(0, customLeveGaps[i], 0) + new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angle), 0.0f, radius * Mathf.Sin(Mathf.Deg2Rad * angle));

                angle += 360.0f / numberOfSpheresOnLevel;
            }
            angle = 0.0f;
            
            levelGap += new Vector3(0, maxleveGap.y / numberofLevels, 0);
        }
    }

    private void createFieldGridHorizontal()
    {
        Dictionary<int, LineRenderer> horizontalLineRenderers = new Dictionary<int, LineRenderer>();
        horizontalLineRenderers = createHorizontalLineRenderers();

               for (int i = 0; i < numberofLevels; i++)
                {
                    horizontalLineRenderers[i].positionCount = numberOfSpheresOnLevel+1;
                    horizontalLineRenderers[i].startWidth = sphereScale/2;
                    horizontalLineRenderers[i].enabled = true;
                    //add more LR-Attributes here
                    for (int j = 0; j < numberOfSpheresOnLevel; j++)
                    {
                        horizontalLineRenderers[i].SetPosition(j,sphereOnLevel[i][j].transform.localPosition);
                        Debug.Log(sphereOnLevel[i][j].transform.localPosition);
                    }
                    horizontalLineRenderers[i].SetPosition(numberOfSpheresOnLevel, sphereOnLevel[i][0].transform.localPosition);
        }
    }

    private Dictionary<int, LineRenderer> createHorizontalLineRenderers()
    {
        Dictionary<int, LineRenderer> horizontalLineRenderers = new Dictionary<int, LineRenderer>();
        

        for (int i = 0; i < numberofLevels; i++)
        {
            horizontalLineRenderers.Add(i, (new GameObject("line")).AddComponent<LineRenderer>());
        }

        return horizontalLineRenderers;
    }

    private void createFielGridVertical()
    {
        Dictionary<int, LineRenderer> verticalLineRenderers = new Dictionary<int, LineRenderer>();
        verticalLineRenderers = createVerticalLineRenderers();

        for (int i = 0; i < numberOfSpheresOnLevel; i++)
        {
            verticalLineRenderers[i].positionCount = numberofLevels;
            verticalLineRenderers[i].startWidth = sphereScale/2;
            verticalLineRenderers[i].enabled = true;
            //add more LR-Attributes here
            for (int j = 0; j < numberofLevels; j++)
            {
                verticalLineRenderers[i].SetPosition(j, sphereOnLevel[j][i].transform.localPosition);
            }
        }
    }

    private Dictionary<int, LineRenderer> createVerticalLineRenderers()
    {
        Dictionary<int, LineRenderer> verticalLineRenderers = new Dictionary<int, LineRenderer>();


        for (int i = 0; i < numberOfSpheresOnLevel; i++)
        {
            verticalLineRenderers.Add(i, (new GameObject("line")).AddComponent<LineRenderer>());
        }

        return verticalLineRenderers;
    }
}
