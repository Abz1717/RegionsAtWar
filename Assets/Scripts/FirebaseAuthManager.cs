using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Google;
using System.Threading.Tasks;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    private FirebaseAuth auth;
    private FirebaseUser user;

    [Header("UI References")]
    public GameObject loginUI;       // Login Page
    public GameObject registerUI;    // Register Page
    public GameObject usernamePopup; // Google First-Time Username Prompt

    public TMP_InputField googleUsernameInput;   // For Google username

    [Header("Login UI References")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;

    [Header("Register UI References")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerUsernameInput;


    public TextMeshProUGUI messageText;
    public Button registerButton;
    public Button loginButton;
    public Button googleLoginButton;
    public Button backButton;
    public Button confirmGoogleUsernameButton;

    [Header("Google Sign-In")]
    public string GoogleAPI = "YOUR_GOOGLE_CLIENT_ID"; // Replace with your Google OAuth Client ID
    private GoogleSignInConfiguration configuration;
    private bool isGoogleSignInInitialized = false;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        // Button Listeners
        registerButton.onClick.AddListener(RegisterUser);
        loginButton.onClick.AddListener(LoginUser);
        googleLoginButton.onClick.AddListener(GoogleLogin);
        backButton.onClick.AddListener(ShowLoginUI);
        confirmGoogleUsernameButton.onClick.AddListener(ConfirmGoogleUsername);

        loginUI.SetActive(true);
        registerUI.SetActive(false);
        usernamePopup.SetActive(false);

        user = auth.CurrentUser;
        if (user != null)
        {
            SceneManager.LoadScene("MainMenuScene"); // Auto-login if already signed in
        }
    }

    // SHOW REGISTER UI
    public void ShowRegisterUI()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    // SHOW LOGIN UI
    public void ShowLoginUI()
    {
        registerUI.SetActive(false);
        loginUI.SetActive(true);
    }

    // REGISTER USER WITH EMAIL & PASSWORD
    public void RegisterUser()
    {
        Debug.Log("✅ RegisterUser() function was called!"); // Debugging

        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;
        string username = registerUsernameInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("⚠️ Missing Fields: Email, Password, or Username is empty.");
            messageText.text = "Please enter Email, Password, and Username.";
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("❌ Registration Failed: " + task.Exception);
                messageText.text = "Registration Failed: " + task.Exception.Message;
                return;
            }

            FirebaseUser newUser = auth.CurrentUser;
            if (newUser != null)
            {
                Debug.Log("✅ User Created: " + newUser.Email);

                UserProfile profile = new UserProfile { DisplayName = username };
                newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(profileTask =>
                {
                    if (profileTask.IsFaulted)
                    {
                        Debug.LogError("❌ Username update failed.");
                        messageText.text = "Username update failed.";
                    }
                    else
                    {
                        Debug.Log("✅ Registration Successful! Redirecting to Main Menu.");
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

            FirebaseAuth authInstance = FirebaseAuth.DefaultInstance;
            FirebaseUser newUser = authInstance.CurrentUser;

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

                FirebaseAuth authInstance = FirebaseAuth.DefaultInstance;
                FirebaseUser newUser = authInstance.CurrentUser;

                if (newUser != null)
                {
                    if (string.IsNullOrEmpty(newUser.DisplayName))
                    {
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

        FirebaseAuth authInstance = FirebaseAuth.DefaultInstance;
        FirebaseUser currentUser = authInstance.CurrentUser;

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
