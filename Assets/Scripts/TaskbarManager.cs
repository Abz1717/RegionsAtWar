using UnityEngine;
using UnityEngine.UI;

public class TaskbarManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject HomePanel;
    public GameObject GamesPanel;
    public GameObject StatsPanel;
    public GameObject SettingsPanel;

    [Header("Taskbar Buttons")]
    public Button HomeButton;
    public Button GamesButton;
    public Button StatsButton;
    public Button SettingsButton;

    private Color defaultColor = Color.white;               // Default color for buttons
    private Color selectedColor = new Color(0.7f, 0.7f, 0.7f); // Gray for selected tab

    private void Start()
    {
        // Show Home panel by default
        ShowPanel(HomePanel, HomeButton);

        // Assign button listeners
        HomeButton.onClick.AddListener(() => ShowPanel(HomePanel, HomeButton));
        GamesButton.onClick.AddListener(() => ShowPanel(GamesPanel, GamesButton));
        StatsButton.onClick.AddListener(() => ShowPanel(StatsPanel, StatsButton));
        SettingsButton.onClick.AddListener(() => ShowPanel(SettingsPanel, SettingsButton));
    }

    private void ShowPanel(GameObject panelToShow, Button activeButton)
    {
        // Hide all main panels
        HomePanel.SetActive(false);
        GamesPanel.SetActive(false);
        StatsPanel.SetActive(false);
        SettingsPanel.SetActive(false);

        // Show the chosen panel
        panelToShow.SetActive(true);

        // Update button colors to reflect active tab
        UpdateButtonColors(activeButton);
    }

    private void UpdateButtonColors(Button activeButton)
    {
        HomeButton.GetComponent<Image>().color = defaultColor;
        GamesButton.GetComponent<Image>().color = defaultColor;
        StatsButton.GetComponent<Image>().color = defaultColor;
        SettingsButton.GetComponent<Image>().color = defaultColor;

        activeButton.GetComponent<Image>().color = selectedColor;
    }
}
