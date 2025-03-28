using UnityEngine;

public class DebugTransformChecker : MonoBehaviour
{
    // Set thresholds for what you consider "huge"
    public float positionThreshold = 10000f;
    public float scaleThreshold = 1000f;

    private void Start()
    {
        CheckAllTransforms();
        CheckAllColliders();
    }

    void CheckAllTransforms()
    {
        // Get all transforms in the scene
        Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>();
        foreach (Transform t in allTransforms)
        {
            // Check for NaN in position
            if (float.IsNaN(t.position.x) || float.IsNaN(t.position.y) || float.IsNaN(t.position.z))
            {
                Debug.LogWarning($"GameObject '{t.gameObject.name}' has a NaN position: {t.position}");
            }
            // Check for position magnitude
            if (t.position.magnitude > positionThreshold)
            {
                Debug.LogWarning($"GameObject '{t.gameObject.name}' has a huge position: {t.position}");
            }
            // Check for NaN in scale
            if (float.IsNaN(t.localScale.x) || float.IsNaN(t.localScale.y) || float.IsNaN(t.localScale.z))
            {
                Debug.LogWarning($"GameObject '{t.gameObject.name}' has a NaN localScale: {t.localScale}");
            }
            // Check for scale magnitude
            if (t.localScale.magnitude > scaleThreshold)
            {
                Debug.LogWarning($"GameObject '{t.gameObject.name}' has a huge localScale: {t.localScale}");
            }
        }
    }

    void CheckAllColliders()
    {
        // Check for 2D Colliders
        Collider2D[] colliders2D = GameObject.FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in colliders2D)
        {
            if (col.bounds.extents.magnitude > positionThreshold)
            {
                Debug.LogWarning($"2D Collider on '{col.gameObject.name}' has huge bounds: {col.bounds}");
            }
        }

        // Check for 3D Colliders
        Collider[] colliders3D = GameObject.FindObjectsOfType<Collider>();
        foreach (Collider col in colliders3D)
        {
            if (col.bounds.extents.magnitude > positionThreshold)
            {
                Debug.LogWarning($"3D Collider on '{col.gameObject.name}' has huge bounds: {col.bounds}");
            }
        }
    }
}
