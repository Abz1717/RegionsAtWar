using UnityEngine;
using UnityEngine.UI;

public class DebugButtonTest : MonoBehaviour
{
    public Button testButton;

    void Start()
    {
        Debug.Log("🔥 Test script Start() is running");

        if (testButton != null)
        {
            testButton.onClick.AddListener(() =>
            {
                Debug.Log("✅ Test button was clicked!");
            });
        }
        else
        {
            Debug.LogWarning("⚠️ Test button is not assigned in inspector.");
        }
    }
}
