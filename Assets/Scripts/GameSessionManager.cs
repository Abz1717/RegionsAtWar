using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameSessionManager : MonoBehaviour
{
    // This static variable stores the current game session ID so it can be used in the lobby.
    public static string CurrentGameID;

    public GameObject gameEntryPrefab;
    public Transform contentPanel;

    private Dictionary<string, Sprite> gameImages = new Dictionary<string, Sprite>();

    void Start()
    {
        Debug.Log("🟢 [GameSessionManager] Start() called. Checking existing game sessions...");

        // Load known images
        gameImages["British Isles"] = Resources.Load<Sprite>("GameImages/BritishIsles");
        Debug.Log("🟢 [GameSessionManager] Loaded image for British Isles");

        // Optionally ensure a static singleplayer session exists, then...
        LoadExistingGameSessions();
    }

    // -------------------------------
    // SINGLEPLAYER FLOW
    // -------------------------------
    public void JoinOrCreateSinglePlayerGame(string mapName)
    {
        Debug.Log($"🟢 [JoinOrCreateSinglePlayerGame] Called for map: {mapName}");

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User not authenticated!");
            return;
        }

        string userId = user.UserId;
        DatabaseReference userGameRef = FirebaseDatabase.DefaultInstance
            .GetReference($"user_games/{userId}/{mapName}_singleplayer");

        userGameRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ [JoinOrCreateSinglePlayerGame] Error: " + task.Exception);
                return;
            }
            if (!task.IsCompleted)
            {
                Debug.LogWarning("⚠ [JoinOrCreateSinglePlayerGame] Task not completed!");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists && snapshot.Child("gameID").Exists)
            {
                string existingGameID = snapshot.Child("gameID").Value.ToString();
                Debug.Log($"🟢 [JoinOrCreateSinglePlayerGame] Found existing game ID: {existingGameID}");
                LoadLobbyScene(existingGameID);
            }
            else
            {
                Debug.Log($"🟢 [JoinOrCreateSinglePlayerGame] No session found for {mapName}, creating new...");
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

        Debug.Log($"🟢 [CreateNewSinglePlayerSession] Setting value at: game_sessions/{gameID}");
        gameRef.SetValueAsync(gameData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ [CreateNewSinglePlayerSession] Failed: " + task.Exception);
                return;
            }
            if (task.IsCanceled)
            {
                Debug.LogWarning("⚠ [CreateNewSinglePlayerSession] Task was canceled!");
                return;
            }

            Debug.Log("✅ [CreateNewSinglePlayerSession] Session data written to Firebase. Updating user_games...");

            // Save the newly created gameID under user_games for future lookups
            FirebaseDatabase.DefaultInstance
                .GetReference($"user_games/{userId}/{mapName}_singleplayer")
                .SetValueAsync(new Dictionary<string, object> { { "gameID", gameID } })
                .ContinueWithOnMainThread(task2 =>
                {
                    if (task2.IsFaulted)
                    {
                        Debug.LogError("❌ [CreateNewSinglePlayerSession] Error updating user_games: " + task2.Exception);
                        return;
                    }
                    Debug.Log("✅ [CreateNewSinglePlayerSession] user_games updated. Loading lobby scene...");
                    LoadLobbyScene(gameID);
                });
        });
    }

    // -------------------------------
    // MULTIPLAYER FLOW
    // -------------------------------
    public void JoinMultiplayerGame(string gameType)
    {
        Debug.Log($"🟢 [JoinMultiplayerGame] Called for type: {gameType}");

        DatabaseReference gamesRef = FirebaseDatabase.DefaultInstance.GetReference("game_sessions");
        gamesRef.OrderByChild("map").EqualTo(gameType).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("❌ [JoinMultiplayerGame] Error querying: " + task.Exception);
                    return;
                }
                if (!task.IsCompleted)
                {
                    Debug.LogWarning("⚠ [JoinMultiplayerGame] Task not completed!");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                Debug.Log($"🟢 [JoinMultiplayerGame] snapshot.ChildrenCount: {snapshot.ChildrenCount} for type {gameType}");

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
                    Debug.Log($"🟢 [JoinMultiplayerGame] Found session: {availableGameID}, joining...");
                    AddPlayerToMultiplayerGame(availableGameID);
                }
                else
                {
                    Debug.Log($"🟢 [JoinMultiplayerGame] No available session found, creating new...");
                    CreateNewMultiplayerSession(gameType);
                }
            });
    }

    private void AddPlayerToMultiplayerGame(string gameID)
    {
        Debug.Log($"🟢 [AddPlayerToMultiplayerGame] Called for {gameID}");
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User not authenticated!");
            return;
        }

        string userId = user.UserId;
        DatabaseReference gameRef = FirebaseDatabase.DefaultInstance
            .GetReference($"game_sessions/{gameID}/players/{userId}");

        gameRef.SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ [AddPlayerToMultiplayerGame] Failed to join: " + task.Exception);
                return;
            }
            Debug.Log($"✅ [AddPlayerToMultiplayerGame] Joined game: {gameID}");
            LoadLobbyScene(gameID);
        });
    }

    private void CreateNewMultiplayerSession(string gameType)
    {
        Debug.Log($"🟢 [CreateNewMultiplayerSession] Called for {gameType}");
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User not authenticated!");
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

        Debug.Log($"🟢 [CreateNewMultiplayerSession] Setting value at game_sessions/{gameID}");
        gameRef.SetValueAsync(gameData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ [CreateNewMultiplayerSession] Failed: " + task.Exception);
                return;
            }
            Debug.Log("✅ [CreateNewMultiplayerSession] New game created: " + gameID);
            LoadLobbyScene(gameID);
        });
    }

    // -------------------------------
    // LOAD EXISTING GAMES
    // -------------------------------
    public void LoadExistingGameSessions()
    {
        Debug.Log("🟢 [LoadExistingGameSessions] Loading sessions...");
        DatabaseReference gameSessionsRef = FirebaseDatabase.DefaultInstance.GetReference("game_sessions");

        gameSessionsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ [LoadExistingGameSessions] Error: " + task.Exception);
                return;
            }
            if (!task.IsCompleted)
            {
                Debug.LogWarning("⚠ [LoadExistingGameSessions] Task not completed!");
                return;
            }

            DataSnapshot snapshot = task.Result;
            Debug.Log($"🟢 [LoadExistingGameSessions] snapshot.ChildrenCount: {snapshot.ChildrenCount}");

            foreach (var child in snapshot.Children)
            {
                string gameID = child.Key;
                string gameTitle = child.Child("map").Value.ToString();
                int maxPlayers = int.Parse(child.Child("maxPlayers").Value.ToString());
                int currentPlayers = (int)child.Child("players").ChildrenCount;

                Debug.Log($"🟢 [LoadExistingGameSessions] Found {gameTitle} with ID {gameID}. {currentPlayers}/{maxPlayers}");
                CreateGameSession(gameTitle, gameID, maxPlayers, currentPlayers);
            }
        });
    }

    private void CreateGameSession(string gameTitle, string gameID, int maxPlayers, int currentPlayers)
    {
        Debug.Log($"🟢 [CreateGameSession] Creating UI for {gameTitle} (ID: {gameID}). {currentPlayers}/{maxPlayers}");

        if (gameEntryPrefab == null || contentPanel == null)
        {
            Debug.LogError("❌ GameEntryPrefab or ContentPanel not assigned!");
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
            Debug.Log($"✅ [CreateGameSession] UI created for {gameTitle}");
        }
        else
        {
            Debug.LogError("❌ GameEntryUI component missing on prefab!");
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

}
