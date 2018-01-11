using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPointAttributes : MonoBehaviour {
    public float _fCapacity1 { get; set; }
    public float _fCapacity2 { get; set; }
    public float _fDistanceToOrigin { get; set; }
    public Vector3 _originPosition { get; set; }
    public bool _bToDisable { get; set; }
    public float _fCapacityVariance { get; set; }
}
