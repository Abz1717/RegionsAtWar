using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    [Tooltip("Time in seconds between AI moves.")]
    public float moveInterval = 40f;

    private void Start()
    {
        StartCoroutine(AIMoveRoutine());
    }

    private IEnumerator AIMoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            // Get all AIUnitController components in the scene.
            AIUnitController[] aiUnitControllers = FindObjectsOfType<AIUnitController>();
            List<UnitController> idleAIUnits = new List<UnitController>();

            // Filter out units that are idle.
            foreach (AIUnitController aiCtrl in aiUnitControllers)
            {
                UnitController unit = aiCtrl.GetComponent<UnitController>();
                if (unit != null && unit.CurrentState == UnitController.UnitState.Idle)
                {
                    idleAIUnits.Add(unit);
                }
            }

            if (idleAIUnits.Count > 0)
            {
                int randomIndex = Random.Range(0, idleAIUnits.Count);
                UnitController chosenUnit = idleAIUnits[randomIndex];

                // Get a random destination from the available regions.
                RegionCapturePoint destination = GetRandomDestination();
                if (destination != null)
                {
                    Debug.Log($"[AIManager] Moving AI unit {chosenUnit.name} from region {chosenUnit.CurrentPoint.region.regionID} " +
                              $"to region {destination.region.regionID} at time {Time.time}.");
                    UnitManager.Instance.MoveUnit(chosenUnit, destination);
                }
                else
                {
                    Debug.LogWarning("[AIManager] Could not find a valid destination.");
                }
            }
            else
            {
                Debug.Log("[AIManager] No idle AI units available to move.");
            }
        }
    }

    /// <summary>
    /// Picks a random Region from the RegionManager's regionData and returns its capture point.
    /// </summary>
    private RegionCapturePoint GetRandomDestination()
    {
        List<Region> allRegions = RegionManager.Instance.regionData;
        if (allRegions == null || allRegions.Count == 0)
        {
            Debug.LogWarning("[AIManager] No regions found in regionData.");
            return null;
        }

        int randomIndex = Random.Range(0, allRegions.Count);
        Region randomRegion = allRegions[randomIndex];
        if (randomRegion == null || randomRegion.CapturePoint == null)
        {
            Debug.LogWarning("[AIManager] Randomly chosen region or its capture point is null.");
            return null;
        }

        return randomRegion.CapturePoint;
    }
}
