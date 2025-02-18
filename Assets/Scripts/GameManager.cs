using UnityEngine;
using System.Collections;  // 🔹 Add this line!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Singleton for easy access

    public enum GameMode { SinglePlayer, Multiplayer }
    public GameMode currentMode = GameMode.SinglePlayer; // Default to Single Player

    public bool isPlayerTurn = true;  // Track turns

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (currentMode == GameMode.SinglePlayer)
        {
            Debug.Log("🔹Single Player Mode Activated");
            StartSinglePlayer();
        }
    }

    void StartSinglePlayer()
    {
        // Initialize game setup for Single Player
        Debug.Log("🎮 Starting Single Player Mode");
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;  // Toggle turns
        Debug.Log(isPlayerTurn ? "🔵 Player's Turn" : "🔴 AI's Turn");

        if (!isPlayerTurn)
        {
            StartCoroutine(AITurn());
        }
    }

    IEnumerator AITurn()
    {
        Debug.Log("🧠 AI Thinking...");
        yield return new WaitForSeconds(2); // Simulate AI thinking

        // AI logic (To be implemented later)
        Debug.Log("🤖 AI took action!");

        EndTurn(); // Back to Player
    }
}
