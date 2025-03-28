using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIUnitController : MonoBehaviour
{
    private UnitController unitController;
    [Tooltip("Time (in seconds) between each AI move attempt.")]
    public float moveInterval = 40f;

    private void Start()
    {
        unitController = GetComponent<UnitController>();
        //StartCoroutine(RandomMoveRoutine());
    }

    private IEnumerator RandomMoveRoutine()
    {
        while (true)
        {
            // Wait the specified interval before deciding on a new move
            yield return new WaitForSeconds(moveInterval);

            // Only move if we're idle (not already moving or attacking)
            if (unitController.CurrentState != UnitController.UnitState.Idle)
                continue;

            // Choose a random RegionCapturePoint from the entire map
            RegionCapturePoint randomDestination = GetRandomDestination();
            if (randomDestination != null)
            {
                Debug.Log($"[AI] {unitController.name} is moving from region {unitController.CurrentPoint.region.regionID} " +
                          $"to region {randomDestination.region.regionID} at time {Time.time}.");

                // Reuse your existing UnitManager logic to move
                UnitManager.Instance.MoveUnit(unitController, randomDestination);
            }
            else
            {
                Debug.LogWarning($"[AI] {unitController.name} could not find a valid destination.");
            }
        }
    }

    /// <summary>
    /// Picks a random Region from your RegionManager's data, then returns its capture point.
    /// </summary>
    private RegionCapturePoint GetRandomDestination()
    {
        // If you're using regionData (List<Region>), do this:
        List<Region> allRegions = RegionManager.Instance.regionData;

        if (allRegions == null || allRegions.Count == 0)
        {
            Debug.LogWarning("No regions found in regionData.");
            return null;
        }

        // Pick a random region
        int randomIndex = Random.Range(0, allRegions.Count);
        Region randomRegion = allRegions[randomIndex];
        if (randomRegion == null || randomRegion.CapturePoint == null)
        {
            Debug.LogWarning("Randomly chosen region or its capture point is null.");
            return null;
        }

        return randomRegion.CapturePoint;
    }
}
