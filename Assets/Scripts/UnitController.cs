
using UnityEngine;
using DG.Tweening;  // Make sure you have DOTween imported
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    // Optionally, reference a UI button that triggers movement.

    // Call this method when instantiating your unit.
    public void Initialize(Transform startPoint)
    {
        transform.position = startPoint.position;
    }

    // Call this method when move is triggered (e.g., via a button press).
    public void MoveAlongPath(List<RegionCapturePoint> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to move along.");
            return;
        }

        // Create a DOTween sequence to chain movements.
        Sequence moveSequence = DOTween.Sequence();
        float moveDuration = 1f;  // Adjust duration per segment as needed

        foreach (RegionCapturePoint region in path)
        {
            // Assuming each RegionCapturePoint’s transform.position is the center of that region.
            moveSequence.Append(transform.DOMove(region.transform.position, moveDuration));
        }

        moveSequence.Play();
    }
}
