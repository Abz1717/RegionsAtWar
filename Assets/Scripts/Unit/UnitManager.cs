using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Drawing;

public class UnitManager : Singleton<UnitManager>
{
    // Singleton instance for easy global access

    [SerializeField] private UnitController unitPrefab;

    //private UnitController unitController;
    private List<UnitController> spawnedUnits = new List<UnitController>();

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
    }

}
