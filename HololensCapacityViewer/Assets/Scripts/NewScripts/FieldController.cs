using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{

    private Vector3 _fieldPointSize = new Vector3(0.01f, 0.01f, 0.01f);
    private int _iPointsToShow = 25;

    private List<GameObject> _fieldPoints;
    private GameObject _meanOrigin;
    private GameObject _sensorBoard;
    private SensorDataController _sensorData;

    private float _fCapacity1;
    private float _fCapacity2;

    float st;
    float at;

    // Use this for initialization
    void Start()
    {
        Util._fieldPointSize = _fieldPointSize;
        _sensorBoard = GameObject.Find("SensorBoard");
        _sensorData = GameObject.Find("Main").GetComponent<SensorDataController>();
        InitField();
        Util.UpdateVisibility(_fieldPoints);

        st = Time.realtimeSinceStartup;
        at = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        //at = Time.realtimeSinceStartup;
        //if (at - st > 0.1f)
        //{
            UpdateValues();
            UpdateFieldPoints();
        //    st = at;
        //}
    }

    private void InitField()
    {
        _fieldPoints = Util.GetFieldPointsFromLog();
        _fieldPoints = Util.ViconCsToUnityCs(_fieldPoints);
        _meanOrigin = Util.GetMeanOrigin(_fieldPoints);
        _meanOrigin = Util.ViconCsToUnityCs(_meanOrigin);
        Util.AttachParent(_meanOrigin, _fieldPoints);
        _meanOrigin.transform.position = _sensorBoard.transform.position;
        Util.AttachParent(_sensorBoard, _meanOrigin);
    }

    private void UpdateFieldPoints()
    {
        Util.SetAllToDisable(_fieldPoints);

        Util.UpdateCapacityVariance(_fieldPoints, _fCapacity1, _fCapacity2);
        Util.SetMostSuitableToEnable(_fieldPoints, _iPointsToShow);

        Util.UpdateVisibility(_fieldPoints);
    }

    private void UpdateValues()
    {
        _fCapacity1 = _sensorData._fCapacity1;
        _fCapacity2 = _sensorData._fCapacity2;
    }

}
