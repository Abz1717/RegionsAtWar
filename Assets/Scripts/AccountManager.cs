using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

public class UserAccountManager : MonoBehaviour
{
    private FirebaseAuth auth;

    [Header("Email Update")]
    public TMP_InputField emailInput;           // TMP InputField for new email
    public Button updateEmailButton;            // Button to trigger email update

    [Header("Password Update")]
    public TMP_InputField passwordInput;        // TMP InputField for new password
    public Button updatePasswordButton;         // Button to trigger password update

    [Header("Username Update")]
    public TMP_InputField usernameInput;        // TMP InputField for new username/display name
    public Button updateUsernameButton;         // Button to trigger username update

    [Header("Feedback")]
    public TMP_Text feedbackText;               // Text to show feedback messages

    void Start()
    {
        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;

        // Check if required components are assigned
        if (feedbackText == null)
        {
            Debug.LogError("Feedback Text is not assigned in the inspector.");
        }
        if (emailInput == null)
        {
            Debug.LogError("Email Input is not assigned in the inspector.");
        }
        if (passwordInput == null)
        {
            Debug.LogError("Password Input is not assigned in the inspector.");
        }
        if (usernameInput == null)
        {
            Debug.LogError("Username Input is not assigned in the inspector.");
        }
        if (updateEmailButton == null)
        {
            Debug.LogError("Update Email Button is not assigned in the inspector.");
        }
        if (updatePasswordButton == null)
        {
            Debug.LogError("Update Password Button is not assigned in the inspector.");
        }
        if (updateUsernameButton == null)
        {
            Debug.LogError("Update Username Button is not assigned in the inspector.");
        }

        // Assign button listeners
        if (updateEmailButton != null) updateEmailButton.onClick.AddListener(UpdateEmail);
        if (updatePasswordButton != null) updatePasswordButton.onClick.AddListener(UpdatePassword);
        if (updateUsernameButton != null) updateUsernameButton.onClick.AddListener(UpdateUsername);
    }

    /// <summary>
    /// Updates the user's email address using the new email input.
    /// This uses the deprecated UpdateEmailAsync method (without verification).
    /// </summary>
    public void UpdateEmail()
    {
        // Check if feedbackText and emailInput are assigned
        if (feedbackText == null) return;
        if (emailInput == null)
        {
            feedbackText.text = "Email input is not assigned.";
            return;
        }

        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            feedbackText.text = "No user is signed in.";
            return;
        }

        string newEmail = emailInput.text;
        if (string.IsNullOrEmpty(newEmail))
        {
            feedbackText.text = "Please enter a valid email.";
            return;
        }

        // Suppress deprecation warning for UpdateEmailAsync
#pragma warning disable CS0618
        user.UpdateEmailAsync(newEmail)
#pragma warning restore CS0618
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Error updating email: " + task.Exception);
                    feedbackText.text = "Error updating email. (Re-auth may be needed.)";
                }
                else
                {
                    feedbackText.text = "Email updated successfully.";
                }
            });
    }

    /// <summary>
    /// Updates the user's password using the new password input.
    /// </summary>
    public void UpdatePassword()
    {
        // Check if feedbackText and passwordInput are assigned
        if (feedbackText == null) return;
        if (passwordInput == null)
        {
            feedbackText.text = "Password input is not assigned.";
            return;
        }

        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            feedbackText.text = "No user is signed in.";
            return;
        }

        string newPassword = passwordInput.text;
        if (string.IsNullOrEmpty(newPassword))
        {
            feedbackText.text = "Please enter a valid password.";
            return;
        }

        user.UpdatePasswordAsync(newPassword)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Error updating password: " + task.Exception);
                    feedbackText.text = "Error updating password. (Re-auth may be needed.)";
                }
                else
                {
                    feedbackText.text = "Password updated successfully.";
                }
            });
    }

    /// <summary>
    /// Updates the user's display name (username) using the new username input.
    /// </summary>
    public void UpdateUsername()
    {
        // Check if feedbackText and usernameInput are assigned
        if (feedbackText == null) return;
        if (usernameInput == null)
        {
            feedbackText.text = "Username input is not assigned.";
            return;
        }

        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            feedbackText.text = "No user is signed in.";
            return;
        }

        string newUsername = usernameInput.text;
        if (string.IsNullOrEmpty(newUsername))
        {
            feedbackText.text = "Please enter a valid username.";
            return;
        }

        UserProfile profile = new UserProfile { DisplayName = newUsername };
        user.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Error updating username: " + task.Exception);
                    feedbackText.text = "Error updating username.";
                }
                else
                {
                    feedbackText.text = "Username updated successfully.";
                }
            });
    }
}
