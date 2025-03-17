using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public static RegionClickHandler currentlySelectedRegion = null; // Last selected region

    // Public references for the Region Action Panel and province Text.
    public GameObject regionActionPanel;
    public TextMeshProUGUI provinceNameText;

    // Automatically find the Taskbar panel by name.
    private GameObject taskbarPanel;

    // Reference to region data (if available).
    private Region regionData;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Get region data if the Region component exists.
        regionData = GetComponent<Region>();

        // Hide the region action panel initially.
        if (regionActionPanel != null)
            regionActionPanel.SetActive(false);

        // Find the Taskbar.
        taskbarPanel = GameObject.Find("Taskbar");
        if (taskbarPanel != null)
            taskbarPanel.SetActive(true);
    }

    // Called by ClickManager.
    public void OnRegionSelected()
    {
        // Use region data if available; otherwise, fallback to the GameObject's name.
        string regionID = regionData != null ? regionData.regionID : gameObject.name;
        Debug.Log($"RegionClickHandler: Region Selected: {regionID}");

        // Reset previously selected region's color if needed.
        if (currentlySelectedRegion != null && currentlySelectedRegion != this)
            currentlySelectedRegion.ResetColor();

        currentlySelectedRegion = this;


        // Highlight this region.
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
        currentlySelectedRegion = this;

        // Delegate panel control to the UI Manager.
        MaproomUIManager uiManager = FindObjectOfType<MaproomUIManager>();
        if (uiManager != null)
            uiManager.OpenRegionActionPanel(regionID);

        // Update the province text.
        if (provinceNameText != null)
            provinceNameText.text = regionID;
    }

    public void ResetColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
