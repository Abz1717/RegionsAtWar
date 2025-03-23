using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public int id;
    public string name;
    public BuildingType type;
    public Sprite sprite;
    public List<Cost> cost;
    internal string duration;
    //public BuildingController prefab;

}

[Serializable]
public class BuildingTypeData
{
    public string name;
    public BuildingType type;
    public Sprite sprite;
}

public enum BuildingType
{
    Barracks,
    Fort,
    Foundry,
}

