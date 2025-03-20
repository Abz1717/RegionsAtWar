using UnityEngine;

public class Unit : MonoBehaviour
{
    [Tooltip("Which faction/team this unit belongs to. E.g., 1 = Player, 2 = AI.")]
    public int factionID;


    [Header("Unit Stats")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attack = 10;
    public int defense = 5;
    public float moveSpeed = 3f;

    [SerializeField] private Animator animator;


    private void Awake()
    {
        // Set the unit's current health to its maximum health when it spawns.
        currentHealth = maxHealth;
    }

    /// Reduces the unit's health based on incoming damage adjusted by defense.
    public void TakeDamage(int damage)
    {
        int effectiveDamage = Mathf.Max(damage - defense, 0);
        currentHealth -= effectiveDamage;
        Debug.Log($"{gameObject.name} took {effectiveDamage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// Attacks a target unit by dealing damage equal to this unit's attack stat.
    public void Attack(Unit target)
    {
        if (target == null)
        {
            Debug.LogError("No target to attack!");
            return;
        }
        Debug.Log($"{gameObject.name} is attacking {target.gameObject.name} for {attack} damage!");
        target.TakeDamage(attack);
        animator.Play("Attack");
    }

    public void Walk()
    {
        animator.Play("Walk");
    }

    public void Reset()
    {
        animator.Play("Idle");
    }

    /// Handles unit death.
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // ADD additional death logic here (animation, effects, etc)
        Destroy(gameObject);
    }
}
