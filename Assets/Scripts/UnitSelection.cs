using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UnitSelection : MonoBehaviour
{
    public GameObject unitActionPanel;
    public TextMeshProUGUI unitNameText;
    public Button moveButton;  // ✅ Button for Move Mode

    private GameObject taskbarPanel;
    private static UnitSelection currentlySelectedUnit;

    void Start()
    {
        // Initially hide the Unit Action Panel.
        if (unitActionPanel != null)
            unitActionPanel.SetActive(false);

        // Find the Taskbar.
        taskbarPanel = GameObject.Find("Taskbar");

        // ✅ Add Move Mode Activation
        if (moveButton != null)
            moveButton.onClick.AddListener(ActivateMoveMode);
    }

    // Called by ClickManager.
    public void OnUnitSelected()
    {
        string unitID = gameObject.name;
        Debug.Log($"UnitSelection: OnUnitSelected called for: {unitID}");

        currentlySelectedUnit?.CloseUnitActionPanel();
        currentlySelectedUnit = this;

        // Delegate panel control to the UI Manager.
        MaproomUIManager uiManager = FindObjectOfType<MaproomUIManager>();
        if (uiManager != null)
        {
            uiManager.OpenUnitActionPanel(unitID);
            Debug.Log("UnitSelection: UnitActionPanel activated via UI Manager.");
        }
        else
        {
            Debug.LogError("UnitSelection: MaproomUIManager not found!");
        }

        if (taskbarPanel != null)
        {
            taskbarPanel.SetActive(false);
            Debug.Log("UnitSelection: Taskbar hidden.");
        }
        else
        {
            Debug.LogError("UnitSelection: Taskbar not found!");
        }

        if (unitNameText != null)
        {
            unitNameText.text = unitID;
            Debug.Log($"UnitSelection: UnitNameText set to {unitID}");
        }
        else
        {
            Debug.LogError("UnitSelection: unitNameText reference is missing!");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectUnit(gameObject);
            Debug.Log("UnitSelection: GameManager.SelectUnit called.");
        }
    }

    public void CloseUnitActionPanel()
    {
        Debug.Log("UnitSelection: Closing Unit Action Panel.");
        if (unitActionPanel != null)
            unitActionPanel.SetActive(false);
        if (taskbarPanel != null)
            taskbarPanel.SetActive(true);

        currentlySelectedUnit = null;
    }


    // ✅ Move Mode Trigger
    public void ActivateMoveMode()
    {
        Debug.Log("UnitSelection: Move button clicked - Attempting to activate Move Mode.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ActivateMoveMode();
            Debug.Log("UnitSelection: Move Mode activated in GameManager.");
        }
        else
        {
            Debug.LogError("UnitSelection: GameManager instance is NULL!");
        }
    }

}
