using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Drawing;
using System.Collections;
using System.Linq;


public class UnitManager : Singleton<UnitManager>
{
    // Singleton instance for easy global access

    [SerializeField] private UnitsConfiguration config;

    //private UnitController unitController;
    private List<UnitController> spawnedUnits = new List<UnitController>();

    private Dictionary<UnitType, int> unitCount = new Dictionary<UnitType, int>();


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
            SpawnUnit(region, playerData.Id, config.units[0]);
        }
    }

    public void SpawnUnit(RegionCapturePoint point, int faction, UnitData data)
    {
        Vector3 spawnPos = point.transform.position;
        spawnPos.z = 0f; // Force Z to 0 if needed
        UnitController newUnit = Instantiate(data.prefab, spawnPos, Quaternion.identity, transform);
        // If your unit has an Initialize method that takes a Transform:
        newUnit.Initialize(point,data);
        newUnit.SetFaction(faction);


        // Attach AIUnitController if this faction is AI-controlled.
        if (IsAIFaction(faction))
        {
            if (newUnit.GetComponent<AIUnitController>() == null)
            {
                newUnit.gameObject.AddComponent<AIUnitController>();
            }
        }

        // Update the counter for this unit type.
        if (!unitCount.ContainsKey(data.type))
        {
            unitCount[data.type] = 0;
        }
        unitCount[data.type]++;
        int count = unitCount[data.type];

        // Build the unit's name using ordinal conversion.
        // For example, if the unit type is Archer, it becomes "1st Archery Regiment", "2nd Archery Regiment", etc.
        string ordinal = GetOrdinal(count);
        string unitName = "";

        switch (data.type)
        {
            case UnitType.Archer:
                unitName = ordinal + " Archery Regiment";
                break;
            case UnitType.Cavalry:
                unitName = ordinal + " Cavalry Squadron";
                break;
            case UnitType.Catapult:
                unitName = ordinal + " Catapult Battalion";
                break;
            case UnitType.Infantry:  
                unitName = ordinal + " Order of Knights";
                break;
            default:
                unitName = data.name;
                break;
        }

        newUnit.gameObject.name = unitName;
        newUnit.SetCustomName(unitName);

        spawnedUnits.Add(newUnit);
    }


    private bool IsAIFaction(int faction)
    {
        // Replace with your logic. For example, if local player's faction is not AI:
        return faction != GameManager.Instance.LocalPlayerId;
    }
    public UnitController FindClosestUnit(Vector3 position, int factionId)
    {
        var factionUnits = GetFactionUnits(factionId);
        if (factionUnits.Count == 0)
            return null;
        var closestUnit = factionUnits[0];
        var minDistance = float.MaxValue;
        foreach (var unit in factionUnits)
        {
            var distance = (position - unit.transform.position).sqrMagnitude;
            if (minDistance > distance)
            {
                closestUnit = unit;
                minDistance = distance;
            }
        }

        return closestUnit;
    }

    public List<UnitController> GetFactionUnits(int factionId)
    {
        return spawnedUnits.FindAll(unit => unit != null && unit.Unit.factionID == factionId).ToList();
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
    /// This method calculates a path from the unit�s current region to the destination,
    /// then commands the unit to move along that path using DOTween.
    /// </summary>
    public void OnMoveButtonPressed(UnitController selectedUnit, RegionCapturePoint destinationRegion)
    {
        if (selectedUnit == null)
        {
            Debug.LogError("UnitManager: selectedUnit is NULL!");
            return;
        }

        MoveUnit(selectedUnit, destinationRegion);

        // After moving, update enemy visibility.
        // Here we assume the player's unit position is the basis for what is visible.
        UpdateEnemyUnitVisibility(selectedUnit.transform.position);
    }

    public void MoveUnit(UnitController unit, RegionCapturePoint destinationRegion)
    {

        if (destinationRegion == null)
        {
            Debug.LogError("UnitManager: destinationRegion is NULL!");
            return;
        }

        // Calculate the start region from the SELECTED UNIT's position
        RegionCapturePoint startRegion = RegionManager.Instance.GetCurrentRegion(unit.transform.position);
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
        unit.MoveAlongPath(path);
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


    private string GetOrdinal(int number)
    {
        if (number <= 0) return number.ToString();

        int lastDigit = number % 10;
        int lastTwoDigits = number % 100;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
        {
            return number + "th";
        }
        switch (lastDigit)
        {
            case 1:
                return number + "st";
            case 2:
                return number + "nd";
            case 3:
                return number + "rd";
            default:
                return number + "th";
        }
    }





}
