using UnityEngine;

public class Road : MonoBehaviour
{
    [Header("Road Settings")]
    [Tooltip("The starting region of this road.")]
    public Region startRegion;

    [Tooltip("The ending region of this road.")]
    public Region endRegion;

    [Tooltip("Determines if this road is selectable (e.g., when move mode is active).")]
    public bool isSelectable = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }


    // Called when the road is clicked (2D collider based)
    private void OnMouseDown()
    {
        if (!isSelectable)
            return;

        // For 2D, the OnMouseDown works with 2D colliders.
        // You can calculate the clicked position along the road if needed.
        Vector3 clickedPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickedPoint.z = 0f;

        // Calculate the percent along the road from startRegion to endRegion.
        float percentAlongRoad = CalculatePercentAlongRoad(clickedPoint);

        Debug.Log($"Road clicked at {percentAlongRoad * 100f:F1}% along the road.");

        // Here, you can call your movement logic from your GameManager or UnitMovement script.
        GameManager.Instance.MoveSelectedUnitViaRoad(this, percentAlongRoad);
    }

    // ✅ Enable or disable selection
    public void SetSelectable(bool state)
    {
        isSelectable = state;
        if (spriteRenderer != null)
            spriteRenderer.color = state ? Color.yellow : originalColor;
    }

    /// <summary>
    /// Projects the click position onto the line defined by the road endpoints.
    /// </summary>
    private float CalculatePercentAlongRoad(Vector3 clickPoint)
    {
        if (startRegion == null || endRegion == null)
            return 0f;

        // Use the centerpoint if it exists; otherwise, fallback to the region's transform position.
        Vector3 startPos = (startRegion.centerPoint != null) ? startRegion.centerPoint.position : startRegion.transform.position;
        Vector3 endPos = (endRegion.centerPoint != null) ? endRegion.centerPoint.position : endRegion.transform.position;
        Vector3 roadVector = endPos - startPos;
        float roadLength = roadVector.magnitude;

        Vector3 startToClick = clickPoint - startPos;
        float projection = Vector3.Dot(startToClick, roadVector.normalized);
        projection = Mathf.Clamp(projection, 0, roadLength);

        return projection / roadLength;
    }


    // Optional: Visualize the road in the Scene view.
    private void OnDrawGizmos()
    {
        if (startRegion != null && endRegion != null)
        {
            Vector3 startPos = (startRegion.centerPoint != null) ? startRegion.centerPoint.position : startRegion.transform.position;
            Vector3 endPos = (endRegion.centerPoint != null) ? endRegion.centerPoint.position : endRegion.transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startRegion.transform.position, endRegion.transform.position);
        }
    }
}
