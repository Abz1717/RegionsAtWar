
using UnityEngine;
using DG.Tweening;  // Make sure you have DOTween imported
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private Transform pivot;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private float speed = 1.0f;
    // Optionally, reference a UI button that triggers movement.

    // Call this method when instantiating your unit.
    public void Initialize(Transform startPoint)
    {
        transform.position = startPoint.position;
        Vector3 newPos = transform.position;

        newPos.z = 0f;
        transform.position = newPos;
    }

    public void SetColor(Color color)
    {
        mesh.material.color = color;
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
        float moveDuration = 1f;  // Adjust duration per segment as needed

        foreach (RegionCapturePoint region in path)
        {
            // Get the region's position, but override its z coordinate
            Vector3 targetPos = region.transform.position;
            targetPos.z = 0f;  // or your desired constant value

            var distance = targetPos - transform.position;

            // Assuming each RegionCapturePoint’s transform.position is the center of that region.
            moveSequence.Append(pivot.DOLookAt(targetPos, 0.2f, AxisConstraint.Y));
            moveSequence.Append(transform.DOMove(targetPos, moveDuration));
        }

        moveSequence.Play().OnComplete(() => unit.Reset());
    }
}




