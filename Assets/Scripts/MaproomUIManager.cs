using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaproomUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject regionActionPanel;
    public GameObject taskbarPanel;
    public GameObject mapPanel;

    [Header("Region Action Sub-Panels")]
    public GameObject negotiatePanel;
    public GameObject producePanel;
    public GameObject provincePanel;
    public GameObject marketPanel;

    [Header("Taskbar Buttons (for Region Action Panel)")]
    public Button negotiateButton;
    public Button produceButton;
    public Button provinceButton;
    public Button marketButton;

    [Header("Region Info (Optional)")]
    public TextMeshProUGUI regionNameText;

    [Header("Unit Action Panel")]
    public GameObject unitActionPanel;
    public TextMeshProUGUI unitNameText;

    private bool allowClosing = false; // Controls outside-click closure.

    private void Start()
    {
        // Hide all panels initially.
        regionActionPanel?.SetActive(false);
        negotiatePanel?.SetActive(false);
        producePanel?.SetActive(false);
        provincePanel?.SetActive(false);
        marketPanel?.SetActive(false);
        unitActionPanel?.SetActive(false);

        taskbarPanel?.SetActive(true);
        mapPanel?.SetActive(true);

        // Assign button listeners.
        negotiateButton?.onClick.AddListener(() => ShowSubPanel(negotiatePanel, negotiateButton));
        produceButton?.onClick.AddListener(() => ShowSubPanel(producePanel, produceButton));
        provinceButton?.onClick.AddListener(() => ShowSubPanel(provincePanel, provinceButton));
        marketButton?.onClick.AddListener(() => ShowSubPanel(marketPanel, marketButton));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Delay the check to prevent immediate closure.
            Invoke(nameof(CheckOutsideClick), 0.1f);
        }
    }

    private void CheckOutsideClick()
    {
        if (!allowClosing) return;

        // Close region action panel if the click is outside.
        if (regionActionPanel != null && regionActionPanel.activeSelf)
        {
            RectTransform rt = regionActionPanel.GetComponent<RectTransform>();
            if (rt != null && !RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, null))
            {
                Debug.Log("MaproomUIManager: Closing Region Action Panel due to outside click.");
                CloseRegionActionPanel();
            }
        }

        // Close unit action panel if the click is outside.
        if (unitActionPanel != null && unitActionPanel.activeSelf)
        {
            RectTransform rt1 = unitActionPanel.GetComponent<RectTransform>();
            if (rt1 != null && !RectTransformUtility.RectangleContainsScreenPoint(rt1, Input.mousePosition, null))
            {
                Debug.Log("MaproomUIManager: Closing Unit Action Panel due to outside click.");
                CloseUnitActionPanel();
            }
        }
    }

    // Region Panel Controls.
    public void OpenRegionActionPanel(string regionName)
    {

        if (unitActionPanel != null && unitActionPanel.activeSelf)
        {
            CloseUnitActionPanel();
        }


        regionActionPanel?.SetActive(true);
        taskbarPanel?.SetActive(false);
        // We no longer hide the mapPanel so the map remains visible.
        if (regionNameText != null)
            regionNameText.text = regionName;

        // Set allowClosing with a slight delay to prevent immediate closure.
        allowClosing = false;
        Invoke(nameof(EnablePanelClosing), 0.2f);
    }

    public void CloseRegionActionPanel()
    {
        regionActionPanel?.SetActive(false);
        taskbarPanel?.SetActive(true);
        mapPanel?.SetActive(true);

        if (RegionClickHandler.currentlySelectedRegion != null)
        {
            RegionClickHandler.currentlySelectedRegion.ResetColor();
            RegionClickHandler.currentlySelectedRegion = null;
        }
    }

    private void ShowSubPanel(GameObject panelToShow, Button activeButton)
    {
        negotiatePanel?.SetActive(false);
        producePanel?.SetActive(false);
        provincePanel?.SetActive(false);
        marketPanel?.SetActive(false);

        panelToShow?.SetActive(true);
        mapPanel?.SetActive(false);

        UpdateButtonColors(activeButton);
    }

    private void UpdateButtonColors(Button activeButton)
    {
        if (negotiateButton != null) negotiateButton.GetComponent<Image>().color = Color.white;
        if (produceButton != null) produceButton.GetComponent<Image>().color = Color.white;
        if (provinceButton != null) provinceButton.GetComponent<Image>().color = Color.white;
        if (marketButton != null) marketButton.GetComponent<Image>().color = Color.white;

        if (activeButton != null)
            activeButton.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    // Unit Panel Controls.
    public void OpenUnitActionPanel(string unitName)
    {

        if (regionActionPanel != null && regionActionPanel.activeSelf)
        {
            CloseRegionActionPanel();
        }

        unitActionPanel?.SetActive(true);
        taskbarPanel?.SetActive(false);

        if (unitNameText != null)
            unitNameText.text = unitName;

        Debug.Log("MaproomUIManager: Opened Unit Action Panel - Preventing Immediate Closure.");

        allowClosing = false;
        Invoke(nameof(EnablePanelClosing), 0.2f);
    }

    private void EnablePanelClosing()
    {
        allowClosing = true;
        Debug.Log("MaproomUIManager: Now allowing outside clicks to close panels.");
    }

    public void CloseUnitActionPanel()
    {
        unitActionPanel?.SetActive(false);
        taskbarPanel?.SetActive(true);
    }
}
