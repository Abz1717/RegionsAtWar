using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTypeSelectPanel : MonoBehaviour
{
    public event Action<BuildingType> OnBuildingSelected;

    [SerializeField] private BuildingConfiguration buildingConfiguration;
    [SerializeField] private BuildingTypeSelectView buildingSelectPrefab;
    [SerializeField] private Transform buttonsParent;  //put GridLayoutGroup on parent

    private void Start()
    {
        foreach (var building in buildingConfiguration.types)
        {
            var view = Instantiate(buildingSelectPrefab, buttonsParent);
            view.SetData(building);
            view.OnClick += ProcessBuildingSelected;
        }
    }

    private void ProcessBuildingSelected(BuildingType type)
    {
        OnBuildingSelected?.Invoke(type);
    }
}
