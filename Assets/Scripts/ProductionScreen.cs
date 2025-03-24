using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionScreen : MonoBehaviour, IUIPanel
{
    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private Button buildingButton;
    [SerializeField] private Button unitsButton;

    [SerializeField] private UnitSelectPanel unitsSelectPanel;
    [SerializeField] private UnitTypeSelectPanel unitsTypeSelectPanel;
    [SerializeField] private BuildingSelectPanel buildingSelectPanel;
    [SerializeField] private BuildingTypeSelectPanel buildingTypeSelectPanel;

    private Region currentRegion;

    private void Awake()
    {
        buildingButton.onClick.AddListener(ShowBuildingSelectPanel);
        unitsButton.onClick.AddListener(ShowUnitSelection);

        buildingSelectPanel.OnBuildingSelected += TryBuyBuilding;
        unitsSelectPanel.OnUnitSelected += TryBuyUnit;
        unitsTypeSelectPanel.OnUnitSelected += SelectUnitType;
        buildingTypeSelectPanel.OnBuildingSelected += SelectBuildingType;
    }

    private void SelectUnitType(UnitType type)
    {
        unitsSelectPanel.Show(type);
    }

    private void SelectBuildingType(BuildingType type)
    {
        buildingSelectPanel.Show(type);
    }

    private void TryBuyBuilding(BuildingData data)
    {
        Hide();
        GameManager.Instance.TryBuyBuilding(currentRegion, data);
    }

    private void TryBuyUnit(UnitData data)
    {
        Hide();
        GameManager.Instance.TryBuyUnit(currentRegion, data);
    }

    public void ShowBuildings(Region region)
    {
        title.text = "Construction";
        currentRegion = region;
        Show();
        ShowBuildingSelectPanel();
    }

    public void ShowUnits(Region region)
    {
        title.text = "Production";
        currentRegion = region;
        Show();
        ShowUnitSelection();
    }

    private void ShowBuildingSelectPanel()
    {
        buildingSelectPanel.Show(BuildingType.Barracks);
        unitsSelectPanel.gameObject.SetActive(false);
        unitsTypeSelectPanel.gameObject.SetActive(false);
        buildingTypeSelectPanel.gameObject.SetActive(true);
    }

    private void ShowUnitSelection()
    {
        buildingSelectPanel.gameObject.SetActive(false);
        unitsSelectPanel.Show(UnitType.Infantry);
        unitsTypeSelectPanel.Show(currentRegion);
        buildingTypeSelectPanel.gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);

        MaproomUIManager.Instance.mapPanel.SetActive(false);
        MaproomUIManager.Instance.taskbarPanel.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false );
        MaproomUIManager.Instance.mapPanel.SetActive(true);
        MaproomUIManager.Instance.taskbarPanel.SetActive(true);
    }
}
