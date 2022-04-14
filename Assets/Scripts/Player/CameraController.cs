using UnityEngine;

public class CameraController : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("Movement and Rotation")]
    public float moveSpeed;
    public float rotationSpeed;

    [Space(10)]
    [Header("Zoom")]

    public float minFOV;
    public float maxFOV;
    public float zoomSpeed;
    public float zoomRotation;

    // ===== Private Variables ====================================================================

    private Camera cam;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        cam = Camera.main;
    }

    // ===== Update ===============================================================================
    
    private void Update ()
    {
        // Movement

        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);

        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);

        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.Self);

        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);

        // Rotation

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);

        // Reset camera

        if (Input.GetKeyDown(KeyCode.Z))
        {
            cam.fieldOfView = 60f;
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
            cam.transform.eulerAngles = new Vector3(45f, 0f, 0f);
        }

        // Zoom

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            float amnt = Input.GetAxis("Mouse ScrollWheel") * -1;

            // Make sure zoom keeps camera in good FOV range  
            if ((amnt < 0 && cam.fieldOfView > minFOV) || (amnt > 0 && cam.fieldOfView < maxFOV))
            {
                cam.fieldOfView += amnt * zoomSpeed;
                cam.transform.Rotate(Vector3.right * amnt * zoomRotation);
            }
        }
    }
}
