using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTypeSelectView : MonoBehaviour
{
    public event Action<BuildingType> OnClick;

    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI name;

    private BuildingTypeData typeData;

    private void Start()
    {
        button.onClick.AddListener(ProcessClick);
    }

    private void ProcessClick()
    {
        OnClick?.Invoke(typeData.type);
    }

    public void SetData(BuildingTypeData data)
    {
        typeData = data;
        image.sprite = data.sprite;
        name.text = data.name;
    }
}
