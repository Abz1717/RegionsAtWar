using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitTypeSelectView : MonoBehaviour
{
    public event Action<UnitType> OnClick;

    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI name;

    private UnitTypeData typeData;

    public UnitType Type => typeData.type;

    private void Start()
    {
        button.onClick.AddListener(ProcessClick);
    }

    private void ProcessClick()
    {
        OnClick?.Invoke(typeData.type);
    }

    public void SetData(UnitTypeData data)
    {
        typeData = data;
        image.sprite = data.sprite;
        name.text = data.name;
    }

}
