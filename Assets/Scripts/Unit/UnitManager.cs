using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UnitManager : MonoBehaviour
{
    // Singleton instance for easy global access
    public static UnitManager Instance;

    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private List<Transform> centerPoints; // Assign in the Inspector (unit spawn point)

    //private UnitController unitController;
    private List<UnitController> spawnedUnits = new List<UnitController>();

    private RegionManager regionManager;

    private void Awake()
    {
        // Set the static instance reference
        Instance = this;
    }

    private void Start()
    {
        /*
        // Capture the centerPoint’s position
        Vector3 spawnPos = centerPoint.position;

        // Override the Z if you want it fixed at 0 (or some other value)
        spawnPos.z = 0f;
        // Instantiate the unit at the specified center point
        unitController = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        // UnitController unitController = Instantiate(unitPrefab, centerPoint.position, Quaternion.identity);

        // Initialize the unit at the center point
        unitController.Initialize(centerPoint);

        // Find the RegionManager in the scene
        regionManager = FindObjectOfType<RegionManager>();

        */

        // Assign the RegionManager first:
        regionManager = FindObjectOfType<RegionManager>();

        // Spawn one unit for each center point
        foreach (Transform point in centerPoints)
        {
            Vector3 spawnPos = point.position;
            spawnPos.z = 0f; // Force Z to 0 if needed
            UnitController newUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
            // If your unit has an Initialize method that takes a Transform:
            newUnit.Initialize(point);

            spawnedUnits.Add(newUnit);

        }

    }


    /// <summary>
    /// Called when we have a destination region (e.g., after the player clicks on a region).
    /// This method calculates a path from the unit’s current region to the destination,
    /// then commands the unit to move along that path using DOTween.
    /// </summary>
    public void OnMoveButtonPressed(UnitController selectedUnit, RegionCapturePoint destinationRegion)
    {
        if (regionManager == null)
        {
            Debug.LogError("UnitManager: regionManager is NULL!");
            return;
        }
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
        RegionCapturePoint startRegion = regionManager.GetCurrentRegion(selectedUnit.transform.position);
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
        List<RegionCapturePoint> path = regionManager.GetPath(startRegion, destinationRegion);
        selectedUnit.MoveAlongPath(path);
    }

}
