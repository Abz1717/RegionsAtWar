using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectPanel : MonoBehaviour
{
    public event Action<UnitData> OnUnitSelected;

    [SerializeField] private UnitsConfiguration unitConfiguration;
    [SerializeField] private UnitSelectView unitSelectPrefab;
    [SerializeField] private Transform buttonsParent;  //put GridLayoutGroup on parent

    private List<UnitSelectView> units = new List<UnitSelectView>();

    private void Awake()
    {
        foreach (var unit in unitConfiguration.units)
        {
            var view = Instantiate(unitSelectPrefab, buttonsParent);
            view.SetData(unit);
            view.OnClick += ProcessUnitSelected;
            units.Add(view);
        }
    }

    public void Show(UnitType unitType)
    {
        gameObject.SetActive(true);
        foreach (var unit in units)
        {
            unit.gameObject.SetActive(unit.UnitType == unitType);
        }
    }

    private void ProcessUnitSelected(UnitData unit)
    {
        OnUnitSelected?.Invoke(unit);
    }
}
