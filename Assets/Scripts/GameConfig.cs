using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Configuration", menuName = "Configs/Game")]
public class GameConfig : ScriptableObject
{
    public List<PlayerData> Players;

    public int dayLengthInMinutes = 5;

}

[System.Serializable]
public class PlayerData
{
    public int Id;
    public PlayerType Type;
    public Sprite flag;
    public List<int> StartRegions;
    public List<int> SpawnPoints;
    public PlayerColor color;
}

public enum PlayerType
{
    Player,
    AI
}
