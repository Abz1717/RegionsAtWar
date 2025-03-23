using System;
using UnityEngine;

public class UnitTypeSelectPanel : MonoBehaviour
{
    public event Action<UnitType> OnUnitSelected;

    [SerializeField] private UnitsConfiguration unitConfiguration;
    [SerializeField] private UnitTypeSelectView unitSelectPrefab;
    [SerializeField] private Transform buttonsParent;  //put GridLayoutGroup on parent

    private void Start()
    {
        foreach (var unit in unitConfiguration.unitTypes)
        {
            var view = Instantiate(unitSelectPrefab, buttonsParent);
            view.SetData(unit);
            view.OnClick += ProcessUnitSelected;
        }
    }

    private void ProcessUnitSelected(UnitType unit)
    {
        OnUnitSelected?.Invoke(unit);
    }
}