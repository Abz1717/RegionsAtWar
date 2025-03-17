using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject NewGamePanel;
    public GameObject MyGamesPanel;
    public GameObject ArchivedPanel;

    public Button NewGameButton;
    public Button MyGamesButton;
    public Button ArchivedButton;

    private Color defaultColor = Color.white; // Default button color
    private Color selectedColor = new Color(0.7f, 0.7f, 0.7f); // Gray for selected

    void Start()
    {
        // Set default panel to New Game
        ShowNewGamePanel();

        // Assign button listeners
        NewGameButton.onClick.AddListener(ShowNewGamePanel);
        MyGamesButton.onClick.AddListener(ShowMyGamesPanel);
        ArchivedButton.onClick.AddListener(ShowArchivedPanel);
    }

    public void ShowNewGamePanel()
    {
        NewGamePanel.SetActive(true);
        MyGamesPanel.SetActive(false);
        ArchivedPanel.SetActive(false);
        UpdateButtonColors(NewGameButton);
    }

    public void ShowMyGamesPanel()
    {
        NewGamePanel.SetActive(false);
        MyGamesPanel.SetActive(true);
        ArchivedPanel.SetActive(false);
        UpdateButtonColors(MyGamesButton);
    }

    public void ShowArchivedPanel()
    {
        NewGamePanel.SetActive(false);
        MyGamesPanel.SetActive(false);
        ArchivedPanel.SetActive(true);
        UpdateButtonColors(ArchivedButton);
    }

    private void UpdateButtonColors(Button activeButton)
    {
        // Reset all buttons to default color
        NewGameButton.GetComponent<Image>().color = defaultColor;
        MyGamesButton.GetComponent<Image>().color = defaultColor;
        ArchivedButton.GetComponent<Image>().color = defaultColor;

        // Change active button color
        activeButton.GetComponent<Image>().color = selectedColor;
    }
}
