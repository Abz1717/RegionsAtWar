
using UnityEngine;
using DG.Tweening;  // Make sure you have DOTween imported
using System.Collections.Generic;
using System;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private Transform pivot;
    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private float speed = 1.0f;

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
                .OnComplete(() =>
                {
                    if (!region.IsContested())  // You need to implement IsContested() accordingly.
                    {
                        region.region.SetOwner(unit.factionID);
                    }
                });
            moveSequence.Append(pivot.DOLookAt(targetPos, 0.2f, AxisConstraint.Y));
            moveSequence.Append(tween);
        }

        tweener = moveSequence;
        moveSequence.Play().OnComplete(() => unit.Reset());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.gameObject.GetComponent<Unit>();
        if(enemy != null && enemy.factionID != unit.factionID)
        {
            if(tweener != null && !tweener.IsComplete())
            {
                tweener.Pause();
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




