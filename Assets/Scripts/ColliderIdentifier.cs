using UnityEngine;

public enum ColliderRole
{
    Melee,
    Range
}

public class ColliderIdentifier : MonoBehaviour
{
    public ColliderRole role;
}

