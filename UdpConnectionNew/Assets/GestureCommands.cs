using UnityEngine;

public class GestureCommands : MonoBehaviour
{
    private static bool isRendered = true;
    // Called by GestureRecognizer when the user performs a Select gesture
    void OnSelect()
    {
        Debug.Log("Tap Message recieved");
        MeshRenderer Sphere1 = GameObject.Find("Sphere1").GetComponent<MeshRenderer>();
        if (isRendered)
        {
            Debug.Log("Sphere Out");
            isRendered = false;
            Sphere1.enabled = false;
        }
        else
        {
            Debug.Log("Sphere In");
            isRendered = true;
            Sphere1.enabled = true;
        }
    }
}