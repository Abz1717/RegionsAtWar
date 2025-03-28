using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectPanel : MonoBehaviour
{
    public event Action<BuildingData> OnBuildingSelected;

    [SerializeField] private BuildingConfiguration buildingConfiguration;
    [SerializeField] private BuildingSelectView buildingSelectPrefab;
    [SerializeField] private Transform buttonsParent;

    private List<BuildingSelectView> buildings = new();

    private void Awake()
    {
        foreach (var building in buildingConfiguration.buildings)
        {
            var view = Instantiate(buildingSelectPrefab, buttonsParent);
            view.SetData(building);
            view.OnClick += ProcessBuildingSelected;
            buildings.Add(view);
        }
    }

    public void Show(BuildingType buildingType)
    {
        gameObject.SetActive(true);
        foreach (var building in buildings)
        {
            building.gameObject.SetActive(building.Type == buildingType);
        }
    }

    private void ProcessBuildingSelected(BuildingData data)
    {
        OnBuildingSelected?.Invoke(data);
    }
}
