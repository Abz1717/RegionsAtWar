using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    // Store the currently selected country.
    public string selectedCountry = "";

    // List of available countries to choose from.
    public List<string> availableCountries = new List<string> { "Britain", "France" };

    // List of country panel Image components, assigned via the Inspector.
    public List<Image> countryPanelImages = new List<Image>();

    // Colors for the panel states.
    // Set normalColor to A2A2A2 (RGB 162, 162, 162)
    public Color normalColor = new Color32(162, 162, 162, 255);
    public Color selectedColor = Color.green;

    // Reference to the last selected panel so we can reset it.
    private Image lastSelectedPanel = null;

    // Flag to ensure the player can choose only once.
    private bool hasChosen = false;

    /// <summary>
    /// Called when a country panel is clicked.
    /// It stores the selected country, updates the UI (i.e., panel highlighting),
    /// and then locks the choice so that further selections are ignored.
    /// </summary>
    /// <param name="country">The chosen country/power.</param>
    public void SelectCountry(string country)
    {
        // If the player has already chosen a region, ignore further selections.
        if (hasChosen)
        {
            Debug.Log("🟢 [SelectCountry] Country already chosen, cannot change selection.");
            return;
        }

        Debug.Log($"🟢 [SelectCountry] Method called with parameter: {country}");
        selectedCountry = country;
        Debug.Log($"🟢 [SelectCountry] Selected country is now: {selectedCountry}");

        // Get the panel that was clicked.
        GameObject clickedPanel = EventSystem.current.currentSelectedGameObject;
        if (clickedPanel != null)
        {
            // Try to get the Image component on the clicked panel.
            Image clickedImage = clickedPanel.GetComponent<Image>();
            if (clickedImage != null)
            {
                // If another panel was previously selected, reset its color.
                if (lastSelectedPanel != null && lastSelectedPanel != clickedImage)
                {
                    lastSelectedPanel.color = normalColor;
                }

                // Highlight the clicked panel.
                clickedImage.color = selectedColor;
                lastSelectedPanel = clickedImage;
                Debug.Log("🟢 [SelectCountry] Panel highlighted.");
            }
            else
            {
                Debug.LogWarning("⚠ [SelectCountry] Clicked panel does not have an Image component.");
            }
        }
        else
        {
            Debug.LogWarning("⚠ [SelectCountry] No panel detected as clicked.");
        }

        // Lock in the choice.
        hasChosen = true;
    }

    /// <summary>
    /// Called by the Start button.
    /// Proceeds with the current selection if one is made.
    /// </summary>
    public void OnStartButtonPressed()
    {
        Debug.Log("🟢 [OnStartButtonPressed] Method called.");

        if (string.IsNullOrEmpty(selectedCountry))
        {
            Debug.LogError("❌ [OnStartButtonPressed] No country selected! Please select a country first.");
            return;
        }

        Debug.Log($"🟢 [OnStartButtonPressed] Proceeding with country: {selectedCountry}");
        OnCountrySelected(selectedCountry);
    }

    /// <summary>
    /// Called by the Random button.
    /// Randomly picks a country from the available list and proceeds.
    /// This locks the choice if none has been made yet.
    /// </summary>
    public void OnRandomButtonPressed()
    {
        Debug.Log("🟢 [OnRandomButtonPressed] Method called.");

        // If a selection is already locked in, ignore random selection.
        if (hasChosen)
        {
            Debug.Log("🟢 [OnRandomButtonPressed] Country already chosen, ignoring random selection.");
            return;
        }

        if (availableCountries.Count == 0)
        {
            Debug.LogError("❌ [OnRandomButtonPressed] No available countries defined!");
            return;
        }

        int index = Random.Range(0, availableCountries.Count);
        selectedCountry = availableCountries[index];
        Debug.Log($"🟢 [OnRandomButtonPressed] Randomly selected country: {selectedCountry}");

        // Reset all panels to normal color.
        foreach (Image img in countryPanelImages)
        {
            if (img != null)
            {
                img.color = normalColor;
            }
        }

        // Lock in the random selection.
        hasChosen = true;

        OnCountrySelected(selectedCountry);
    }

    /// <summary>
    /// This method updates Firebase with the selected country and then loads the game scene.
    /// </summary>
    /// <param name="selectedCountry">The country selected (either manually or randomly).</param>
    public void OnCountrySelected(string selectedCountry)
    {
        Debug.Log($"🟢 [OnCountrySelected] Method called with parameter: {selectedCountry}");

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("❌ [OnCountrySelected] User not authenticated!");
            return;
        }

        string userId = user.UserId;
        string gameID = GameSessionManager.CurrentGameID; // retrieved from our persistent session manager

        Debug.Log($"🟢 [OnCountrySelected] About to update Firebase for user {userId}, game {gameID}");

        // Update the player's selected country in the Firebase game session.
        DatabaseReference countryRef = FirebaseDatabase.DefaultInstance
            .GetReference($"game_sessions/{gameID}/players/{userId}/selectedCountry");

        countryRef.SetValueAsync(selectedCountry).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ [OnCountrySelected] Error saving selected country: " + task.Exception);
                return;
            }
            Debug.Log("✅ [OnCountrySelected] Country selection saved. Loading game scene...");
            SceneManager.LoadScene("GameScene");
        });
    }

    /// <summary>
    /// Called by the Back button.
    /// Returns the user to the Main Menu scene.
    /// </summary>
    public void OnBackButtonPressed()
    {
        Debug.Log("🟢 [OnBackButtonPressed] Loading MainMenuScene...");
        SceneManager.LoadScene("MainMenuScene");
    }
}
