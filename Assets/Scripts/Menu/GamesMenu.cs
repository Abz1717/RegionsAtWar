using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamesMenu : MonoBehaviour
{
    [SerializeField] private List<GameMenuView> games;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button searchButton;

    private void Start()
    {
        searchButton.onClick.AddListener(Filter);
    }

    private void Filter()
    {
        var filter = inputField.text;
        foreach (var game in games)
        {
            game.Filter(filter);
        }
    }
}
