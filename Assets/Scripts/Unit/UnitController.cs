
using UnityEngine;
using DG.Tweening;  // Make sure you have DOTween imported
using System.Collections.Generic;
using System;
using System.Collections;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private Transform pivot;
    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private float speed = 1.0f;

    public Unit Unit => unit;


    private Sequence tweener;
    // Optionally, reference a UI button that triggers movement.

    // Call this method when instantiating your unit.
    public void Initialize(Transform startPoint)
    {
        transform.position = startPoint.position;
        Vector3 newPos = transform.position;

        newPos.z = 0f;
        transform.position = newPos;


        unit.OnEnemyKilled += ProcessEnemyKilled;
    }

    private void ProcessEnemyKilled()
    {
        if (tweener != null && !tweener.IsComplete() && !tweener.IsPlaying())
        {
            tweener.TogglePause();
        }
    }

    public void SetColor(Color color)
    {
        mesh.material.color = color;
    }

    public void SetFaction(int faction)
    {
        unit.factionID = faction;
        SetColor(GameManager.Instance.gameConfig.Players.Find(p => p.Id == faction).color);
    }

    // Call this method when move is triggered (e.g., via a button press).
    public void MoveAlongPath(List<RegionCapturePoint> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to move along.");
            return;
        }
        // Remove the starting region since unit is already there
        path.RemoveAt(0);

        unit.Walk();

        // Create a DOTween sequence to chain movements.
        Sequence moveSequence = DOTween.Sequence();

        foreach (RegionCapturePoint region in path)
        {
            // Get the region's position, but override its z coordinate
            Vector3 targetPos = region.transform.position;
            targetPos.z = 0f;  // or your desired constant value

            var distance = targetPos - transform.position;

            // Assuming each RegionCapturePoint’s transform.position is the center of that region.
            var tween = transform.DOMove(targetPos, distance.magnitude / unit.moveSpeed).SetEase(Ease.Linear)
                // Update enemy visibility continuously during the movement.
                .OnUpdate(() =>
                {
                UnitManager.Instance.UpdateEnemyUnitVisibility(transform.position);
                })
                .OnComplete(() =>
                {
                    // When reaching this region, set its owner if it's not contested.
                    if (!region.IsContested())  
                    {
                        region.region.SetOwner(unit.factionID);
                    }
                });
            moveSequence.Append(pivot.DOLookAt(targetPos, 0.2f, AxisConstraint.Y));
            moveSequence.Append(tween);
        }

        tweener = moveSequence;
        moveSequence.Play().OnComplete(() =>
        {
            unit.Reset();
            Debug.Log("Move complete. New position: " + transform.position);
            StartCoroutine(DelayedEnemyVisibilityUpdate());
        });
    }

    private IEnumerator DelayedEnemyVisibilityUpdate()
    {
        yield return null; // waiting one frame
        UnitManager.Instance.UpdateEnemyUnitVisibility(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.gameObject.GetComponent<Unit>();

        // Declare enemyController here:
        var enemyController = collision.gameObject.GetComponent<UnitController>();
        if (enemy != null && enemy.factionID != unit.factionID)
        {
            if (enemy != null && enemy.factionID != unit.factionID)
            {
            
                if (tweener != null && !tweener.IsComplete())
                {
                tweener.Pause();
                }


                // Force the enemy to remain visible for 60 seconds.
                if (enemyController != null)
                {
                    UnitManager.Instance.ForceEnemyVisibility(enemyController, 60f);
                }

                unit.StartAttack(enemy);
                pivot.DOLookAt(enemy.transform.position+ Vector3.back*10, 0.2f, AxisConstraint.Y);
                //Vector3.up
                //Vector3.down
                //Vector3.right
                //Vector3.left
     

            }
        }

    }
}




