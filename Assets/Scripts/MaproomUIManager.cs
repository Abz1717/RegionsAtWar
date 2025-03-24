using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class MaproomUIManager : Singleton<MaproomUIManager>
{
    [Header("Panels")]
    public RegionActionPanel regionActionPanel;
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
    public Button constructButton;
    public Button produceButton;
    public Button marketButton;
    public Button researchButton;      // New Research button.
    public Button moreButton;          // New More button.

    [Header("Region Info (Optional)")]
    public TextMeshProUGUI regionNameText;

    [Header("Unit Action Panel")]
    public TextMeshProUGUI unitNameText;

    [Header("Map Text Fade Settings (World-Space Texts)")]
    public Camera mapCamera;                    // Assign your map's orthographic camera.
    public TMP_Text[] mapTexts;                 // Drag your world-space TextMeshPro objects here.
    public float fadeInZoomThreshold = 10f;     // Zoom value where texts are fully visible.
    public float fadeOutZoomThreshold = 5f;     // Zoom value where texts are completely faded out.


    [Header("Unit Action Panel Sub-Panels")]
    public GameObject playerUnitButtons;
    public GameObject enemyUnitAttackButton;

    [SerializeField] private ResourcesBar resourcesBar;
    [SerializeField] private ProductionScreen productionScreen;
    [SerializeField] private UnitActionPanel unitActionPanel;
    public UnitActionPanel UnitActionPanel => unitActionPanel;

    [Header("Taskbar Score Counters")]
    [SerializeField] private TextMeshProUGUI primaryCounterText;   
    [SerializeField] private TextMeshProUGUI secondaryCounterText;

    private int primaryCounter = 1;     
    private TimeSpan secondaryCounter;

    [Header("Menu Button")]
    public Button mainMenuButton;

    private int dayLengthInMinutes = 5;


    [System.Serializable]
    public class PlayerScoreUI
    {
        public int playerId;
        public TextMeshProUGUI scoreNumberText;       // Displays raw region count
        public TextMeshProUGUI rankText;              // Displays the rank 
    }

    public List<PlayerScoreUI> playerScoreUIList;

    private bool allowClosing = false; // Controls outside-click closure

    IUIPanel currentPanel = null;
    public bool HasPanel => currentPanel != null;
    private void Start()
    {

        // Hide all panels initially.
        regionActionPanel.Hide();
        negotiatePanel?.SetActive(false);
        producePanel?.SetActive(false);
        provincePanel?.SetActive(false);
        marketPanel?.SetActive(false);
        researchPanel?.SetActive(false);
        morePanel?.SetActive(false);
        unitActionPanel.Hide();

        taskbarPanel?.SetActive(true);
        mapPanel?.SetActive(true);

        secondaryCounter = TimeSpan.FromMinutes( GameManager.Instance.gameConfig.dayLengthInMinutes);

        mainMenuButton?.onClick.AddListener(GoToMainMenu);

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
        constructButton?.onClick.AddListener(() => OpenRegionConstruction(RegionClickHandler.currentlySelectedRegion == null ? null: RegionClickHandler.currentlySelectedRegion.RegionData));
        produceButton?.onClick.AddListener(() => OpenRegionProduction(RegionClickHandler.currentlySelectedRegion == null ? null : RegionClickHandler.currentlySelectedRegion.RegionData));
        marketButton?.onClick.AddListener(() => ShowSubPanel(marketPanel, marketButton));
        researchButton?.onClick.AddListener(() => ShowSubPanel(researchPanel, researchButton));
        moreButton?.onClick.AddListener(() => ShowSubPanel(morePanel, moreButton));

        ServiceCoroutine.Instance.StartCoroutine(TaskbarCounterRoutine());


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

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    private void CheckOutsideClick()
    {
        if (!allowClosing) return;


        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;


        // Check if Region Action Panel is open.
        if (regionActionPanel != null && regionActionPanel.gameObject.activeSelf)
        {
            RectTransform rt = regionActionPanel.GetComponent<RectTransform>();
            if (rt != null && !RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, null))
            {

                // Also, if the currently selected UI object is not a child of the panel, then close.
                if (currentSelected == null || !currentSelected.transform.IsChildOf(regionActionPanel.transform))
                {
                    CloseRegionActionPanel();
                }
            }
        }

        // Close unit action panel if the click is outside.
        if (unitActionPanel != null && unitActionPanel.gameObject.activeSelf)
        {
            RectTransform rt1 = unitActionPanel.GetComponent<RectTransform>();
            if (rt1 != null && !RectTransformUtility.RectangleContainsScreenPoint(rt1, Input.mousePosition, null))
            {
                if (currentSelected == null || !currentSelected.transform.IsChildOf(unitActionPanel.transform))
                {
                    CloseUnitActionPanel();
                }
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

    public void UpdateResource(Resource type, int ammount)
    {
        resourcesBar.UpdateResource(type, ammount);
    }

    private void HideCurrentPanel()
    {
        taskbarPanel.gameObject.SetActive(true);
        if (currentPanel != null)
        {
            currentPanel.Hide();
        }
        currentPanel = null;
    }

    // Region Panel Controls.
    public void OpenRegionActionPanel(Region region)
    {
        HideCurrentPanel();
        currentPanel = regionActionPanel;

        regionActionPanel.Show(region);
        taskbarPanel?.SetActive(false);
        // Update region name if available.
        if (regionNameText != null)
            regionNameText.text = region.regionID;

        // Set allowClosing with a slight delay to prevent immediate closure.
        allowClosing = false;
        Invoke(nameof(EnablePanelClosing), 0.2f);
    }

    public void OpenRegionConstruction(Region region)
    {
        HideCurrentPanel();
        currentPanel = productionScreen;
        productionScreen.ShowBuildings(region);
    }

    public void OpenRegionProduction(Region region)
    {
        HideCurrentPanel();
        currentPanel = productionScreen;
        productionScreen.ShowUnits(region);
    }

    public void CloseRegionActionPanel()
    {
        regionActionPanel.Hide();
        taskbarPanel?.SetActive(true);
        mapPanel?.SetActive(true);
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
    public void OpenUnitActionPanel()
    {
        HideCurrentPanel();
        currentPanel = unitActionPanel;

        // Show the main Unit Action Panel
         unitActionPanel.gameObject.SetActive(true);

        unitActionPanel.Show();

        // Check the faction of the selected unit.
        if (GameManager.Instance.SelectedUnit != null)
        {
            if (GameManager.Instance.SelectedUnit.Unit.factionID == GameManager.Instance.LocalPlayerId)
            {
                // Friendly unit: show player buttons, hide enemy button.
                playerUnitButtons.SetActive(true);
                enemyUnitAttackButton.SetActive(false);
            }
            else
            {
                // Enemy unit: show enemy attack button, hide player buttons.
                playerUnitButtons.SetActive(false);
                enemyUnitAttackButton.SetActive(true);
            }
        }
        else
        {
            // No unit selected – disable both (optional).
            playerUnitButtons.SetActive(false);
            enemyUnitAttackButton.SetActive(false);
        }

        // Hide the taskbar so it doesn't overlap.
        if (taskbarPanel != null)
            taskbarPanel.SetActive(false);
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
        unitActionPanel.Hide();
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



    public bool IsUnitActionPanelOpen
    {
        get
        {
            // If unitActionPanel is not null, return whether its GameObject is active
            return unitActionPanel != null && unitActionPanel.gameObject.activeSelf;
        }
    }


    public void UpdateScoreUI()
    {
        int totalRegions = RegionManager.Instance.regionData.Count;

        List<PlayerScoreData> scores = new List<PlayerScoreData>();
        foreach (var player in GameManager.Instance.gameConfig.Players)
        {
            int regionCount = RegionManager.Instance.regionData.FindAll(r => r.ownerID == player.Id).Count;
            scores.Add(new PlayerScoreData { playerId = player.Id, regionCount = regionCount });
        }

        scores.Sort((a, b) => b.regionCount.CompareTo(a.regionCount));

        Dictionary<int, string> playerRankings = new Dictionary<int, string>();

        for (int i = 0; i < scores.Count; i++)
        {
            int rankNumber = i + 1; // Rank starts at 1.
            string rankString = GetOrdinal(rankNumber);
            playerRankings[scores[i].playerId] = rankString;
        }

        foreach (var playerScore in playerScoreUIList)
        {
            int regionCount = RegionManager.Instance.regionData.FindAll(r => r.ownerID == playerScore.playerId).Count;
            playerScore.scoreNumberText.text = regionCount.ToString();

            if (playerRankings.ContainsKey(playerScore.playerId))
            {
                playerScore.rankText.text = playerRankings[playerScore.playerId];
            }
            else
            {
                playerScore.rankText.text = "";
            }
        }
    }

    private string GetOrdinal(int number)
    {
        if (number <= 0)
            return number.ToString();

        int lastTwoDigits = number % 100;
        int lastDigit = number % 10;
        if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
            return number + "th";
        switch (lastDigit)
        {
            case 1:
                return number + "st";
            case 2:
                return number + "nd";
            case 3:
                return number + "rd";
            default:
                return number + "th";
        }
    }

    private class PlayerScoreData
    {
        public int playerId;
        public int regionCount;
    }


    // Coroutine to update the taskbar counters.
    private IEnumerator TaskbarCounterRoutine()
    {
        float interval = 1f;
        while (true)
        {
            yield return new WaitForSeconds(interval);

            secondaryCounter-=TimeSpan.FromSeconds(1); 

            if (secondaryCounter.TotalSeconds < 0)
            {
                secondaryCounter = TimeSpan.FromMinutes( GameManager.Instance.gameConfig.dayLengthInMinutes);
                primaryCounter++;
            }

            UpdateTaskbarCountersUI();
        }
    }

    private void UpdateTaskbarCountersUI()
    {
        if (primaryCounterText != null)
            primaryCounterText.text = primaryCounter.ToString(); 
        if (secondaryCounterText != null)
            secondaryCounterText.text = $"{secondaryCounter.Minutes:00}:{secondaryCounter.Seconds:00}";
    }

}


public interface IUIPanel { void Hide(); }