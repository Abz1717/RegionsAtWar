using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting.FullSerializer;

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

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;


    private void Awake()
    {
        capturePoint = GetComponentInChildren<RegionCapturePoint>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = Color.white;
    }

    public void SetOwner(int id)
    {
        ownerID = id;
        //TODO show owner
        UpdateOwnerVisual();
        GameManager.Instance.CheckGameEnd();
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void ResetColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }


    // to get tints
    private void UpdateOwnerVisual()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameConfig != null)
        {
            var config = GameManager.Instance.gameConfig;
            if (ownerID >= 0)
            {
                originalColor = GameManager.Instance.gameConfig.Players.Find(p => p.Id == ownerID).color;
            }
            else
            {
                originalColor = Color.white; // Default color for invalid ownerID.
            }
            ResetColor();
        }
    }
}
