using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitSelection : MonoBehaviour
{
    public GameObject unitActionPanel;
    public TextMeshProUGUI unitNameText;
    public Button moveButton;  // Assigned in the Inspector.

    private GameObject taskbarPanel;
    private static UnitSelection currentlySelectedUnit;

    private void Start()
    {
        if (unitActionPanel != null)
            unitActionPanel.SetActive(false);

        taskbarPanel = GameObject.Find("Taskbar");

        if (moveButton != null)
            moveButton.onClick.AddListener(ActivateMoveMode);
    }

    public void OnUnitSelected()
    {
        string unitID = gameObject.name;
        currentlySelectedUnit?.CloseUnitActionPanel();
        currentlySelectedUnit = this;

        // (Assuming you have a UI manager to handle the panel.)
        MaproomUIManager uiManager = FindObjectOfType<MaproomUIManager>();
        if (uiManager != null)
            uiManager.OpenUnitActionPanel(unitID);

        if (taskbarPanel != null)
            taskbarPanel.SetActive(false);

        if (unitNameText != null)
            unitNameText.text = unitID;

        if (GameManager.Instance != null)
            GameManager.Instance.SelectUnit(gameObject);
    }

    public void CloseUnitActionPanel()
    {
        if (unitActionPanel != null)
            unitActionPanel.SetActive(false);
        if (taskbarPanel != null)
            taskbarPanel.SetActive(true);
        currentlySelectedUnit = null;
    }

    // Called when the move button is pressed.
    public void ActivateMoveMode()
    {
        Debug.Log("Move button clicked - activating move mode.");
        if (GameManager.Instance != null)
            GameManager.Instance.ActivateMoveMode();
    }
}
