using Firebase;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using TMPro;

public class ReleaseNotesManager : MonoBehaviour
{
    private static ReleaseNotesManager instance;
    private FirebaseRemoteConfig remoteConfig;
    public TextMeshProUGUI releaseNotesText;

    public static ReleaseNotesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ReleaseNotesManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("ReleaseNotesManager");
                    instance = obj.AddComponent<ReleaseNotesManager>();
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

            //commented out to stop overusing vonfig during testing

            remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            FetchAndActivateConfig(); 
        });
    }

    private async void FetchAndActivateConfig()
    {
        try
        {
            Debug.Log("Fetching Remote Config...");
            var defaults = new Dictionary<string, object>()
            {
                {"release_notes", "No release notes available."}
            };
            await remoteConfig.SetDefaultsAsync(defaults);

            await remoteConfig.FetchAsync(System.TimeSpan.FromHours(1));
            await remoteConfig.ActivateAsync();

            string releaseNotes = remoteConfig.GetValue("release_notes").StringValue;
            Debug.Log("Release Notes Fetched: " + releaseNotes);
            UpdateReleaseNotes(releaseNotes);
        }
        catch (FirebaseException fe)
        {
            Debug.LogError("Firebase Exception: " + fe.Message + "\n" + fe.StackTrace);
            UpdateReleaseNotes("Error fetching release notes.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("General Exception: " + e.Message + "\n" + e.StackTrace);
            UpdateReleaseNotes("Error fetching release notes.");
        }
    }

    public void UpdateReleaseNotes(string newNotes)
    {
        if (releaseNotesText != null)
        {
            releaseNotesText.text = newNotes;
        }
        else
        {
            Debug.LogError("❌ Release Notes Text UI is not assigned!");
        }
    }
}
