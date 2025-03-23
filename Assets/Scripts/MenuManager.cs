using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject HomePanel;
    public GameObject GamesPanel;
    public GameObject StatsPanel;
    public GameObject SettingsPanel;
    public GameObject ReleaseNotesPanel;
    public GameObject GameTipsPanel;
    public GameObject GameChatPanel; 
    public GameObject JoinGamePanel;


    [Header("Buttons")]
    public Button HomeButton;
    public Button GamesButton;
    public Button StatsButton;
    public Button SettingsButton;

    // NEW: Close (X) buttons for the sub-panels
    [Header("Close Buttons on Sub-Panels")]
    public Button CloseReleaseNotesButton;
    public Button CloseGameTipsButton;
    public Button CloseGameChatButton;

    private Color defaultColor = Color.white;
    private Color selectedColor = new Color(0.7f, 0.7f, 0.7f);

    private void Start()
    {
        // Ensure only the HomePanel is visible at the start
        ShowPanel(HomePanel, HomeButton);

        // Assign button listeners for main tabs
        HomeButton.onClick.AddListener(() => ShowPanel(HomePanel, HomeButton));
        GamesButton.onClick.AddListener(() => ShowPanel(GamesPanel, GamesButton));
        StatsButton.onClick.AddListener(() => ShowPanel(StatsPanel, StatsButton));
        SettingsButton.onClick.AddListener(() => ShowPanel(SettingsPanel, SettingsButton));

        // NEW: Assign close button listeners in each sub-panel
        CloseReleaseNotesButton.onClick.AddListener(() => ShowPanel(HomePanel, HomeButton));
        CloseGameTipsButton.onClick.AddListener(() => ShowPanel(HomePanel, HomeButton));
        CloseGameChatButton.onClick.AddListener(() => ShowPanel(HomePanel, HomeButton));
    }

    public void ShowPanel(GameObject panelToShow, Button activeButton)
    {
        // Hide all panels before showing the selected one
        HomePanel.SetActive(false);
        GamesPanel.SetActive(false);
        StatsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ReleaseNotesPanel.SetActive(false);
        GameTipsPanel.SetActive(false);
        GameChatPanel.SetActive(false);
        JoinGamePanel.SetActive(false); 


        // Show only the selected panel
        panelToShow.SetActive(true);

        // Update button colors
        UpdateButtonColors(activeButton);
    }

    private void UpdateButtonColors(Button activeButton)
    {
        // Reset all buttons to default color
        HomeButton.GetComponent<Image>().color = defaultColor;
        GamesButton.GetComponent<Image>().color = defaultColor;
        StatsButton.GetComponent<Image>().color = defaultColor;
        SettingsButton.GetComponent<Image>().color = defaultColor;

        // Highlight active button
        activeButton.GetComponent<Image>().color = selectedColor;
    }
}
