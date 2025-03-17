using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Resources & Points")]
    public int playerResources = 0;
    public int aiResources = 0;
    public int playerPoints = 0;
    public int aiPoints = 0;
    public int pointsPerRegion = 10;
    public int pointsToWin = 100; // Win condition.

    [Header("Resource Production")]
    public float resourceCollectionInterval = 1f;
    public List<Region> allRegions = new List<Region>();

    private float resourceTimer = 0f;

    [Header("Unit Selection & Movement")]
    public GameObject selectedUnit;

    [Header("Global State")]
    public bool unitClickedThisFrame = false;
    public bool isMoveModeActive = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        unitClickedThisFrame = false;
        resourceTimer += Time.deltaTime;
        if (resourceTimer >= resourceCollectionInterval)
        {
            resourceTimer = 0f;
            CollectResources();
        }
    }

    public void SelectUnit(GameObject unit)
    {
        selectedUnit = unit;
        Debug.Log($"GameManager: Unit {unit.name} selected.");
    }

    public void RegisterUnitClick()
    {
        unitClickedThisFrame = true;
        Debug.Log("GameManager: Unit clicked this frame - Preventing immediate closure.");
    }

    public void OnRegionSelected(Region region)
    {
        if (region != null)
            Debug.Log($"GameManager: Region {region.regionID} selected. Owner: {region.ownerID}");
    }

    private void CollectResources()
    {
        foreach (Region region in allRegions)
        {
            if (region.ownerID == 1)
                playerResources += region.resourceRate;
            else if (region.ownerID == 2)
                aiResources += region.resourceRate;
        }
        Debug.Log($"GameManager: Player Resources: {playerResources}, AI Resources: {aiResources}");
    }

    // Called when a region is captured by a unit.
    public void RegionCaptured(Region region, int factionID)
    {
        if (factionID == 1)
        {
            playerPoints += pointsPerRegion;
            Debug.Log($"GameManager: Player points: {playerPoints}");
            if (playerPoints >= pointsToWin)
                Debug.Log("Player Wins!");
        }
        else if (factionID == 2)
        {
            aiPoints += pointsPerRegion;
            Debug.Log($"GameManager: AI points: {aiPoints}");
            if (aiPoints >= pointsToWin)
                Debug.Log("AI Wins!");
        }
    }


    public void ActivateMoveMode()
    {
        if (selectedUnit == null)
        {
            Debug.LogWarning("No unit selected.");
            return;
        }

        isMoveModeActive = true;

        // ✅ Highlight roads for selection
        Road[] roads = FindObjectsOfType<Road>();
        foreach (Road road in roads)
            road.SetSelectable(true);

        Debug.Log("GameManager: Move mode activated.");
    }

    // ✅ Deactivate Move Mode when movement is done
    public void DeactivateMoveMode()
    {
        isMoveModeActive = false;

        // Remove highlights
        Road[] roads = FindObjectsOfType<Road>();
        foreach (Road road in roads)
            road.SetSelectable(false);

        Debug.Log("GameManager: Move mode deactivated.");
    }

    // Called by Road.cs when a road is clicked.
    public void MoveSelectedUnitViaRoad(Road road, float percentAlongRoad)
    {
        if (selectedUnit == null)
        {
            Debug.LogWarning("GameManager: No unit selected.");
            return;
        }

        Vector3 startPos = (road.startRegion.centerPoint != null) ? road.startRegion.centerPoint.position : road.startRegion.transform.position;
        Vector3 endPos = (road.endRegion.centerPoint != null) ? road.endRegion.centerPoint.position : road.endRegion.transform.position;
        Vector3 destination = Vector3.Lerp(startPos, endPos, percentAlongRoad);

        // Find closest point on road line from the unit.
        Vector3 nearestRoadPoint = NearestPointOnLine(startPos, endPos, selectedUnit.transform.position);

        UnitMovement movement = selectedUnit.GetComponent<UnitMovement>();
        if (movement != null)
            movement.MoveAlongRoad(nearestRoadPoint, destination);
    }

    // Helper method: calculate nearest point on a line.
    public static Vector3 NearestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDir = (lineEnd - lineStart).normalized;
        float projectionLength = Vector3.Dot(point - lineStart, lineDir);
        projectionLength = Mathf.Clamp(projectionLength, 0, Vector3.Distance(lineStart, lineEnd));
        return lineStart + lineDir * projectionLength;
    }

    
}
