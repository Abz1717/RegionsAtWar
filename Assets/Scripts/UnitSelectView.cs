using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectView : MonoBehaviour
{
    public event Action<UnitData> OnClick;

    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI durationText;

    [Header("Resources")]
    [SerializeField] ResourcesConfiguration resourcesConfiguration;
    [SerializeField] ResourceView resourcePrefab;
    [SerializeField] Transform resourcesParent;

    private UnitData unitData;

    public UnitType UnitType => unitData.type;

    private void Start()
    {
        button.onClick.AddListener(ProcessClick);
    }

    private void ProcessClick()
    {
        OnClick?.Invoke(unitData);
    }

    public void SetData(UnitData data)
    {
        unitData = data;
        image.sprite = data.sprite;
        nameText.text = data.name;
        durationText.text = data.duration;

        foreach (var resourceCost in data.cost)
        {
            var resourceView = Instantiate(resourcePrefab, resourcesParent);
            resourceView.SetData(resourcesConfiguration.resources.Find(resource => resource.type == resourceCost.resource), resourceCost.ammount);
        }
    }

}
