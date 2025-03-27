using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System.Collections.Generic;

public class UserListManager : MonoBehaviour
{
    [Header("UI Component")]
    public TMP_Text userListText;  // TextMeshPro text component to display the usernames

    private DatabaseReference databaseRef;

    void Start()
    {
        // Initialize the Firebase Database reference
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Fetch the list of users and display their usernames
        FetchUsernames();
    }

    /// <summary>
    /// Fetches the list of usernames from the "users" node in the database.
    /// </summary>
    void FetchUsernames()
    {
        // Access the "users" node
        databaseRef.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle any errors
                if (userListText != null)
                    userListText.text = "Error fetching users.";
                Debug.LogError("Error fetching users: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<string> usernames = new List<string>();

                // Iterate through each user node
                foreach (DataSnapshot userSnapshot in snapshot.Children)
                {
                    // Assuming each user node has a "displayName" field
                    string username = userSnapshot.Child("displayName").Value?.ToString();
                    if (!string.IsNullOrEmpty(username))
                    {
                        usernames.Add(username);
                    }
                    else
                    {
                        usernames.Add("Unknown");
                    }
                }

                // Display the usernames in the TMP_Text component
                if (userListText != null)
                {
                    if (usernames.Count > 0)
                    {
                        userListText.text = "Usernames:\n" + string.Join("\n", usernames);
                    }
                    else
                    {
                        userListText.text = "No users found.";
                    }
                }
            }
        });
    }
}
