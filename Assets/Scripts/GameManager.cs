
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const float PERCENT_TO_WIN = 80f;
    public State CurrentState { get; private set; }

    private RegionCapturePoint destinationRegion;
    private UnitController selectedUnit;

    public UnitController SelectedUnit => selectedUnit;



    [SerializeField] public GameConfig gameConfig;
    private List<PlayerController> players = new List<PlayerController>();

    public ResourcesConfiguration resourcesConfiguration;



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
            players.Add(new PlayerController(player));
        }

        UnitManager.Instance.UpdateEnemyVisibilityAtStart();

    }

    private void EndGame()
    {
        RegionManager.Instance.Reset();
        UnitManager.Instance.Reset();

        players.Clear();

        StartGame();
    }

    public PlayerController GetPlayer(int id)
    {
        return players.Find(player => player.PlayerData.Id == id);
    }

    public void AddPlayerResource(int playerId, Resource resource, int ammount)
    {
        var playerToAdd = players.Find(player => player.PlayerData.Id == playerId);
        playerToAdd.PlayerModel.Resources[resource] += ammount;

        if (playerId == LocalPlayerId)
        {
            MaproomUIManager.Instance.UpdateResource(resource, playerToAdd.PlayerModel.Resources[resource]);
        }
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

    public void SelectUnit(UnitController unit)
    {
        Debug.Log("GameManager: Selected unit " + unit.name);
        selectedUnit = unit;
    }

    // Call this when the move button is pressed.
    public void ActivateMoveMode()
    {
        CurrentState = selectedUnit == null ? State.Idle : State.Move;
    }

    // Call this when the attack button is pressed.
    public void ActivateAttackMode()
    {
        CurrentState = selectedUnit == null ? State.Idle : State.Attack;
        Debug.Log("Attack mode activated. CurrentState: " + CurrentState);

    }

    public void DeactivateMoveMode()
    {
        CurrentState = State.Idle;
        Debug.Log("Move mode deactivated.");
    }


    // Called by RegionSelector when a region is clicked.
    public void SetDestination(RegionCapturePoint selectedRegion)
    {
        if (CurrentState != State.Move)
            return;

        if (selectedUnit == null)
        {
            Debug.LogWarning("No selected unit to move.");
            return;
        }


        destinationRegion = selectedRegion;
        Debug.Log($"Destination region set: {selectedRegion.name}");
        CurrentState = State.Idle;

        // Trigger movement in the UnitManager.
        //UnitManager.Instance.OnMoveButtonPressed(destinationRegion);
        UnitManager.Instance.OnMoveButtonPressed(selectedUnit, destinationRegion);
    }

    public void TryBuyUnit(Region currentRegion, UnitData data)
    {
        if (HasEnoughMoney(data.cost) && currentRegion.ownerID == LocalPlayerId)
        {
            UnitManager.Instance.SpawnUnit(RegionManager.Instance.regions.Find(region => region.region.regionID == currentRegion.regionID), LocalPlayerId, data);
            MakePurchase(LocalPlayerId, data.cost);
        }
    }

    public void TryBuyBuilding(Region currentRegion, BuildingData data)
    {
        if (HasEnoughMoney(data.cost) && currentRegion.ownerID == LocalPlayerId)
        {
            currentRegion.ConstructBuilding(data);
            MakePurchase(LocalPlayerId, data.cost);
        }
    }

    private void MakePurchase(int playerId, List<Cost> cost)
    {
        foreach (var costItem in cost)
        {
            AddPlayerResource(playerId, costItem.resource, -costItem.ammount);
        }
    }

    private bool HasEnoughMoney(List<Cost> costs)
    {
        var player = GetPlayer(LocalPlayerId);

        foreach (var cost in costs)
        {
            if (cost.ammount > player.PlayerModel.Resources[cost.resource])
                return false;
        }
        return true;
    }

    internal void AttackRegion(int attackedId, Region currentRegion)
    {
        var point = RegionManager.Instance.GetPoint(currentRegion);
        var unit = UnitManager.Instance.FindClosestUnit(point.transform.position, attackedId);
        if (unit != null)
        {
            UnitManager.Instance.MoveUnit(unit, point);
        }
    }

    public enum State
    {
        Idle,
        Move,
        Attack
    }


    public int LocalPlayerId
    {
        get
        {
            // Suppose you find the first PlayerData that is Type == Player.
            PlayerData localPlayer = gameConfig.Players.Find(p => p.Type == PlayerType.Player);
            if (localPlayer != null)
            {
                return localPlayer.Id;
            }

            // Fallback if not found, or you can throw an error.
            Debug.LogError("No local player found in GameConfig!");
            return -1;
        }
    }


    public void UpdateScoreUI()
    {
        foreach (var player in gameConfig.Players)
        {
            int regionsCount = RegionManager.Instance.regionData.FindAll(region => region.ownerID == player.Id).Count;
            // Update your taskbar UI for each player.
            
        }
    }
}
