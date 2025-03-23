using UnityEngine;
using TMPro;

public class ResourceUIView : MonoBehaviour
{
    [SerializeField] private Resource type;
    [SerializeField] private TextMeshProUGUI text;

    public Resource Type => type;

    public void UpdateValue(int value)
    {
        text.text = value.ConvertAmmountToText();
    }

}
