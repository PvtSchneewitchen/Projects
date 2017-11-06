using Unity.Partial.System;
using UnityEngine;
using UnityEngine.XR.WSA;
using Type = System.Type;

public class WorldCursor : MonoBehaviour
{
    private MeshRenderer CursorMeshObjectHit;
    private MeshRenderer CursorMeshSpatialMapHit;
    private MeshRenderer CursorMeshNoneHit;
    public Type Object3D;
    public Type SpatialMap;

    // Use this for initialization
    void Start()
    {
        Debug.Log("INIT WORLDCURSOR");
        CursorMeshObjectHit = GameObject.Find("CursorMeshObjectHit").GetComponent<MeshRenderer>();
        CursorMeshSpatialMapHit = GameObject.Find("CursorMeshSpatialMapHit").GetComponent<MeshRenderer>();
        CursorMeshNoneHit = GameObject.Find("CursorMeshNoneHit").GetComponent<MeshRenderer>();
        CursorMeshObjectHit.enabled = false;
        CursorMeshSpatialMapHit.enabled = false;
        CursorMeshNoneHit.enabled = true;
        Object3D = GameObject.Find("Sphere").GetType();
        SpatialMap = GameObject.Find("Spatial Mapping").GetType();
    }

    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            Debug.Log("Type " + hitInfo.collider.GetType());
            Debug.Log("Type " + Object3D);
            Debug.Log("Type " + SpatialMap);
            if (hitInfo.collider.GetType() == typeof(SphereCollider))
            {
                CursorMeshObjectHit.enabled = true;
                CursorMeshSpatialMapHit.enabled = false;
                CursorMeshNoneHit.enabled = false; 
            }
            else if (hitInfo.collider.GetType() == typeof(MeshCollider))
            {
                CursorMeshObjectHit.enabled = false;
                CursorMeshSpatialMapHit.enabled = true;
                CursorMeshNoneHit.enabled = false;
            }
            else
            {
                CursorMeshObjectHit.enabled = false;
                CursorMeshSpatialMapHit.enabled = false;
                CursorMeshNoneHit.enabled = true;
            }

            this.transform.position = hitInfo.point;
            this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        else
        {
            // If the raycast did not hit a hologram, hide the cursor mesh.
            CursorMeshObjectHit.enabled = false;
            CursorMeshSpatialMapHit.enabled = false;
            CursorMeshNoneHit.enabled = true;

            // Move the cursor to the point where the raycast hit.
            this.transform.position = headPosition + gazeDirection;

            this.transform.rotation = Quaternion.FromToRotation(headPosition, gazeDirection);
        }
    }
}