using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegionManager : Singleton<RegionManager>
{
    [SerializeField] public List<RegionCapturePoint> regions = new List<RegionCapturePoint>();
    [SerializeField] public List<Region> regionData = new List<Region>();

    public RegionCapturePoint GetPoint(Region region)
    {
        return regions.Find(r => r.region.regionID == region.regionID);
    }

    public List<RegionCapturePoint> GetPath(RegionCapturePoint startRegion, RegionCapturePoint destinationRegion)
    {
        if (destinationRegion == null)
        {
            Debug.LogWarning("Destination region is null");
            return new List<RegionCapturePoint>();
        }

        var result = GetPointPath(startRegion, new List<RegionCapturePoint>(), 0, regions.Count, destinationRegion);
        return result.Points;
    }

    // Updated recursive pathfinder with destination parameter.
    private SearchPathResult GetPointPath(RegionCapturePoint point, List<RegionCapturePoint> path, int depth, int allowedDepth, RegionCapturePoint destination)
    {
        depth++;
        bool pointFound = false;
        if (depth - 1 > allowedDepth)
            return new SearchPathResult() { Depth = depth, Points = path };

        var newPath = new List<RegionCapturePoint>(path);
        newPath.Add(point);
        var returnPaths = new List<List<RegionCapturePoint>>();

        foreach (var neighbor in point.region.neighbors)
        {
            if (!newPath.Contains(neighbor.CapturePoint))
            {
                if (!neighbor.CapturePoint.Equals(destination))
                {
                    var result = GetPointPath(neighbor.CapturePoint, newPath, depth, allowedDepth, destination);
                    if (result != null)
                    {
                        if (result.Depth < allowedDepth)
                            allowedDepth = result.Depth;
                        if (result.PointFound)
                        {
                            returnPaths.Add(result.Points);
                            pointFound = true;
                        }
                    }
                }
                else
                {
                    newPath.Add(neighbor.CapturePoint);
                    return new SearchPathResult() { Depth = depth, Points = newPath, PointFound = true };
                }
            }
        }

        List<RegionCapturePoint> minPath = null;
        if (pointFound)
        {
            foreach (var createdPath in returnPaths)
            {
                if (minPath == null || minPath.Count > createdPath.Count)
                    minPath = createdPath;
            }
        }
        if (minPath == null)
            minPath = newPath;

        return new SearchPathResult() { Depth = allowedDepth, Points = minPath, PointFound = pointFound };
    }

    /*
    public List<RegionCapturePoint> GetPath(RegionCapturePoint startRegion)
    {
        if (endRegion == null)
        {
            return new List<RegionCapturePoint>();
        }

        var result  = GetPointPath(startRegion, new List<RegionCapturePoint>(),0,regions.Count);
        return result.Points;

    }

    private SearchPathResult GetPointPath(RegionCapturePoint point, List<RegionCapturePoint> path,int depth, int allowedDepth)
    {
        depth++;
        bool pointFound = false;
        if (depth-1 > allowedDepth)
            return new SearchPathResult() {Depth = depth, Points = path};
        var newPath = new List<RegionCapturePoint>();
        newPath.AddRange(path);
        newPath.Add(point);
        var returnPaths = new List<List<RegionCapturePoint>>();
        foreach (var neigboor in point.region.neighbors)
        {
            if (!newPath.Contains(neigboor.CapturePoint))
            {
                if(!neigboor.CapturePoint.Equals(endRegion))
                {
                    var result = GetPointPath(neigboor.CapturePoint, newPath,depth, allowedDepth);
                    if (result != null)
                    {
                        if (result.Depth < allowedDepth)
                            allowedDepth = result.Depth;
                        if (result.PointFound)
                        {
                            returnPaths.Add(result.Points);
                            pointFound = true;

                        }
                    }
                }
                else
                {
                    newPath.Add(neigboor.CapturePoint);
                    return new SearchPathResult() { Depth = depth, Points = newPath, PointFound = true };
                }
            }
        }

        List<RegionCapturePoint> minPath = null;

        if(pointFound)
        {

            foreach (var createdPath in returnPaths)
            {
                if (minPath == null || minPath.Count > createdPath.Count)
                {
                    minPath = createdPath;
                }
            }
        }

        if (minPath == null)
            minPath = newPath;

        return new SearchPathResult() { Depth = allowedDepth, Points = minPath, PointFound = pointFound };

    }

    */

    public void Reset()
    {
        foreach (var region in regionData)
        { 
            region.SetOwner(-1);
        }
    }

    private class SearchPathResult
    {
        public List<RegionCapturePoint> Points;
        public int Depth;
        public bool PointFound = false;
    }


    public RegionCapturePoint GetCurrentRegion(Vector3 unitPosition)
    {
        RegionCapturePoint closestRegion = null;
        float shortestDistance = Mathf.Infinity;

        foreach (RegionCapturePoint region in regions)
        {
            float distance = Vector3.Distance(unitPosition, region.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestRegion = region;
            }
        }
        return closestRegion;
    }

}
