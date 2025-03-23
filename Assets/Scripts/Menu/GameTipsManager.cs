using Firebase;
using Firebase.RemoteConfig;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Firebase.Extensions;

public class GameTipsManager : MonoBehaviour
{
    private static GameTipsManager instance;
    private FirebaseRemoteConfig remoteConfig;
    public TextMeshProUGUI gameTipsText;

    public static GameTipsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameTipsManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameTipsManager");
                    instance = obj.AddComponent<GameTipsManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Firebase dependency check canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase dependency check encountered an error: " + task.Exception);
                return;
            }

            remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            FetchAndActivateConfig();
        });
    }

    private async void FetchAndActivateConfig()
    {
        try
        {
            Debug.Log("Fetching Remote Config for Game Tips...");
            var defaults = new Dictionary<string, object>()
            {
                {"game_tips", "No game tips available."}
            };
            await remoteConfig.SetDefaultsAsync(defaults);

            // Set a cache expiration time. Here, we cache for one hour.
            await remoteConfig.FetchAsync(System.TimeSpan.FromHours(1));
            await remoteConfig.ActivateAsync();

            string gameTips = remoteConfig.GetValue("game_tips").StringValue;
            Debug.Log("Game Tips Fetched: " + gameTips);
            UpdateGameTips(gameTips);
        }
        catch (FirebaseException fe)
        {
            Debug.LogError("Firebase Exception: " + fe.Message + "\n" + fe.StackTrace);
            UpdateGameTips("Error fetching game tips.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("General Exception: " + e.Message + "\n" + e.StackTrace);
            UpdateGameTips("Error fetching game tips.");
        }
    }

    public void UpdateGameTips(string newTips)
    {
        if (gameTipsText != null)
        {
            gameTipsText.text = newTips;
        }
        else
        {
            Debug.LogError("❌ Game Tips Text UI is not assigned!");
        }
    }
}
