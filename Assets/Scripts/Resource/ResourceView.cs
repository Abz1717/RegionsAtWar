using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI name;

    public void SetData(ResourceData data, int ammount)
    {
        image.sprite = data.sprite;
        name.text =ammount.ConvertAmmountToText();
    }
}