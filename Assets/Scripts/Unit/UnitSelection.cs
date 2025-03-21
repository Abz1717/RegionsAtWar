using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitSelection : MonoBehaviour
{
    public GameObject unitActionPanel;
    public TextMeshProUGUI unitNameText;
    public Button moveButton;  // Assigned in the Inspector

    public Unit unit;


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


    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }



    public void OnUnitSelected()
    {
        Debug.Log("OnUnitSelected: unit is " + (unit != null ? unit.gameObject.name : "NULL"));

        // Close previously open unit panel (if any)
        string unitID = gameObject.name;
        currentlySelectedUnit?.CloseUnitActionPanel();
        currentlySelectedUnit = this;

        // Get the UI manager
        MaproomUIManager uiManager = FindObjectOfType<MaproomUIManager>();
        if (uiManager != null)
            uiManager.OpenUnitActionPanel(unit);

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
