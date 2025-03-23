using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitData
{
    public int id;
    public string name;
    public UnitType type;
    public Sprite sprite; // Base sprite

    // These must match what you're referencing in code:
    public Sprite uniqueFullHealthSprite;
    public Sprite uniqueSeventyFiveSprite;
    public Sprite uniqueFiftySprite;
    public Sprite uniqueTwentyFiveSprite;


    public UnitController prefab;
    public List<Cost> cost;
    internal string duration;
}

[Serializable]
public class UnitTypeData
{
    public string name;
    public UnitType type;
    public Sprite sprite;
}

public enum UnitType
{
    Infantry,
    Cavalry,
    Catapult,
    Archer,
}
