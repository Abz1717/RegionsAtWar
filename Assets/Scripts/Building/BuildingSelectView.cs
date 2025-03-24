using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectView : MonoBehaviour
{
    public event Action<BuildingData> OnClick;

    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI durationText;

    [Header("Resources")]
    [SerializeField] ResourcesConfiguration resourcesConfiguration;
    [SerializeField] ResourceView resourcePrefab;
    [SerializeField] Transform resourcesParent;

    private BuildingData buildingData;

    public BuildingType Type => buildingData.type;

    private void Start()
    {
        button.onClick.AddListener(ProcessClick);
    }

    private void ProcessClick()
    {
        OnClick?.Invoke(buildingData);
    }

    public void SetData(BuildingData data)
    {
        buildingData = data;
        image.sprite = data.sprite;
        nameText.text = data.name;
        durationText.text = data.duration.ToString()+"m";

        foreach (var resourceCost in data.cost)
        {
            var resourceView = Instantiate(resourcePrefab, resourcesParent);
            resourceView.SetData(resourcesConfiguration.resources.Find(resource => resource.type == resourceCost.resource), resourceCost.ammount);
        }
    }

}
