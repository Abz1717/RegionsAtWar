using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    private FirebaseAuth auth;
    private FirebaseUser user;

    [Header("UI Panels")]
    public GameObject mainStartPanel;  // New main start panel containing the three buttons (Google, Login, Register)
    public GameObject loginPanel;      // Existing login panel for email and password
    public GameObject registerPanel;   // Registration panel for username, email, and password
    public GameObject usernamePopup;   // Popup for setting username for first-time Google logins

    [Header("Main Start Panel Buttons")]
    public Button googleButtonMain;    // Google sign-in button on the main start panel
    public Button loginButtonMain;     // Button to open the login panel
    public Button registerButtonMain;  // Button to open the registration panel

    [Header("Login Panel UI References")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;
    public Button backButtonLogin;     // Back button on the login panel

    [Header("Register Panel UI References")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerUsernameInput;
    public Button registerButton;
    public Button backButtonRegister;  // Back button on the registration panel

    [Header("Google Username Popup UI References")]
    public TMP_InputField googleUsernameInput;   // For Google username entry on first login
    public Button confirmGoogleUsernameButton;

    [Header("Message Display")]
    public TextMeshProUGUI messageText;

    [Header("Google Sign-In")]
    public string GoogleAPI = "YOUR_GOOGLE_CLIENT_ID"; // Replace with your Google OAuth Client ID
    private bool isGoogleSignInInitialized = false;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        // Start with only the main start panel visible.
        mainStartPanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        usernamePopup.SetActive(false);

        // Setup listeners for main start panel buttons.
        googleButtonMain.onClick.AddListener(GoogleLogin);
        loginButtonMain.onClick.AddListener(() =>
        {
            mainStartPanel.SetActive(false);
            loginPanel.SetActive(true);
        });
        registerButtonMain.onClick.AddListener(() =>
        {
            mainStartPanel.SetActive(false);
            registerPanel.SetActive(true);
        });

        // Setup listeners for login panel buttons.
        loginButton.onClick.AddListener(LoginUser);
        backButtonLogin.onClick.AddListener(() =>
        {
            loginPanel.SetActive(false);
            mainStartPanel.SetActive(true);
        });

        // Setup listeners for register panel buttons.
        registerButton.onClick.AddListener(RegisterUser);
        backButtonRegister.onClick.AddListener(() =>
        {
            registerPanel.SetActive(false);
            mainStartPanel.SetActive(true);
        });

        // Setup listener for Google username popup.
        confirmGoogleUsernameButton.onClick.AddListener(ConfirmGoogleUsername);

        // Auto-login if already signed in.
        user = auth.CurrentUser;
        if (user != null)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    // REGISTER USER WITH EMAIL & PASSWORD
    public void RegisterUser()
    {
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;
        string username = registerUsernameInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            messageText.text = "Please enter Email, Password, and Username.";
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                messageText.text = "Registration Failed: " + task.Exception.Message;
                return;
            }

            FirebaseUser newUser = auth.CurrentUser;
            if (newUser != null)
            {
                UserProfile profile = new UserProfile { DisplayName = username };
                newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(profileTask =>
                {
                    if (profileTask.IsFaulted)
                    {
                        messageText.text = "Username update failed.";
                    }
                    else
                    {
                        messageText.text = "Registration Successful!";
                        SceneManager.LoadScene("MainMenuScene");
                    }
                });
            }
        });
    }

    // LOGIN USER WITH EMAIL & PASSWORD
    public void LoginUser()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Please enter Email and Password.";
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                messageText.text = "Login Failed: " + task.Exception.Message;
                return;
            }

            FirebaseUser newUser = auth.CurrentUser;
            if (newUser != null)
            {
                messageText.text = "Login Successful!";
                SceneManager.LoadScene("MainMenuScene");
            }
        });
    }

    // GOOGLE SIGN-IN
    public void GoogleLogin()
    {
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };
            isGoogleSignInInitialized = true;
        }
        else
        {
            // Reconfigure if needed.
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };
        }

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                messageText.text = "Google Sign-In Failed.";
                return;
            }

            GoogleSignInUser googleUser = task.Result;
            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsCanceled || authTask.IsFaulted)
                {
                    messageText.text = "Firebase Google Login Failed.";
                    return;
                }

                FirebaseUser newUser = auth.CurrentUser;
                if (newUser != null)
                {
                    if (string.IsNullOrEmpty(newUser.DisplayName))
                    {
                        // If the user is signing in with Google for the first time, prompt for a username.
                        usernamePopup.SetActive(true);
                        messageText.text = "Enter a username to complete registration.";
                    }
                    else
                    {
                        SceneManager.LoadScene("MainMenuScene");
                    }
                }
            });
        });
    }

    // CONFIRM USERNAME FOR FIRST-TIME GOOGLE USERS
    public void ConfirmGoogleUsername()
    {
        string newUsername = googleUsernameInput.text;

        if (string.IsNullOrEmpty(newUsername))
        {
            messageText.text = "Please enter a username.";
            return;
        }

        FirebaseUser currentUser = auth.CurrentUser;
        if (currentUser == null)
        {
            messageText.text = "User not found. Please login again.";
            return;
        }

        UserProfile profile = new UserProfile { DisplayName = newUsername };
        currentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                messageText.text = "Failed to set username.";
                return;
            }
            usernamePopup.SetActive(false);
            SceneManager.LoadScene("MainMenuScene");
        });
    }
}
