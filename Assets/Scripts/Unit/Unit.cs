using System;
using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public event Action OnEnemyKilled;

    [Tooltip("Which faction/team this unit belongs to. E.g., 0 = Player, 1 = AI.")]
    public int factionID;

    [Header("Unit Stats")]
    public int maxHealth = 100;
    public int attack = 10;
    public int defense = 5;
    public float moveSpeed = 3f;

    [SerializeField] private Animator animator;

    public int CurrentHealth { get; private set; }

    private Unit Target { get; set; }

    // Flag to indicate if this unit is currently performing its attack animation.
    private bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }

    // Store the last attacker so that when this unit dies, the killer's faction can capture the region.
    private Unit lastAttacker;

    private void Awake()
    {
        // Set the unit's current health to its maximum health when it spawns.
        CurrentHealth = maxHealth;
    }

    /// Reduces the unit's health based on incoming damage adjusted by defense.
    /// If an attacker is provided and this unit isn't already attacking, it immediately counterattacks.
    public bool TakeDamage(int damage)
    {
        int effectiveDamage = Mathf.Max(damage - defense, 0);
        CurrentHealth -= effectiveDamage;
        Debug.Log($"{gameObject.name} took {effectiveDamage} damage, current health: {CurrentHealth}");


        // If this unit is currently selected and the UnitActionPanel is open, refresh it.
        if (GameManager.Instance.SelectedUnit == this.GetComponent<UnitController>() &&
            MaproomUIManager.Instance.IsUnitActionPanelOpen)
        {
            MaproomUIManager.Instance.UnitActionPanel.Show();
        }
        if (CurrentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    /// Attacks the target unit by dealing damage equal to this unit's attack stat.
    /// The attack animation is played and the attack flag remains active until the animation finishes.
    public void Attack()
    {
        if (Target == null)
        {
            Debug.LogError("No target to attack!");
            return;
        }
        animator.Play("Attack");
        Debug.Log($"{gameObject.name} is attacking {Target.gameObject.name} for {attack} damage!");

        // Deal damage to the target.
        bool isKilled = Target.TakeDamage(attack);
        if (isKilled)
        {
            Reset();
            OnEnemyKilled?.Invoke();
        }
    }

    public void Walk()
    {
        animator.Play("Walk");
    }

    public void Reset()
    {
        animator.Play("Idle");
    }



    private void CaptureCurrentRegion()
    {
        RegionCapturePoint currentRegion = RegionManager.Instance.GetCurrentRegion(transform.position);
        if (currentRegion != null)
        {
            // Only capture if the region isn't contested.
            if (!currentRegion.IsContested())
            {
                // Capture the region with this unit's faction.
                currentRegion.region.SetOwner(factionID);
                Debug.Log($"{gameObject.name} has captured region {currentRegion.name}!");
            }
            else
            {
                Debug.Log($"{currentRegion.name} is contested. Capture skipped.");
            }
        }
        else
        {
            Debug.LogWarning("No region found at the unit's position.");
        }
    }


    /// Handles unit death.
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");


        // Only capture the region if there's a recorded attacker.
        if (lastAttacker != null)
        {
            RegionCapturePoint currentRegion = RegionManager.Instance.GetCurrentRegion(transform.position);
            if (currentRegion != null && !currentRegion.IsContested())
            {
                currentRegion.region.SetOwner(lastAttacker.factionID);
                Debug.Log($"{lastAttacker.gameObject.name} captured region {currentRegion.name} after killing {gameObject.name}!");
            }
        }

        animator.speed = 0;

        // If this unit is the one currently selected, close its panel
        var dyingUnitController = GetComponent<UnitController>();
        if (GameManager.Instance != null &&
            GameManager.Instance.SelectedUnit == dyingUnitController)
        {
            MaproomUIManager.Instance.CloseUnitActionPanel();
        }

        // Update enemy visibility if needed.
        if (GameManager.Instance != null && GameManager.Instance.SelectedUnit != null)
        {
            UnitManager.Instance.UpdateEnemyUnitVisibility(GameManager.Instance.SelectedUnit.transform.position);
        }

        // Additional death logic can be added here.
        Destroy(gameObject);


    }

    public void StartAttack(Unit enemy)
    {
       StartCoroutine(StartAttackDelay(enemy));
    }

    private IEnumerator StartAttackDelay(Unit enemy)
    {
        Target = enemy;
        var delay = UnityEngine.Random.Range(0f, 1f);
        yield return new WaitForSeconds(delay);
        animator.Play("Attack");
    }
}
