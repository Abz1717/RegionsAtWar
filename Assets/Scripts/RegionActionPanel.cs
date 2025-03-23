using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionActionPanel : MonoBehaviour, IUIPanel
{
    [SerializeField] private Button constructButton;
    [SerializeField] private Button produceButton;
    [SerializeField] private Button attackButton;

    private Region currentRegion;

    [Header("Region Info")]
    [SerializeField] private TextMeshProUGUI regionNameText;
    [SerializeField] private Image factionFlagImage;


    [Header("Production Info")]
    [SerializeField] private TextMeshProUGUI moneyProductionText;
    [SerializeField] private TextMeshProUGUI manpowerProductionText;
    [SerializeField] private TextMeshProUGUI resourceProductionText;

    [SerializeField] private Image resourceIconImage;





    private void Start()
    {
        constructButton.onClick.AddListener(Construct);
        produceButton.onClick.AddListener(Produce);
        attackButton.onClick.AddListener(Attack);
    }

    public void Show(Region region)
    {
        currentRegion = region;

        if (regionNameText != null)
            regionNameText.text = region.regionID;

        gameObject.SetActive(true);

        bool isPlayer = (region.ownerID == GameManager.Instance.LocalPlayerId);

        constructButton.gameObject.SetActive(isPlayer);
        produceButton.gameObject.SetActive(isPlayer);
        attackButton.gameObject.SetActive(!isPlayer);


        // Update production info text.
        if (moneyProductionText != null)
            moneyProductionText.text = region.moneyRate.ToString();
        if (manpowerProductionText != null)
            manpowerProductionText.text = region.manpowerRate.ToString();
        if (resourceProductionText != null)
            resourceProductionText.text = region.resourceRate.ToString();

        // Update the resource icon using the region's resource type.
        if (resourceIconImage != null && GameManager.Instance.gameConfig.resourcesConfiguration != null)
        {
            // Look for a matching ResourceData entry where the type matches.
            ResourceData rd = GameManager.Instance.gameConfig.resourcesConfiguration.resources
                .Find(r => r.type == region.resourseType);
            if (rd != null)
            {
                resourceIconImage.sprite = rd.sprite;
            }
            else
            {
                Debug.LogWarning("No ResourceData found for resource type: " + region.resourseType);
                resourceIconImage.sprite = null; // Optionally clear the icon if not found.
            }
        }


        // Update the faction flag using the player data from GameConfig.
        if (factionFlagImage != null && GameManager.Instance.gameConfig != null)
        {
            // Find the player with the ID matching the region's owner.
            var ownerData = GameManager.Instance.gameConfig.Players.Find(p => p.Id == region.ownerID);
            if (ownerData != null)
            {
                factionFlagImage.sprite = ownerData.flag;
            }
            else
            {
                Debug.LogWarning("No player data found for region owner: " + region.ownerID);
                factionFlagImage.sprite = null;
            }
        }
    }

    public void Hide()
    {
        if (currentRegion != null)
        {
            currentRegion.ResetColor();
        }
        gameObject.SetActive(false);
    }


    private void Construct()
    {
        Hide();
        MaproomUIManager.Instance.OpenRegionConsruction(currentRegion);
    }

    private void Produce()
    {

        Hide();
        MaproomUIManager.Instance.OpenRegionProduction(currentRegion);
    }

    private void Attack()
    {
        GameManager.Instance.AttackRegion(GameManager.Instance.LocalPlayerId, currentRegion);
    }
}
