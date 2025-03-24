using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting.FullSerializer;
using System.Collections;

public class Region : MonoBehaviour
{
    [SerializeField]
    private RegionCapturePoint capturePoint;

    public RegionCapturePoint CapturePoint => capturePoint;

    [Header("Region Settings")]
    public string regionID = "Region_1";
    public int ownerID = -1;

    [Header("Production Rates (per interval)")]
    public int moneyRate = 10;
    public int manpowerRate = 0;
    public int resourceRate = 10;
    public Resource resourseType;
    public float productionTime = 1f;

    [Header("Neighbors & Movement")]
    public List<Region> neighbors = new List<Region>();
    public Transform centerPoint;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public List<BuildingData> buildings = new();


    private void Awake()
    {
        capturePoint = GetComponentInChildren<RegionCapturePoint>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = Color.white;


        StartProduction(Resource.Money, moneyRate);
        StartProduction(Resource.Manpower, manpowerRate);
        StartProduction(resourseType, resourceRate);
    }

    private void StartProduction(Resource resourseType, int resourceRate)
    {
        ServiceCoroutine.Instance.RunCoroutine(Production(resourseType, resourceRate));
    }

    public void SetOwner(int id)
    {
        ownerID = id;
        //TODO show owner
        UpdateOwnerVisual();
        GameManager.Instance.CheckGameEnd();

        if (MaproomUIManager.Instance != null)
        {
            MaproomUIManager.Instance.UpdateScoreUI();
        }
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

    public void ConstructBuilding(BuildingData building)
    {
        buildings.Add(building);
    }


    // to get tints
    private void UpdateOwnerVisual()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameConfig != null)
        {
            var config = GameManager.Instance.gameConfig;
            if (ownerID >= 0)
            {
                // change players colours
                originalColor = GameManager.Instance.gameConfig.Players.Find(p => p.Id == ownerID).color == PlayerColor.Green ? new Color(1.5f, 1f, 0f, 1f) : new Color(0.8f, 1f, 0.8f, 1f);
            }
            else
            {
                originalColor = Color.white; // Default color for invalid ownerID.
            }
            ResetColor();
        }
    }

    private IEnumerator Production(Resource resource, int ammount)
    {
        while (true)
        {
            yield return new WaitForSeconds(productionTime);
            if(ownerID >= 0)
            {
                GameManager.Instance.AddPlayerResource(ownerID, resource, ammount);
            }
        }
    }
}
