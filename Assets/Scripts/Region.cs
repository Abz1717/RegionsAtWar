using UnityEngine;
using System.Collections.Generic;

public class Region : MonoBehaviour
{
    [Header("Region Settings")]
    [Tooltip("Unique name or ID for this region.")]
    public string regionID = "Region_1";

    [Tooltip("Which player/AI owns this region? 0 = Neutral, 1 = Player, 2 = AI")]
    public int ownerID = 0;

    [Tooltip("Resources produced per interval (e.g., per second).")]
    public int resourceRate = 10;

    [Tooltip("List of adjacent/neighboring regions.")]
    public List<Region> neighbors = new List<Region>();

    [Tooltip("The child centerpoint that units should move to for capture.")]
    public Transform centerPoint;
}
