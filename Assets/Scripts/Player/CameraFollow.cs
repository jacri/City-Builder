using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    public bool active;
    public Vector3 offset;
    public Transform target;

    // ===== Private Functions ====================================================================

    public void Follow ()
    {
        StartCoroutine(FollowLoop());
    }


    private IEnumerator FollowLoop ()
    {
        Debug.Log("Following");
        for (float f = 0f; f <= 1.0f; f += 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, f); 

            yield return new WaitForEndOfFrame();
        }
    }
}
