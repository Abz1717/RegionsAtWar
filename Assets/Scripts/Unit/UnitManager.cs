using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Drawing;
using System.Collections;


public class UnitManager : Singleton<UnitManager>
{
    // Singleton instance for easy global access

    [SerializeField] private UnitController unitPrefab;

    //private UnitController unitController;
    private List<UnitController> spawnedUnits = new List<UnitController>();

    //maps enemy units to the time until which they should remain visible
    private Dictionary<UnitController, float> forcedVisibility = new Dictionary<UnitController, float>();



    private void Update()
    {
        // For example, if you have a reference to the player's unit or a known player position,
        // call the UpdateEnemyUnitVisibility method every frame (or throttle it to every few frames).
        if (GameManager.Instance != null && GameManager.Instance.SelectedUnit != null)
        {
            UpdateEnemyUnitVisibility(GameManager.Instance.SelectedUnit.transform.position);
        }

    }



    public void SpawnPlayerUnits(PlayerData playerData)
    {
        foreach(var spawnRegion in playerData.SpawnPoints)
        {
            var region = RegionManager.Instance.regions[spawnRegion];
            SpawnUnit(region.transform, playerData.Id);
        }
    }

    public void SpawnUnit(Transform point, int faction)
    {
        Vector3 spawnPos = point.position;
        spawnPos.z = 0f; // Force Z to 0 if needed
        UnitController newUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        // If your unit has an Initialize method that takes a Transform:
        newUnit.Initialize(point);
        newUnit.SetFaction(faction);

        spawnedUnits.Add(newUnit);
    }

    public void Reset()
    {
        foreach (var spawnedUnit in spawnedUnits) 
        {
            Destroy(spawnedUnit.gameObject);
        }
        spawnedUnits.Clear();
    }


    /// <summary>
    /// Called when we have a destination region (e.g., after the player clicks on a region).
    /// This method calculates a path from the unit’s current region to the destination,
    /// then commands the unit to move along that path using DOTween.
    /// </summary>
    public void OnMoveButtonPressed(UnitController selectedUnit, RegionCapturePoint destinationRegion)
    {
        if (selectedUnit == null)
        {
            Debug.LogError("UnitManager: selectedUnit is NULL!");
            return;
        }

        if (destinationRegion == null)
        {
            Debug.LogError("UnitManager: destinationRegion is NULL!");
            return;
        }

        // Calculate the start region from the SELECTED UNIT's position
        RegionCapturePoint startRegion = RegionManager.Instance.GetCurrentRegion(selectedUnit.transform.position);
        if (startRegion == null)
        {
            Debug.LogError("UnitManager: startRegion is NULL!");
            return;
        }
        Debug.Log($"UnitManager: Start region is {startRegion.name} at position {startRegion.transform.position}");


        // Validate that the unit is not already in the destination region.
        if (startRegion == destinationRegion)
        {
            Debug.LogWarning("UnitManager: The unit is already in the destination region. Movement aborted.");
            return;
        }

        // Compute the path and move the selected Unit.
        List<RegionCapturePoint> path = RegionManager.Instance.GetPath(startRegion, destinationRegion);
        selectedUnit.MoveAlongPath(path);


        // After moving, update enemy visibility.
        // Here we assume the player's unit position is the basis for what is visible.
        UpdateEnemyUnitVisibility(selectedUnit.transform.position);
    }

    /// <summary>
    /// Updates enemy unit visibility based on the player's current region.
    /// Enemy units will only be visible if they're in the same region as the player
    /// or in one of that region's neighboring regions.
    /// </summary>
    public void UpdateEnemyUnitVisibility(Vector3 playerPosition)
    {
        // Get the player's current region.
        RegionCapturePoint playerRCP = RegionManager.Instance.GetCurrentRegion(playerPosition);
        if (playerRCP == null || playerRCP.region == null)
        {
            Debug.LogWarning("Player region is null in UpdateEnemyUnitVisibility");
            return;
        }

        // Create a set of visible regions: player's region plus its neighbors.
        HashSet<Region> visibleRegions = new HashSet<Region>();
        visibleRegions.Add(playerRCP.region);
        foreach (Region neighbor in playerRCP.region.neighbors)
        {
            visibleRegions.Add(neighbor);
        }

        Debug.Log("Player region: " + playerRCP.region.regionID);
        foreach (var vr in visibleRegions)
        {
            Debug.Log("Visible region: " + vr.regionID);
        }

        // Loop through all spawned units.
        foreach (UnitController unitController in spawnedUnits)
        {
            if (unitController == null)
                continue;

            // Only update enemy units (assuming enemy unit if faction differs from local player)
            if (unitController.Unit.factionID != GameManager.Instance.LocalPlayerId)
            {
                RegionCapturePoint enemyRCP = RegionManager.Instance.GetCurrentRegion(unitController.transform.position);
                string enemyRegion = enemyRCP != null && enemyRCP.region != null ? enemyRCP.region.regionID : "NULL";
                bool isVisible = enemyRCP != null && enemyRCP.region != null && visibleRegions.Contains(enemyRCP.region);

                // Check if this unit is forced visible.
                if (forcedVisibility.ContainsKey(unitController) && Time.time < forcedVisibility[unitController])
                {
                    isVisible = true;
                }
                // Clean up expired entries:
                if (forcedVisibility.ContainsKey(unitController) && Time.time >= forcedVisibility[unitController])
                {
                    forcedVisibility.Remove(unitController);
                }


                Debug.Log(unitController.name + " is in region " + enemyRegion + " isVisible: " + isVisible);

                // If your enemy units do not use SpriteRenderer, ensure you re-enable their actual Renderer component if applicable.
                Renderer[] renderers = unitController.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = isVisible;
                }

                // Enable the collider.
                Collider2D col = unitController.GetComponent<Collider2D>();
                if (col != null)
                {
                    col.enabled = isVisible;
                    Debug.Log("Collider on " + unitController.name + " set to: " + col.enabled);
                }
            }
        }
    }

    public void UpdateEnemyVisibilityAtStart()
    {
        // Create a set for visible regions.
        HashSet<Region> visibleRegions = new HashSet<Region>();

        // Loop through each human player (or PlayerData with Type == Player).
        foreach (PlayerData player in GameManager.Instance.gameConfig.Players)
        {
            if (player.Type == PlayerType.Player)
            {
                // For each starting region index in this player's data...
                foreach (int regionIndex in player.SpawnPoints)
                {
                    // Use your region manager data to get the region.
                    Region startingRegion = RegionManager.Instance.regionData[regionIndex];
                    if (startingRegion != null)
                    {
                        visibleRegions.Add(startingRegion);
                        // Add all neighboring regions as visible.
                        foreach (Region neighbor in startingRegion.neighbors)
                        {
                            visibleRegions.Add(neighbor);
                        }
                    }
                }
            }
        }

        // Now update enemy unit visibility based on this union.
        foreach (UnitController unitController in spawnedUnits)
        {
            // Skip if the unit has been destroyed.
            if (unitController == null)
                continue;

            // Check if this is an enemy unit.
            if (unitController.Unit.factionID != GameManager.Instance.LocalPlayerId)
            {
                // Determine which region the enemy unit is in.
                RegionCapturePoint enemyRCP = RegionManager.Instance.GetCurrentRegion(unitController.transform.position);
                bool isVisible = enemyRCP != null &&
                                 enemyRCP.region != null &&
                                 visibleRegions.Contains(enemyRCP.region);

                // If your enemy units do not use SpriteRenderer, ensure you re-enable their actual Renderer component if applicable.
                Renderer[] renderers = unitController.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = isVisible;
                }


                Collider2D col = unitController.GetComponent<Collider2D>();
                if (col != null)
                Debug.Log($"{unitController.name} isVisible: {isVisible}");
                col.enabled = isVisible;
            }
        }
    }

    public void ForceEnemyVisibility(UnitController enemy, float duration)
    {
        if (enemy == null) return;
        forcedVisibility[enemy] = Time.time + duration;
        StartCoroutine(RemoveVisibilityAfter(enemy, duration));
    }

    private IEnumerator RemoveVisibilityAfter(UnitController enemy, float duration)
    {
        yield return new WaitForSeconds(duration);
        // Once 60s pass, re-check visibility so it removes the forced entry
        if (GameManager.Instance != null && GameManager.Instance.SelectedUnit != null)
        {
            UpdateEnemyUnitVisibility(GameManager.Instance.SelectedUnit.transform.position);
        }
    }




}
