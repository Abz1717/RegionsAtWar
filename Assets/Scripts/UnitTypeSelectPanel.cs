using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitTypeSelectPanel : MonoBehaviour
{
    public event Action<UnitType> OnUnitSelected;

    [SerializeField] private UnitsConfiguration unitConfiguration;
    [SerializeField] private UnitTypeSelectView unitSelectPrefab;
    [SerializeField] private Transform buttonsParent;  

    private List<UnitTypeSelectView> unitTypes = new List<UnitTypeSelectView>();

    private void Awake()
    {
        foreach (var unit in unitConfiguration.unitTypes)
        {
            var view = Instantiate(unitSelectPrefab, buttonsParent);
            view.SetData(unit);
            view.OnClick += ProcessUnitSelected;
            unitTypes.Add(view);
        }
    }

    private void ProcessUnitSelected(UnitType unit)
    {
        OnUnitSelected?.Invoke(unit);
    }

    internal void Show(Region currentRegion)
    {
        gameObject.SetActive(true);
        if (currentRegion == null)
            return;
        var catapultButton = unitTypes.Find(unitType => unitType.Type == UnitType.Catapult);
        catapultButton.gameObject.SetActive(currentRegion!= null && currentRegion.buildings.Exists(building => building.type == BuildingType.Foundry));
    }
}