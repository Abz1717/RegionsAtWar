using UnityEngine;

public class Unit : MonoBehaviour
{
    [Tooltip("Which faction/team this unit belongs to. E.g., 1 = Player, 2 = AI.")]
    public int factionID;

    /*
    [Tooltip("Movement speed of the unit.")]
    public float moveSpeed = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Optionally set rb to kinematic if you don't want physics forces:
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        // Simple WASD movement (if this is a player-controlled unit)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Move the unit
        Vector2 movement = new Vector2(h, v) * moveSpeed;
        rb.velocity = movement;
    }*/
}
