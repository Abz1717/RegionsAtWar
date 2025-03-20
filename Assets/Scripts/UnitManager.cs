using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    // Singleton instance for easy global access
    public static UnitManager Instance;

    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Transform centerPoint; // Assign in the Inspector (unit spawn point)

    private UnitController unitController;
    private RegionManager regionManager;

    private void Awake()
    {
        // Set the static instance reference
        Instance = this;
    }

    private void Start()
    {
        // Instantiate the unit at the specified center point
        UnitController unitController = Instantiate(unitPrefab, centerPoint.position, Quaternion.identity);
        // Initialize the unit at the center point
        unitController.Initialize(centerPoint);

        // Find the RegionManager in the scene
        regionManager = FindObjectOfType<RegionManager>();
    }

    /// <summary>
    /// Called when we have a destination region (e.g., after the player clicks on a region).
    /// This method calculates a path from the unit’s current region to the destination,
    /// then commands the unit to move along that path using DOTween.
    /// </summary>
    public void OnMoveButtonPressed(RegionCapturePoint destinationRegion)
    {
        if (regionManager == null)
        {
            Debug.LogError("UnitManager: regionManager is NULL!");
            return;
        }
        if (destinationRegion == null)
        {
            Debug.LogError("UnitManager: destinationRegion is NULL!");
            return;
        }

        RegionCapturePoint startRegion = regionManager.GetCurrentRegion(unitController.transform.position);
        if (startRegion == null)
        {
            Debug.LogError("UnitManager: startRegion is NULL!");
            return;
        }

        List<RegionCapturePoint> path = regionManager.GetPath(startRegion, destinationRegion);
        unitController.MoveAlongPath(path);
    }

}
