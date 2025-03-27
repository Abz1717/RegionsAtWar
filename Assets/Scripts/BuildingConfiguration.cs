using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings Configuration", menuName = "Configs/Buildings")]
public class BuildingConfiguration : ScriptableObject
{
    public List<BuildingData> buildings;
    public List<BuildingTypeData> types;
    
}
