using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEntryUI : MonoBehaviour
{
    /*
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


        this.gameTitle = gameTitle; 
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
        GameSessionManager gameSessionManager = FindObjectOfType<GameSessionManager>();
        if (gameSessionManager == null)
        {

            return;
        }

        if (isMultiplayer)
        {
            gameSessionManager.JoinMultiplayerGame(gameTitle);
        }
        else

            gameSessionManager.JoinOrCreateSinglePlayerGame(gameTitle);
        }
    }
*/
}
