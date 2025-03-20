using System.Collections.Generic;
using UnityEngine;

public class RegionCapturePoint : MonoBehaviour
{
    [Tooltip("Reference to the Region data for this capture point.")]
    public Region region;

    private List<Unit> unitsInRegion = new List<Unit>();


    private void Awake()
    {

        if (region == null)
        {
            // Assuming the hierarchy: Region (parent) -> Centerpoint (child) -> CenterpointCollider (this GameObject)
            region = transform.parent.parent.GetComponent<Region>();
        }
    }



    /// <summary>
    /// Returns true if there are units from more than one faction inside the capture point.
    /// </summary>
    public bool IsContested()
    {
        if (unitsInRegion.Count == 0)
            return false;

        // Assume the faction of the first unit is the baseline.
        int baseFaction = unitsInRegion[0].factionID;

        // If any unit belongs to a different faction, the region is contested.
        foreach (var unit in unitsInRegion)
        {
            if (unit.factionID != baseFaction)
                return true;
        }
        return false;
    }
}

