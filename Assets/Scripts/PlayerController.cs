public class PlayerController
{
    public PlayerModel PlayerModel;
    public PlayerData PlayerData;

    public PlayerController(PlayerData data)
    {
        PlayerModel = new PlayerModel();
        PlayerData = data;
    }
}
