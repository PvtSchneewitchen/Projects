using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricFieldBehaviour : MonoBehaviour
{
    private static int numberOfSpheresOnLevel = 16;
    private static float sphereScale = 0.025f;
    private static int numberofLevels = 5;
    private static float maxRadius = 0.3f;
    private static Vector3 maxleveGap = new Vector3(0, maxRadius, 0);
    private static Dictionary<int, GameObject[]> sphereOnLevel = new Dictionary<int, GameObject[]>();
    private static float startTime;
    private static int counter;

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time;
        counter = 0;
        createFieldSpheres();
        createFieldGridHorizontal();

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
        Vector3 fieldOrigin = Camera.main.transform.position + new Vector3(0.0f, 0.0f, 3.0f);
        

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
            for (int j = 0; j < numberOfSpheresOnLevel; j++)
            {
                horizontalLineRenderers[i].SetPosition(j,sphereOnLevel[i][j].transform.position);
            }
        }
    }

    private void createFielGridVertical()
    {

    }

    private Dictionary<int, LineRenderer> createHorizontalLineRenderers()
    {
        Dictionary<int, LineRenderer> horizontalLineRenderers = new Dictionary<int, LineRenderer>();

        for (int i = 0; i < numberofLevels; i++)
        {
            LineRenderer tmpLineRenderer = this.gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
            tmpLineRenderer.positionCount = numberOfSpheresOnLevel;
            tmpLineRenderer.startWidth = sphereScale;
            //add more LR-Attributes here

            horizontalLineRenderers.Add(i, tmpLineRenderer);
        }

        return horizontalLineRenderers;
    }
}
