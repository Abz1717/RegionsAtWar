using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void LogoutUser()
    {

        Debug.Log("Logout button pressed");

        if (auth != null)
        {
            auth.SignOut(); // Logs out the user
            SceneManager.LoadScene("LoginScene"); // Redirect back to login scene
        }
    }
}
