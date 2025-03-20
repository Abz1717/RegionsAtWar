using UnityEngine;
using System.Collections.Generic;
using System;

public class Region : MonoBehaviour
{
    [SerializeField]
    private RegionCapturePoint capturePoint;

    public RegionCapturePoint CapturePoint => capturePoint;

    [Header("Region Settings")]
    public string regionID = "Region_1";
    public int ownerID = -1;

    [Header("Production Rates (per interval)")]
    public int resourceRate = 10;
    public int moneyRate = 0;
    public int manpowerRate = 0;
    public int resource1Rate = 0;
    public int resource2Rate = 0;
    public int resource3Rate = 0;

    [Header("Neighbors & Movement")]
    public List<Region> neighbors = new List<Region>();
    public Transform centerPoint;

    private void Awake()
    {
        capturePoint = GetComponentInChildren<RegionCapturePoint>();
    }

    public void SetOwner(int id)
    {
        ownerID = id;
        //TODO show owner
    }
}
