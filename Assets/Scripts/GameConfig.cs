using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Configuration", menuName = "Configs/Game")]
public class GameConfig : ScriptableObject
{
    public ResourcesConfiguration resourcesConfiguration;
    public List<PlayerData> Players;

}

[System.Serializable]
public class PlayerData
{
    public int Id;
    public PlayerType Type;
    public Sprite flag;
    public List<int> StartRegions;
    public List<int> SpawnPoints;
    public Color color;
}

public enum PlayerType
{
    Player,
    AI
}
