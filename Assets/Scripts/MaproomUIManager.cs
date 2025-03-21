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
    public GameObject researchPanel;   // New Research panel.
    public GameObject morePanel;       // New More panel.

    [Header("Taskbar Buttons (for Region Action Panel)")]
    public Button negotiateButton;
    public Button produceButton;
    public Button provinceButton;
    public Button marketButton;
    public Button researchButton;      // New Research button.
    public Button moreButton;          // New More button.

    [Header("Region Info (Optional)")]
    public TextMeshProUGUI regionNameText;

    [Header("Unit Action Panel")]
    public GameObject unitActionPanel;
    public TextMeshProUGUI unitNameText;

    [Header("Map Text Fade Settings (World-Space Texts)")]
    public Camera mapCamera;                    // Assign your map's orthographic camera.
    public TMP_Text[] mapTexts;                 // Drag your world-space TextMeshPro objects here.
    public float fadeInZoomThreshold = 10f;     // Zoom value where texts are fully visible.
    public float fadeOutZoomThreshold = 5f;     // Zoom value where texts are completely faded out.


    [Header("Unit Action Panel Sub-Panels")]
    public GameObject playerUnitButtons;
    public GameObject enemyUnitAttackButton;

    private bool allowClosing = false; // Controls outside-click closure.

    private void Start()
    {
        // Hide all panels initially.
        regionActionPanel?.SetActive(false);
        negotiatePanel?.SetActive(false);
        producePanel?.SetActive(false);
        provincePanel?.SetActive(false);
        marketPanel?.SetActive(false);
        researchPanel?.SetActive(false);
        morePanel?.SetActive(false);
        unitActionPanel?.SetActive(false);

        taskbarPanel?.SetActive(true);
        mapPanel?.SetActive(true);

        // Force each map text to use its own material instance.
        if (mapTexts != null)
        {
            for (int i = 0; i < mapTexts.Length; i++)
            {
                if (mapTexts[i] != null)
                {
                    // Create an instance of the shared material so that changes affect only this text.
                    mapTexts[i].fontMaterial = new Material(mapTexts[i].fontSharedMaterial);
                }
            }
        }

        // Assign button listeners.
        negotiateButton?.onClick.AddListener(() => ShowSubPanel(negotiatePanel, negotiateButton));
        produceButton?.onClick.AddListener(() => ShowSubPanel(producePanel, produceButton));
        provinceButton?.onClick.AddListener(() => ShowSubPanel(provincePanel, provinceButton));
        marketButton?.onClick.AddListener(() => ShowSubPanel(marketPanel, marketButton));
        researchButton?.onClick.AddListener(() => ShowSubPanel(researchPanel, researchButton));
        moreButton?.onClick.AddListener(() => ShowSubPanel(morePanel, moreButton));
    }

    private void Update()
    {
        // Check for outside clicks.
        if (Input.GetMouseButtonDown(0))
        {
            // Delay the check to prevent immediate closure.
            Invoke(nameof(CheckOutsideClick), 0.1f);
        }

        // Update world-space texts' alpha based on camera zoom.
        UpdateMapTextFade();
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
                //Debug.Log("MaproomUIManager: Closing Region Action Panel due to outside click.");
                CloseRegionActionPanel();
            }
        }

        // Close unit action panel if the click is outside.
        if (unitActionPanel != null && unitActionPanel.activeSelf)
        {
            RectTransform rt1 = unitActionPanel.GetComponent<RectTransform>();
            if (rt1 != null && !RectTransformUtility.RectangleContainsScreenPoint(rt1, Input.mousePosition, null))
            {
                //Debug.Log("MaproomUIManager: Closing Unit Action Panel due to outside click.");
                CloseUnitActionPanel();
            }
        }
    }

    // Fades world-space texts based on camera zoom level.
    private void UpdateMapTextFade()
    {
        if (mapCamera == null || mapTexts == null || mapTexts.Length == 0)
            return;

        // Use orthographicSize as the zoom indicator.
        float zoom = mapCamera.orthographicSize;
        float alpha;

        // Debug log to monitor zoom value.
        //Debug.Log("Camera Zoom (orthographicSize): " + zoom);

        if (zoom >= fadeInZoomThreshold)
        {
            alpha = 1f;  // Fully visible when zoomed out.
        }
        else if (zoom <= fadeOutZoomThreshold)
        {
            alpha = 0f;  // Fully faded out when zoomed in.
        }
        else
        {
            // Smooth interpolation between fade out and fade in.
            alpha = Mathf.InverseLerp(fadeOutZoomThreshold, fadeInZoomThreshold, zoom);
        }

        //Debug.Log("Calculated Alpha: " + alpha);

        // Apply the alpha value to each world-space text.
        foreach (TMP_Text tmp in mapTexts)
        {
            if (tmp != null)
            {
                Color c = tmp.color;
                c.a = alpha;
                tmp.color = c;
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
        // Update region name if available.
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
        // Hide all sub-panels.
        negotiatePanel?.SetActive(false);
        producePanel?.SetActive(false);
        provincePanel?.SetActive(false);
        marketPanel?.SetActive(false);
        researchPanel?.SetActive(false);
        morePanel?.SetActive(false);

        // Show the selected panel.
        panelToShow?.SetActive(true);
        // Hide the main map panel for a clearer UI if needed.
        mapPanel?.SetActive(false);
    }

    // Unit Panel Controls.
    public void OpenUnitActionPanel(Unit unit)
    {
        if (regionActionPanel != null && regionActionPanel.activeSelf)
        {
            CloseRegionActionPanel();
        }

        // Show the main Unit Action Panel
        if (unitActionPanel != null)
            unitActionPanel.SetActive(true);

        // Hide the taskbar so it doesn't overlap
        if (taskbarPanel != null)
            taskbarPanel.SetActive(false);

        if (unitNameText != null)
            unitNameText.text = unit.gameObject.name;

        // --- The important part: decide which sub-panel to show. ---
        // For example, if factionID == 0 means "player", else "enemy"
        if (unit.factionID == 0)
        {
            // Player’s unit
            if (playerUnitButtons != null)
                playerUnitButtons.SetActive(true);

            if (enemyUnitAttackButton != null)
                enemyUnitAttackButton.SetActive(false);
        }
        else
        {
            // Enemy unit
            if (playerUnitButtons != null)
                playerUnitButtons.SetActive(false);

            if (enemyUnitAttackButton != null)
                enemyUnitAttackButton.SetActive(true);
        }
        allowClosing = false;
        Invoke(nameof(EnablePanelClosing), 0.2f);
    }

    private void EnablePanelClosing()
    {
        allowClosing = true;
        //Debug.Log("MaproomUIManager: Now allowing outside clicks to close panels.");
    }

    public void CloseUnitActionPanel()
    {
        unitActionPanel?.SetActive(false);
        taskbarPanel?.SetActive(true);
    }

    public void CloseSubPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }
    }

}
