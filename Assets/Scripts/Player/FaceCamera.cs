using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // ===== Private Variables ====================================================================

    private Transform cam;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        cam = Camera.main.transform;
    }

    // ===== Update ===============================================================================
    
    private void FixedUpdate ()
    {
        transform.LookAt(cam);
    }
}
