
/*

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Global Resources & Points")]
    // Base resource pools:
    public int playerResources = 0;
    public int aiResources = 0;
    public int playerPoints = 0;
    public int aiPoints = 0;
    public int pointsPerRegion = 1;
    public int pointsToWin = 45; // Win condition.

    [Header("Additional Resources (Player)")]
    public int money = 0;
    public int manpower = 0;
    public int resource1 = 0;
    public int resource2 = 0;
    public int resource3 = 0;

    [Header("Additional Resources (AI)")]
    public int aiMoney = 0;
    public int aiManpower = 0;
    public int aiResource1 = 0;
    public int aiResource2 = 0;
    public int aiResource3 = 0;

    [Header("UI References (Player)")]
    // Assign these in the Inspector with your Text components.
    public TextMeshProUGUI playerPointsText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI manpowerText;
    public TextMeshProUGUI resource1Text;
    public TextMeshProUGUI resource2Text;
    public TextMeshProUGUI resource3Text;

    [Header("Resource Production")]
    // Set your desired collection interval (currently set to 1 second; update as needed).
    public float resourceCollectionInterval = 1f;
    public List<Region> allRegions = new List<Region>();

    private float resourceTimer = 0f;

    [Header("Unit Selection & Movement")]
    public GameObject selectedUnit;

    [Header("Global State")]
    public bool unitClickedThisFrame = false;
    public bool isMoveModeActive = false;

    public bool IsMoveModeActive
    {
        get { return isMoveModeActive; }
    }

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

        UpdateUI();
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

    // Updated CollectResources to handle all production values.
    private void CollectResources()
    {
        foreach (Region region in allRegions)
        {
            if (region.ownerID == 1)
            {
                playerResources += region.resourceRate;
                money += region.moneyRate;
                manpower += region.manpowerRate;
                resource1 += region.resource1Rate;
                resource2 += region.resource2Rate;
                resource3 += region.resource3Rate;
            }
            else if (region.ownerID == 2)
            {
                aiResources += region.resourceRate;
                aiMoney += region.moneyRate;
                aiManpower += region.manpowerRate;
                aiResource1 += region.resource1Rate;
                aiResource2 += region.resource2Rate;
                aiResource3 += region.resource3Rate;
            }
        }
        Debug.Log($"GameManager: Player Resources: {playerResources} (Money: {money}, Manpower: {manpower}, R1: {resource1}, R2: {resource2}, R3: {resource3})");
        Debug.Log($"GameManager: AI Resources: {aiResources} (Money: {aiMoney}, Manpower: {aiManpower}, R1: {aiResource1}, R2: {aiResource2}, R3: {aiResource3})");
    }

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
        Road[] roads = FindObjectsOfType<Road>();
        foreach (Road road in roads)
            road.SetSelectable(true);
        Debug.Log("GameManager: Move mode activated.");
    }

    public void DeactivateMoveMode()
    {
        isMoveModeActive = false;
        Road[] roads = FindObjectsOfType<Road>();
        foreach (Road road in roads)
            road.SetSelectable(false);
        Debug.Log("GameManager: Move mode deactivated.");
    }

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

        // Find closest point on the road line from the unit.
        Vector3 nearestRoadPoint = NearestPointOnLine(startPos, endPos, selectedUnit.transform.position);
        UnitMovement movement = selectedUnit.GetComponent<UnitMovement>();
        if (movement != null)
            movement.MoveAlongRoad(nearestRoadPoint, destination);
    }

    public static Vector3 NearestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDir = (lineEnd - lineStart).normalized;
        float projectionLength = Vector3.Dot(point - lineStart, lineDir);
        projectionLength = Mathf.Clamp(projectionLength, 0, Vector3.Distance(lineStart, lineEnd));
        return lineStart + lineDir * projectionLength;
    }

    private void UpdateUI()
    {
        if (playerPointsText != null)
            playerPointsText.text = playerPoints.ToString();

        if (moneyText != null)
            moneyText.text = money.ToString();

        if (manpowerText != null)
            manpowerText.text = manpower.ToString();

        if (resource1Text != null)
            resource1Text.text = resource1.ToString();

        if (resource2Text != null)
            resource2Text.text = resource2.ToString();

        if (resource3Text != null)
            resource3Text.text = resource3.ToString();
    }
}

*/


using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const float PERCENT_TO_WIN = 80f;
    public bool IsMoveModeActive { get; private set; }

    private RegionCapturePoint destinationRegion;
    private UnitController selectedUnit;

    [SerializeField] public GameConfig gameConfig;



    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        var regions = RegionManager.Instance.regionData;
        // Set starting ownership based on GameConfig.
        foreach (var player in gameConfig.Players)
        {
            foreach (var region in player.StartRegions)
            {
                regions[region].SetOwner(player.Id);
            }
            UnitManager.Instance.SpawnPlayerUnits(player);
        }
    }

    private void EndGame()
    {
        RegionManager.Instance.Reset();
        UnitManager.Instance.Reset();

        StartGame();
    }

    public void CheckGameEnd()
    {
        foreach (var player in gameConfig.Players)
        {
            var regionsCount = RegionManager.Instance.regionData.FindAll(region => region.ownerID == player.Id).Count;
            if (regionsCount >= RegionManager.Instance.regionData.Count * PERCENT_TO_WIN / 100f)
            {
                EndGame();
            }
        }
    }

    public void SelectUnit(GameObject unit)
    {
        Debug.Log("GameManager: Selected unit " + unit.name);

        // Optionally store it in a variable
        // selectedUnit = unit;
        // Or do any logic you need when a unit is selected


        // Convert the clicked GameObject into a UnitController reference
        selectedUnit = unit.GetComponent<UnitController>();
        if (selectedUnit == null)
        {
            Debug.LogError("Selected GameObject has no UnitController component!");
        }
    }

    // Call this when the move button is pressed.
    public void ActivateMoveMode()
    {

        if (selectedUnit == null)
        {
            Debug.LogWarning("No unit selected.");
            return;
        }

        if (IsMoveModeActive)
        {
            Debug.Log("Move mode is already active; ignoring duplicate input.");
            return;
        }

        IsMoveModeActive = true;
        Debug.Log("Move mode activated. Click on a destination region.");
    }

    public void DeactivateMoveMode()
    {
        IsMoveModeActive = false;
        Debug.Log("Move mode deactivated.");
    }


    // Called by RegionSelector when a region is clicked.
    public void SetDestination(RegionCapturePoint selectedRegion)
    {
        if (!IsMoveModeActive)
            return;

        if (selectedUnit == null)
        {
            Debug.LogWarning("No selected unit to move.");
            return;
        }


        destinationRegion = selectedRegion;
        Debug.Log($"Destination region set: {selectedRegion.name}");
        IsMoveModeActive = false;

        // Trigger movement in the UnitManager.
        //UnitManager.Instance.OnMoveButtonPressed(destinationRegion);
        UnitManager.Instance.OnMoveButtonPressed(selectedUnit, destinationRegion);

    }
    public enum State
    {
        Idle,
        Move,
        Attack
    }
}
