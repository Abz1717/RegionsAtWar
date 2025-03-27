using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using Firebase;

public class GlobalChat : MonoBehaviour
{
    private DatabaseReference _databaseReference;
    private FirebaseAuth _firebaseAuth;

    public TMP_InputField messageInput;
    public TextMeshProUGUI chatDisplay;
    public Button sendButton;

    void Start()
    {
        Debug.Log("GlobalChat: Starting Firebase CheckAndFixDependenciesAsync.");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Firebase initialization canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Firebase initialization completed successfully.");

                // Initialize references
                _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("DatabaseReference is set.");

                _firebaseAuth = FirebaseAuth.DefaultInstance;
                Debug.Log("FirebaseAuth is set.");

                // Start the chat listener
                InitializeChat();
            }
        });

        sendButton.onClick.AddListener(() => SendChatMessage(messageInput.text));
    }

    private void InitializeChat()
    {
        Debug.Log("Initializing chat listener on 'global_chat' node.");

        var globalChatRef = _databaseReference.Child("global_chat");
        globalChatRef.OrderByKey().ValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log("HandleValueChanged triggered.");

        if (e.DatabaseError != null)
        {
            Debug.LogError("Error fetching chat messages: " + e.DatabaseError.Message);
            return;
        }

        Debug.Log("Received snapshot. Child count: " + e.Snapshot.ChildrenCount);

        // Clear the current text
        chatDisplay.text = "";

        // Loop through each child (message entry) in the snapshot
        foreach (var childSnapshot in e.Snapshot.Children)
        {
            // Get username and message
            string username = childSnapshot.Child("username").Value?.ToString() ?? "Unknown";
            string message = childSnapshot.Child("message").Value?.ToString() ?? "";

            // Try to parse the timestamp (stored as milliseconds since Unix epoch)
            long timestampLong = 0;
            if (long.TryParse(childSnapshot.Child("timestamp").Value?.ToString(), out var parsedTimestamp))
            {
                timestampLong = parsedTimestamp;
            }

            // Convert Unix timestamp (milliseconds) to a local DateTime
            DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestampLong).DateTime.ToLocalTime();
            // Format it however you want, e.g. "HH:mm:ss"
            string timeString = dateTime.ToString("HH:mm:ss");

            // Append everything to the chat display
            // Example: [14:07:59] Username: Hello!
            chatDisplay.text += $"[{timeString}] {username}: {message}\n";
        }

        Debug.Log("UI chat display updated with latest messages.");
    }

    public void SendChatMessage(string message)
    {
        Debug.Log($"SendChatMessage called with text: '{message}'.");

        FirebaseUser user = _firebaseAuth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("User not authenticated. Cannot send message.");
            return;
        }

        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("Cannot send an empty message.");
            return;
        }

        string userId = user.UserId;
        string username = user.DisplayName ?? "Anonymous";

        Debug.Log($"Will send message as: username='{username}', userId='{userId}'.");

        Dictionary<string, object> newMessage = new Dictionary<string, object>()
        {
            { "userId", userId },
            { "username", username },
            { "message", message },
            { "timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds() }
        };

        Debug.Log("Pushing message to Realtime Database without JSON serialization.");

        // Use SetValueAsync(...) instead of SetRawJsonValueAsync(...)
        _databaseReference.Child("global_chat").Push()
            .SetValueAsync(newMessage)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SetValueAsync was canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("SetValueAsync encountered an error: " + task.Exception);
                }
                else
                {
                    Debug.Log("Message uploaded successfully to Realtime Database.");
                }
            });

        // Clear input field
        messageInput.text = "";
    }
}
