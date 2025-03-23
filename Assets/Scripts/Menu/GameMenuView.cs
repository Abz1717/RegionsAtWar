using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuView :MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(StartGamw);
    }

    private void StartGamw()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void Filter(string filter)
    {
        gameObject.SetActive(nameText.text.Contains(filter));
    }
}
