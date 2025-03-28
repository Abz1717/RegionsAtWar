using UnityEngine;

public class Province : MonoBehaviour
{
    // Drag the center point GameObject (child of this province) into this field.
    public Transform centerTransform;

    // This property gives you the global position of the center.
    public Vector3 CenterPoint => centerTransform != null ? centerTransform.position : transform.position;
}
