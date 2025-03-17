using UnityEngine;

public class RegionCapturePoint : MonoBehaviour
{
    [Tooltip("Reference to the Region data for this capture point.")]
    public Region region;

    private void Awake()
    {
        // If the region isn't manually assigned in the Inspector,
        // automatically try to fetch it from the grandparent.
        if (region == null)
        {
            // Assuming the hierarchy: Region (parent) -> Centerpoint (child) -> CenterpointCollider (this GameObject)
            region = transform.parent.parent.GetComponent<Region>();
        }
    }

    // Called when another collider enters the trigger collider on this GameObject.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Assume your unit has a Unit component with a faction property.
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null)
        {
            // Check if the region is not already owned by this unit's faction.
            if (region.ownerID != unit.factionID)
            {
                // Update region ownership.
                region.ownerID = unit.factionID;
                Debug.Log($"Region {region.regionID} captured by faction {unit.factionID}");

                // Notify the GameManager so points can be awarded.
                GameManager.Instance.RegionCaptured(region, unit.factionID);
            }
        }
    }
}
