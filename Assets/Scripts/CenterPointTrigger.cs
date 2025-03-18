using UnityEngine;

public class CenterPointTrigger : MonoBehaviour
{
    // Optionally, you can set a tag for units in the inspector.
    public string unitTag = "Unit";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering object is a unit (by tag or by component).
        if (other.CompareTag(unitTag))
        {
            // Snap the unit to the centerpoint position.
            other.transform.position = transform.position;
            Debug.Log("Unit snapped to centerpoint: " + transform.position);

            // Optionally, signal the unit's movement script that it has reached its destination.
            UnitMovement movement = other.GetComponent<UnitMovement>();
            if (movement != null)
            {
                movement.OnReachedCenterPoint();
            }
        }
    }
}
