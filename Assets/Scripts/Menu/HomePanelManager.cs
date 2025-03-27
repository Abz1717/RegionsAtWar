using UnityEngine;
using UnityEngine.UI;

public class HomePanelManager : MonoBehaviour
{
    [Header("Home Sub-Panels")]
    public GameObject ReleaseNotesPanel;
    public GameObject GameTipsPanel;
    public GameObject GameChatPanel;
    public GameObject JoinGamePanel;
    public GamesMenu GamesMenu;


    [Header("Sub-Panel Buttons (Inside Home)")]
    public Button ReleaseNotesButton;
    public Button TipsButton;
    public Button ChatButton;
    public Button JoinGameButton;

    private void Start()
    {

        // Assign event listeners for sub-panel buttons
        ReleaseNotesButton.onClick.AddListener(ShowReleaseNotesPanel);
        TipsButton.onClick.AddListener(ShowTipsPanel);
        ChatButton.onClick.AddListener(ShowChatPanel);
        JoinGameButton.onClick.AddListener(ShowGamesPanel);

    }

    public void ShowReleaseNotesPanel()
    {
        ReleaseNotesPanel.SetActive(true);
        GameTipsPanel.SetActive(false);
        GameChatPanel.SetActive(false);
    }

    public void ShowTipsPanel()
    {
        ReleaseNotesPanel.SetActive(false);
        GameTipsPanel.SetActive(true);
        GameChatPanel.SetActive(false);
    }

    public void ShowChatPanel()
    {
        ReleaseNotesPanel.SetActive(false);
        GameTipsPanel.SetActive(false);
        GameChatPanel.SetActive(true);
    }


    public void ShowGamesPanel()
    {
        GamesMenu.gameObject.SetActive(true);
    }
}
