using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameSessionManager : MonoBehaviour
{

    /*
    public static string CurrentGameID;

    public GameObject gameEntryPrefab;
    public Transform contentPanel;

    private Dictionary<string, Sprite> gameImages = new Dictionary<string, Sprite>();

    void Start()
    {
       

        // Load known images
        gameImages["British Isles"] = Resources.Load<Sprite>("GameImages/BritishIsles");

      
        LoadExistingGameSessions();
    }

    // -------------------------------
    // SINGLEPLAYER FLOW
    // -------------------------------
    public void JoinOrCreateSinglePlayerGame(string mapName)
    {
        

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
        
            return;
        }

        string userId = user.UserId;
        DatabaseReference userGameRef = FirebaseDatabase.DefaultInstance
            .GetReference($"user_games/{userId}/{mapName}_singleplayer");

        userGameRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
           
                return;
            }
            if (!task.IsCompleted)
            {
          
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists && snapshot.Child("gameID").Exists)
            {
                string existingGameID = snapshot.Child("gameID").Value.ToString();
                LoadLobbyScene(existingGameID);
            }
            else
            {
                CreateNewSinglePlayerSession(userId, mapName);
            }
        });
    }

    private void CreateNewSinglePlayerSession(string userId, string mapName)
    {
        string gameID = $"SP_{userId}_{mapName.Replace(" ", "")}";

        DatabaseReference gameRef = FirebaseDatabase.DefaultInstance.GetReference($"game_sessions/{gameID}");

        Dictionary<string, object> gameData = new Dictionary<string, object>()
        {
            { "players", new Dictionary<string, object> { { userId, true } } },
            { "map", mapName },
            { "maxPlayers", 1 },
            { "status", "active" },
            { "timestamp", ServerValue.Timestamp }
        };

        gameRef.SetValueAsync(gameData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            if (task.IsCanceled)
            {
                return;
            }


            // Save the newly created gameID under user_games for future lookups
            FirebaseDatabase.DefaultInstance
                .GetReference($"user_games/{userId}/{mapName}_singleplayer")
                .SetValueAsync(new Dictionary<string, object> { { "gameID", gameID } })
                .ContinueWithOnMainThread(task2 =>
                {
                    if (task2.IsFaulted)
                    {
                        return;
                    }
                    LoadLobbyScene(gameID);
                });
        });
    }

    // -------------------------------
    // MULTIPLAYER FLOW
    // -------------------------------
    public void JoinMultiplayerGame(string gameType)
    {

        DatabaseReference gamesRef = FirebaseDatabase.DefaultInstance.GetReference("game_sessions");
        gamesRef.OrderByChild("map").EqualTo(gameType).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    return;
                }
                if (!task.IsCompleted)
                {
                    return;
                }

                DataSnapshot snapshot = task.Result;

                string availableGameID = null;
                foreach (var child in snapshot.Children)
                {
                    int currentPlayers = (int)child.Child("players").ChildrenCount;
                    int maxPlayers = int.Parse(child.Child("maxPlayers").Value.ToString());
                    if (currentPlayers < maxPlayers)
                    {
                        availableGameID = child.Key;
                        break;
                    }
                }

                if (availableGameID != null)
                {
                    AddPlayerToMultiplayerGame(availableGameID);
                }
                else
                {
                    CreateNewMultiplayerSession(gameType);
                }
            });
    }

    private void AddPlayerToMultiplayerGame(string gameID)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            return;
        }

        string userId = user.UserId;
        DatabaseReference gameRef = FirebaseDatabase.DefaultInstance
            .GetReference($"game_sessions/{gameID}/players/{userId}");

        gameRef.SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            LoadLobbyScene(gameID);
        });
    }

    private void CreateNewMultiplayerSession(string gameType)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            return;
        }

        string gameID = "MP_" + System.Guid.NewGuid().ToString();
        DatabaseReference gameRef = FirebaseDatabase.DefaultInstance.GetReference($"game_sessions/{gameID}");

        int maxPlayers = (gameType == "British Isles Multiplayer") ? 20 : 30;

        Dictionary<string, object> gameData = new Dictionary<string, object>()
        {
            { "players", new Dictionary<string, object> { { user.UserId, true } } },
            { "map", gameType },
            { "maxPlayers", maxPlayers },
            { "status", "waiting" },
            { "timestamp", ServerValue.Timestamp }
        };

        gameRef.SetValueAsync(gameData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            LoadLobbyScene(gameID);
        });
    }

    // -------------------------------
    // LOAD EXISTING GAMES
    // -------------------------------
    public void LoadExistingGameSessions()
    {
        DatabaseReference gameSessionsRef = FirebaseDatabase.DefaultInstance.GetReference("game_sessions");

        gameSessionsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            if (!task.IsCompleted)
            {
                return;
            }

            DataSnapshot snapshot = task.Result;

            foreach (var child in snapshot.Children)
            {
                string gameID = child.Key;
                string gameTitle = child.Child("map").Value.ToString();
                int maxPlayers = int.Parse(child.Child("maxPlayers").Value.ToString());
                int currentPlayers = (int)child.Child("players").ChildrenCount;

                CreateGameSession(gameTitle, gameID, maxPlayers, currentPlayers);
            }
        });
    }

    private void CreateGameSession(string gameTitle, string gameID, int maxPlayers, int currentPlayers)
    {

        if (gameEntryPrefab == null || contentPanel == null)
        {
            return;
        }

        GameObject newGameEntry = Instantiate(gameEntryPrefab, contentPanel);
        GameEntryUI gameEntryUI = newGameEntry.GetComponent<GameEntryUI>();
        if (gameEntryUI != null)
        {
            bool isMultiplayer = (maxPlayers > 1);
            gameEntryUI.SetGameData(
                gameTitle,
                gameID,
                maxPlayers,
                currentPlayers,
                GetGameImage(gameTitle),
                isMultiplayer
            );
        }
        else
        {
        }
    }

    private Sprite GetGameImage(string gameTitle)
    {
        return gameImages.ContainsKey(gameTitle) ? gameImages[gameTitle] : null;
    }

    // -------------------------------
    // (Optional) Ensure Static Singleplayer Session
    // -------------------------------
    private void EnsureStaticSinglePlayerSession(string mapName, string staticGameID)
    {
        DatabaseReference staticRef = FirebaseDatabase.DefaultInstance
            .GetReference("game_sessions")
            .Child(staticGameID);

        Debug.Log($"[EnsureStaticSinglePlayerSession] Checking for {staticGameID}...");
        staticRef.GetValueAsync().ContinueWithOnMainThread(checkTask =>
        {
            if (checkTask.IsFaulted)
            {
                Debug.LogError($"❌ [EnsureStaticSinglePlayerSession] Error: {checkTask.Exception}");
                return;
            }
            if (!checkTask.IsCompleted)
            {
                Debug.LogWarning("⚠ [EnsureStaticSinglePlayerSession] Task not completed!");
                return;
            }

            DataSnapshot snapshot = checkTask.Result;
            if (snapshot.Exists)
            {
                Debug.Log($"[EnsureStaticSinglePlayerSession] {staticGameID} exists.");
            }
            else
            {
                Debug.Log($"[EnsureStaticSinglePlayerSession] {staticGameID} does not exist; creating...");
                Dictionary<string, object> staticData = new Dictionary<string, object>
                {
                    { "players", new Dictionary<string, object>() },
                    { "map", mapName },
                    { "maxPlayers", 1 },
                    { "status", "active" },
                    { "timestamp", ServerValue.Timestamp }
                };

                staticRef.SetValueAsync(staticData).ContinueWithOnMainThread(createTask =>
                {
                    if (createTask.IsFaulted)
                    {
                        Debug.LogError($"❌ [EnsureStaticSinglePlayerSession] Failed: {createTask.Exception}");
                        return;
                    }
                    if (!createTask.IsCompleted)
                    {
                        Debug.LogWarning("⚠ [EnsureStaticSinglePlayerSession] Task not completed!");
                        return;
                    }
                    Debug.Log($"✅ [EnsureStaticSinglePlayerSession] Created {staticGameID}.");
                });
            }
        });
    }

    // -------------------------------
    // NEW: Load Lobby Scene for Country/Power Selection
    // -------------------------------
    public void LoadLobbyScene(string gameID)
    {
        Debug.Log($"🟢 [LoadLobbyScene] Preparing to load Lobby Scene for gameID: {gameID}");
        CurrentGameID = gameID; // store the session ID for use in the lobby

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            string userId = user.UserId;
            // Reference the selectedCountry field for this user in this game session.
            DatabaseReference regionRef = FirebaseDatabase.DefaultInstance
                .GetReference($"game_sessions/{gameID}/players/{userId}/selectedCountry");

            regionRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("❌ [LoadLobbyScene] Error checking region selection: " + task.Exception);
                    // On error, you might choose to load the lobby scene as a fallback.
                    SceneManager.LoadScene("LobbyScene");
                    return;
                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // Region has been previously selected, so go directly to the game scene.
                        Debug.Log("🟢 [LoadLobbyScene] Region already selected. Loading GameScene directly...");
                        SceneManager.LoadScene("GameScene");
                    }
                    else
                    {
                        // Region not selected; proceed to lobby for selection.
                        Debug.Log("🟢 [LoadLobbyScene] No region selected. Loading LobbyScene...");
                        SceneManager.LoadScene("LobbyScene");
                    }
                }
            });
        }
        else
        {
            Debug.LogWarning("⚠ [LoadLobbyScene] No authenticated user found, loading LobbyScene by default.");
            SceneManager.LoadScene("LobbyScene");
        }
    }

    */

}
