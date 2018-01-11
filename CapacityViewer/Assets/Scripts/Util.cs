using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Util
{
    public static Vector3 _fieldPointSize { get; set; }

    public static List<GameObject> GetFieldPointsFromLog()
    {
        List<GameObject> fieldPointList = new List<GameObject>();
        List<LogBox> logValues = new List<LogBox>();

        TextAsset pointLogAsset = Resources.Load<TextAsset>("PointLog");
        StreamReader logParser = new StreamReader(GenerateStreamFromString(pointLogAsset.text));

        logValues = GetValuesFromLog(logParser);

        Debug.Log(logValues[0].kugel);

        fieldPointList = CreateFieldPointObjects(logValues);

        InkObjects(fieldPointList);

        return fieldPointList;
    }

    public static GameObject GetMeanOrigin(List<GameObject> fieldPoints)
    {
        GameObject meanOrigin;

        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (var point in fieldPoints)
        {
            sum += point.GetComponent<FieldPointAttributes>()._originPosition;
            count++;
        }

        meanOrigin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        meanOrigin.name = "Origin";
        meanOrigin.transform.localScale = Util._fieldPointSize;
        meanOrigin.transform.position = sum / count;
        meanOrigin.GetComponent<Renderer>().material.color = Color.red;


        return meanOrigin;
    }

    public static Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static List<GameObject> ViconCsToUnityCs(List<GameObject> fieldPoints)
    {
        foreach (var point in fieldPoints)
        {
            point.transform.RotateAround(Vector3.zero, Vector3.right, -90);
        }

        return fieldPoints;
    }

    public static GameObject ViconCsToUnityCs(GameObject point)
    {
        point.transform.RotateAround(Vector3.zero, Vector3.right, -90);

        return point;
    }

    public static void AttachParent(GameObject parent, List<GameObject> children)
    {
        foreach (var child in children)
        {
            child.transform.parent = parent.transform;
        }
    }

    public static void AttachParent(GameObject parent, GameObject child)
    {
        child.transform.parent = parent.transform;
    }

    public static void UpdateVisibility(List<GameObject> gameObjects)
    {
        foreach (var obj in gameObjects)
        {
            if (obj.GetComponent<FieldPointAttributes>()._bToDisable)
                obj.SetActive(false);
            else if (!obj.GetComponent<FieldPointAttributes>()._bToDisable)
                obj.SetActive(true);
        }
    }

    public static void SetAllToDisable(List<GameObject> gameObjects)
    {
        foreach (var obj in gameObjects)
        {
            obj.GetComponent<FieldPointAttributes>()._bToDisable = true;
        }
    }

    public static void UpdateCapacityVariance(List<GameObject> gameObjects, float fCapacity1In, float fCapacity2In)
    {
        foreach (var obj in gameObjects)
        {
            float fCapacity1 = obj.GetComponent<FieldPointAttributes>()._fCapacity1;
            float fCapacity2 = obj.GetComponent<FieldPointAttributes>()._fCapacity2;

            obj.GetComponent<FieldPointAttributes>()._fCapacityVariance = Mathf.Abs(fCapacity1In - fCapacity1) + Mathf.Abs(fCapacity2In - fCapacity2);
        }
    }

    public static void SetMostSuitableToEnable(List<GameObject> gameObjects, int iPointsToShow)
    {
        Util.SortByCapacityVariance(gameObjects);

        for (int i = 0; i < iPointsToShow; i++)
        {
            gameObjects[i].GetComponent<FieldPointAttributes>()._bToDisable = false;
        }
    }

    public static void SortByDistance(List<GameObject> fieldpoints)
    {
        fieldpoints.Sort((x, y) => x.GetComponent<FieldPointAttributes>()._fDistanceToOrigin.CompareTo(y.GetComponent<FieldPointAttributes>()._fDistanceToOrigin));
    }

    public static void SortByCapacityVariance(List<GameObject> fieldpoints)
    {
        fieldpoints.Sort((x, y) => x.GetComponent<FieldPointAttributes>()._fCapacityVariance.CompareTo(y.GetComponent<FieldPointAttributes>()._fCapacityVariance));
    }

    public static void PrintEnabledDisabled(List<GameObject> fieldpoints)
    {
        int disabled = 0;
        int enabled = 0;
        foreach (var point in fieldpoints)
        {
            if (point.GetComponent<FieldPointAttributes>()._bToDisable)
                disabled++;
            else
                enabled++;
        }
        Debug.Log("enabled: " + enabled);
        Debug.Log("disabled: " + disabled);
    }

    public static float GetMeanCapacityVariance(List<GameObject> fieldpoints)
    {
        float mean = 0;
        int count = 0;

        foreach (var point in fieldpoints)
        {
            mean += point.GetComponent<FieldPointAttributes>()._fCapacityVariance;
            count++;
        }

        return mean / count;
    }

    private static List<GameObject> CreateFieldPointObjects(List<LogBox> logValues)
    {
        List<GameObject> fieldPointObjects = new List<GameObject>();

        foreach (var log in logValues)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.name = "FieldPoint";
            point.AddComponent<FieldPointAttributes>();

            point.transform.localScale = _fieldPointSize;
            point.transform.position = log.kugel;
            point.GetComponent<FieldPointAttributes>()._fCapacity1 = log.fCapacity_1;
            point.GetComponent<FieldPointAttributes>()._fCapacity2 = log.fCapacity_2;
            point.GetComponent<FieldPointAttributes>()._fDistanceToOrigin = log.fDistanceToOrigin;
            point.GetComponent<FieldPointAttributes>()._originPosition = log.sensorBoard;
            point.GetComponent<FieldPointAttributes>()._bToDisable = true;

            fieldPointObjects.Add(point);
        }
        return fieldPointObjects;
    }

    public static void InkObjects(List<GameObject> points)
    {
        Colors.CreateBorders(GetMinDistance(points), GetMaxDistance(points));

        foreach (var point in points)
        {
            float dist = point.GetComponent<FieldPointAttributes>()._fDistanceToOrigin;

            if(dist < Colors.Boarder1to2)
            {
                point.GetComponent<Renderer>().material.color = Colors.Area1;
            }
            else if (dist > Colors.Boarder1to2 && dist < Colors.Boarder2to3)
            {
                point.GetComponent<Renderer>().material.color = Colors.Area2;
            }
            else if (dist > Colors.Boarder2to3 && dist < Colors.Boarder3to4)
            {
                point.GetComponent<Renderer>().material.color = Colors.Area3;
            }
            else if (dist > Colors.Boarder3to4)
            {
                point.GetComponent<Renderer>().material.color = Colors.Area4;
            }
        }
    }

    public static float GetMaxDistance(List<GameObject> points)
    {
        float max = -10.0f;

        foreach (var point in points)
        {
            float dist = point.GetComponent<FieldPointAttributes>()._fDistanceToOrigin;
            if (dist > max)
                max = dist;

        }

        return max;
    }

    public static float GetMinDistance(List<GameObject> points)
    {
        float min = 1000000000.0f;

        foreach (var point in points)
        {
            float dist = point.GetComponent<FieldPointAttributes>()._fDistanceToOrigin;
            if (dist < min)
                min = dist;
        }

        return min;
    }

    private static List<LogBox> GetValuesFromLog(StreamReader logParser)
    {
        List<LogBox> logValues = new List<LogBox>();

        Vector3 kugel = Vector3.zero;
        Vector3 sensorBoard = Vector3.zero;
        Vector3 kugelOrientation = Vector3.zero;
        Vector3 sensorBoardOrientation = Vector3.zero;
        float distance = 0.0f;
        float capacity_1 = 0.0f;
        float capacity_2 = 0.0f;

        int iBlockLines = 8;
        int iActualLine = 1;

        while (!logParser.EndOfStream)
        {
            var line = logParser.ReadLine();

            var lineValues = line.Split('	');

            if (iActualLine == 4)
            {
                kugel.x = float.Parse(lineValues[0].Substring(21).Replace(',', '.'));
                kugel.y = float.Parse(lineValues[2].Substring(2).Replace(',', '.'));
                kugel.z = float.Parse(lineValues[3].Substring(2).Replace(',', '.'));
                kugelOrientation.x = float.Parse(lineValues[5].Substring(2).Replace(',', '.'));
                kugelOrientation.y = float.Parse(lineValues[6].Substring(2).Replace(',', '.'));
                kugelOrientation.z = float.Parse(lineValues[7].Substring(2).Replace(',', '.'));
            }
            else if (iActualLine == 5)
            {
                sensorBoard.x = float.Parse(lineValues[0].Substring(21).Replace(',', '.'));
                sensorBoard.y = float.Parse(lineValues[2].Substring(2).Replace(',', '.'));
                sensorBoard.z = float.Parse(lineValues[3].Substring(2).Replace(',', '.'));
                sensorBoardOrientation.x = float.Parse(lineValues[5].Substring(2).Replace(',', '.'));
                sensorBoardOrientation.y = float.Parse(lineValues[6].Substring(2).Replace(',', '.'));
                sensorBoardOrientation.z = float.Parse(lineValues[7].Substring(2).Replace(',', '.'));

            }
            else if (iActualLine == 6)
            {
                distance = float.Parse(lineValues[0].Substring(36).Replace(',', '.'));
            }
            else if (iActualLine == 8)
            {
                capacity_1 = float.Parse(lineValues[0].Substring(19).Replace(',', '.'));
                capacity_2 = float.Parse(lineValues[2].Substring(19).Replace(',', '.'));
            }

            if (iActualLine == iBlockLines)
            {
                logValues.Add(new LogBox(kugel, sensorBoard, distance, capacity_1, capacity_2));
            }

            iActualLine++;
            if (iActualLine > iBlockLines)
                iActualLine = 1;
        }
        return logValues;
    }

    public static class Colors
    {
        public static float Boarder1to2 { get; private set; }
        public static float Boarder2to3 { get; private set; }
        public static float Boarder3to4 { get; private set; }

        public readonly static Color Area1 = Color.red;
        public readonly static Color Area2 = Color.magenta;
        public readonly static Color Area3 = Color.blue;
        public readonly static Color Area4 = Color.cyan;

        public static void CreateBorders(float min, float max)
        {
            float quarter = (max - min) / 4;

            Boarder1to2 = min + quarter;
            Boarder2to3 = min + quarter * 2;
            Boarder3to4 = min + quarter * 3;
        }
    }
}
