using UnityEngine;

public class RegionSelector : MonoBehaviour
{
    private RegionCapturePoint regionCapturePoint;

    private void Awake()
    {
        // Ensure the same GameObject has the RegionCapturePoint component.
        regionCapturePoint = GetComponent<RegionCapturePoint>();
        if (regionCapturePoint == null)
        {
            Debug.LogError("RegionSelector: No RegionCapturePoint component found on " + gameObject.name);
        }
    }

    private void OnMouseDown()
    {
        // Check if the game is in move mode.
        if (GameManager.Instance != null && GameManager.Instance.IsMoveModeActive)
        {
            // Set this region as the destination.
            GameManager.Instance.SetDestination(regionCapturePoint);
        }
    }
}
