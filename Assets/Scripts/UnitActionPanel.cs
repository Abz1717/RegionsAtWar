using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionPanel : MonoBehaviour, IUIPanel
{
    [SerializeField] private GameObject playerUnitPanel;
    [SerializeField] private GameObject enemyUnitPanel;

    [SerializeField] private Button playerMoveButton;
    [SerializeField] private Button playerAttackButton;
    [SerializeField] private Button playerAddArmyButton;
    [SerializeField] private Button attackEnemyButton;



    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image factionFlagImage;

    [Header("Stats Info")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Common Health Bar (Same for All Units)")]
    [SerializeField] private Image commonHealthBarImage;

    [SerializeField] private Sprite commonFullHealthSprite;
    [SerializeField] private Sprite commonSeventyFiveSprite;
    [SerializeField] private Sprite commonFiftySprite;
    [SerializeField] private Sprite commonTwentyFiveSprite;




    [Header("Unique Health Bar (Per Unit)")]
    [SerializeField] private Image uniqueHealthBarImage;



    private void Start()
    {
        playerMoveButton.onClick.AddListener(Move);
        playerAttackButton.onClick.AddListener(PlayerAttack);
        playerAddArmyButton.onClick.AddListener(AddArmy);
        attackEnemyButton.onClick.AddListener(AttackEnemy);
    }

    private void Move()
    {
        if (GameManager.Instance.SelectedUnit == null)
        {
            Debug.LogWarning("No unit selected for movement.");
            return;
        }
        GameManager.Instance.ActivateMoveMode();
    }

    private void PlayerAttack()
    {
        if (GameManager.Instance.SelectedUnit == null)
        {
            Debug.LogWarning("No unit selected for attack.");
            return;
        }
        GameManager.Instance.ActivateAttackMode();
    }

    private void AddArmy()
    {
        Hide();
        MaproomUIManager.Instance.OpenRegionProduction(GameManager.Instance.SelectedUnit.CurrentPoint.region);
    }

    private void AttackEnemy()
    {
        var unit = UnitManager.Instance.FindClosestUnit(GameManager.Instance.SelectedUnit.transform.position, GameManager.Instance.LocalPlayerId);
        UnitManager.Instance.MoveUnit(unit,GameManager.Instance.SelectedUnit.CurrentPoint);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        var unitController = GameManager.Instance.SelectedUnit;
        var data = unitController.Data;
        var unit = unitController.Unit;

        // Update the basic info:
        unitNameText.text = unitController.CustomName;


        // Update the stats:
        attackText.text = unit.attack.ToString();
        defenseText.text = unit.defense.ToString();
        speedText.text = unit.moveSpeed.ToString();


        // Update the flag image based on the selected unit's faction.
        PlayerData playerData = GameManager.Instance.gameConfig.Players.Find(p => p.Id == unitController.Unit.factionID);
        if (playerData != null && factionFlagImage != null)
        {
            factionFlagImage.sprite = playerData.flag;
        }

        // Calculate the health percentage:
        int healthPercentage = Mathf.RoundToInt((float)unit.CurrentHealth / unit.maxHealth * 100);
        


        // --- Update the Common Health Bar ---
        if (healthPercentage >= 100)
            commonHealthBarImage.sprite = commonFullHealthSprite;
        else if (healthPercentage >= 75)
            commonHealthBarImage.sprite = commonSeventyFiveSprite;
        else if (healthPercentage >= 50)
            commonHealthBarImage.sprite = commonFiftySprite;
        else
            commonHealthBarImage.sprite = commonTwentyFiveSprite;

        // --- Update the Unique Health Bar using the unit's own data ---
        if (healthPercentage >= 100)
            uniqueHealthBarImage.sprite = data.uniqueFullHealthSprite;
        else if (healthPercentage >= 75)
            uniqueHealthBarImage.sprite = data.uniqueSeventyFiveSprite;
        else if (healthPercentage >= 50)
            uniqueHealthBarImage.sprite = data.uniqueFiftySprite;
        else
            uniqueHealthBarImage.sprite = data.uniqueTwentyFiveSprite;
    }
}
