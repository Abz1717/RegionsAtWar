using System.Collections.Generic;
using UnityEngine;

public class RoadManager : Singleton<RoadManager>
{
    [SerializeField] private List<Road> roads;

    public Road GetRoad(string startRegionID, string endRegionID)
    {
        Debug.Log("ROAD   " + startRegionID + "   " + endRegionID);
        var road = roads.Find(road => (road.startRegion.regionID.Equals(startRegionID) && road.endRegion.regionID.Equals(endRegionID)) || 
        (road.startRegion.regionID.Equals(endRegionID) && road.endRegion.regionID.Equals(startRegionID)));
        Debug.Log("ROAD FOUND " + road.startRegion.regionID + "  " +  road.endRegion.regionID);
        return road;
    }
}
