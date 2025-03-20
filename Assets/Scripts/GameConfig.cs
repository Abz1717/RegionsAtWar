using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Configuration", menuName = "Configs")]
public class GameConfig : ScriptableObject
{
    public List<PlayerData> Players;
}

[System.Serializable]
public class PlayerData
{
    public int Id;
    public PlayerType Type;
    public List<int> StartRegions;
    public List<int> SpawnPoints;
    public Color color;
}

public enum PlayerType
{
    Player,
    AI
}
