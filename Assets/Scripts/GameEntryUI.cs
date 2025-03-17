using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEntryUI : MonoBehaviour
{
    public Image thumbnailImage;
    public TextMeshProUGUI gameTitleText;
    public TextMeshProUGUI gameIDText;
    public TextMeshProUGUI playerCountText;
    public Button joinButton;

    private string gameID;
    private string gameTitle;
    private bool isMultiplayer;

    public void SetGameData(
        string gameTitle,
        string gameID,
        int maxPlayers,
        int currentPlayers = 0,
        Sprite gameImage = null,
        bool isMultiplayer = false
    )
    {
        Debug.Log($"🟢 [GameEntryUI.SetGameData] Setting data for gameID: {gameID}, isMultiplayer: {isMultiplayer}");

        this.gameTitle = gameTitle; // Keep track of the map name
        this.gameID = gameID;
        this.isMultiplayer = isMultiplayer;

        gameTitleText.text = gameTitle;
        gameIDText.text = $"#{gameID}";
        playerCountText.text = $"{currentPlayers}/{maxPlayers}";

        if (gameImage != null)
            thumbnailImage.sprite = gameImage;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(JoinGame);
    }

    private void JoinGame()
    {
        Debug.Log($"🟢 [GameEntryUI.JoinGame] Pressed Join on gameID: {gameID}, isMultiplayer: {isMultiplayer}");

        GameSessionManager gameSessionManager = FindObjectOfType<GameSessionManager>();
        if (gameSessionManager == null)
        {
            Debug.LogError("❌ [GameEntryUI.JoinGame] GameSessionManager NOT found in the scene!");
            return;
        }

        if (isMultiplayer)
        {
            Debug.Log("🟢 [GameEntryUI.JoinGame] Attempting JoinMultiplayerGame(...)");
            // For Multiplayer, we pass the 'map' or 'gameID' as originally intended
            // If your code expects a map name, pass `gameTitle`. If it expects the ID, pass `gameID`.
            // By default in your code, JoinMultiplayerGame expects a 'gameType' (a map name).
            // If you're actually storing "British Isles Multiplayer" in 'gameTitle', do:
            gameSessionManager.JoinMultiplayerGame(gameTitle);
        }
        else
        {
            Debug.Log("🟢 [GameEntryUI.JoinGame] Attempting JoinOrCreateSinglePlayerGame(...)");
            // Instead of just loading the scene, actually create/join it in Firebase:
            gameSessionManager.JoinOrCreateSinglePlayerGame(gameTitle);
        }
    }
}
