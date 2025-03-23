using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Configuration", menuName = "Configs/Units")]
public class UnitsConfiguration : ScriptableObject
{
    public List<UnitTypeData> unitTypes;
    public List<UnitData> units;
}
