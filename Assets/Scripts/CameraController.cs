using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;  // Speed of camera movement
    public float zoomSpeed = 5f;  // Speed of zoom
    public float minZoom = 0.2f;  // Maximum zoom-in
    public float maxZoom = 21f;   // Maximum zoom-out

    public Vector2 minBounds; // Minimum X & Y boundaries
    public Vector2 maxBounds; // Maximum X & Y boundaries

    private Camera cam;
    private Vector3 dragOrigin; // Stores the initial click/touch position
    private int touchId = 1;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleZoom();
        HandlePanning();
        HandleDrag();  // <-- Added this for dragging support
    }

    void HandleZoom()
    {
        // Mouse Scroll Zoom (PC)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - scroll * zoomSpeed,
            minZoom, maxZoom
        );

        // Pinch Zoom (Mobile)
        if (Input.touchCount == 2 && (Input.touches[0].phase == TouchPhase.Stationary || Input.touches[0].phase == TouchPhase.Moved) && 
            (Input.touches[1].phase == TouchPhase.Stationary || Input.touches[1].phase == TouchPhase.Moved))
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = prevMagnitude - currentMagnitude;
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize + difference * zoomSpeed * 0.01f,
                minZoom, maxZoom
            );
        }
    }

    void HandlePanning()
    {
        Vector3 move = new Vector3();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move.y += panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move.y -= panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move.x -= panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move.x += panSpeed * Time.deltaTime;

        transform.position += move;

        // Keep camera within bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
            transform.position.z
        );
    }

    void HandleDrag()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) // Start dragging with left click
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0)) // Drag while holding left click
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference;

            // Keep camera within bounds
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
        }

#else
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began) // Start dragging with left click
        {
            touchId = Input.touches[0].fingerId;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Moved && Input.touches[0].fingerId == touchId) // Drag while holding left click
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference;

            // Keep camera within bounds
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
        }
#endif
    }
}
