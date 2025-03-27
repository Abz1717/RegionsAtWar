using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Unit Prefab")]
    // Drag your unit prefab here in the Inspector.
    public GameObject unitPrefab;

    [Header("Spawn Settings")]
    // Choose whether to spawn at a region center or along a road.
    public bool spawnAtRegionCenter = true;

    // If true, the unit will be spawned at the center of this region.
    public Region startingRegion;

    // If false, the unit will be spawned along a road.
    public Road startingRoad;
    [Range(0f, 1f)]
    // 0 = start of road, 1 = end of road, 0.5 = midpoint.
    public float fractionAlongRoad = 0.5f;

    private void Start()
    {
        Debug.Log("UnitSpawner: Start() called.");

        if (unitPrefab == null)
        {
            Debug.LogError("UnitSpawner: Unit prefab is not assigned!");
            return;
        }

        GameObject newUnit = null;

        if (spawnAtRegionCenter)
        {
            Debug.Log("UnitSpawner: Spawning at region center.");
            if (startingRegion != null)
            {
                // Use the centerPoint if assigned, otherwise fall back to the region's transform.
                Vector3 spawnPosition = (startingRegion.centerPoint != null)
                    ? startingRegion.centerPoint.position
                    : startingRegion.transform.position;
                Debug.Log("UnitSpawner: Calculated spawn position (region center): " + spawnPosition);
                newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                Debug.Log("UnitSpawner: Unit spawned at region center: " + spawnPosition);
            }
            else
            {
                Debug.LogError("UnitSpawner: Starting Region is not assigned!");
            }
        }
        else
        {
            Debug.Log("UnitSpawner: Spawning along road.");
            if (startingRoad != null)
            {
                // Get the start and end positions of the road.
                Vector3 startPos = (startingRoad.startRegion.centerPoint != null)
                    ? startingRoad.startRegion.centerPoint.position
                    : startingRoad.startRegion.transform.position;
                Vector3 endPos = (startingRoad.endRegion.centerPoint != null)
                    ? startingRoad.endRegion.centerPoint.position
                    : startingRoad.endRegion.transform.position;
                Debug.Log("UnitSpawner: Road start position: " + startPos);
                Debug.Log("UnitSpawner: Road end position: " + endPos);

                // Lerp between start and end to get a spawn position along the road.
                Vector3 spawnPosition = Vector3.Lerp(startPos, endPos, fractionAlongRoad);
                Debug.Log("UnitSpawner: Calculated spawn position (road): " + spawnPosition);
                newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                Debug.Log("UnitSpawner: Unit spawned on road at position: " + spawnPosition);
            }
            else
            {
                Debug.LogError("UnitSpawner: Starting Road is not assigned!");
            }
        }

        // Optional: If you need to adjust the unit's scale or other properties immediately after instantiation:
        if (newUnit != null)
        {
            Debug.Log("UnitSpawner: New unit successfully instantiated.");
            // Uncomment and adjust if needed:
            // newUnit.transform.localScale = new Vector3(1f, 1f, 1f); // Set to desired scale.
            // newUnit.GetComponent<UnitMovement>()?.Initialize(someParameters);
        }
        else
        {
            Debug.LogError("UnitSpawner: New unit was not instantiated.");
        }
    }
}
