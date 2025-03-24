
using UnityEngine;
using DG.Tweening;  // Make sure you have DOTween imported
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor;
public enum PlayerColor
{
    Green, Blue
}

[Serializable]
public class UnitTexture
{
    public PlayerColor color;
    public Texture2D texture;
}

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private Transform pivot;
    [SerializeField] private List<SkinnedMeshRenderer> meshes;

    [SerializeField] private List<UnitTexture> textures;

    public Unit Unit => unit;

    public string CustomName { get; private set; }
    public void SetCustomName(string newName)
    {
        CustomName = newName;
    }

    public RegionCapturePoint CurrentPoint { get; private set; }
    public UnitData Data { get; private set; }

    public enum UnitState { Idle, Moving, Attacking }
    public UnitState CurrentState { get; private set; } = UnitState.Idle;



    private Sequence tweener;
    // Optionally, reference a UI button that triggers movement.


    // Call this method when instantiating your unit.
    public void Initialize(RegionCapturePoint startPoint, UnitData data)
    {
        transform.position = startPoint.transform.position;
        Vector3 newPos = transform.position;

        newPos.z = 0f;
        transform.position = newPos;

        SetRegion(startPoint);
        Data = data;


        unit.OnEnemyKilled += ProcessEnemyKilled;
    }

    private void SetRegion(RegionCapturePoint regionPoint)
    {
        CurrentPoint = regionPoint;
        unit.region = regionPoint.region;
    }


    private void ProcessEnemyKilled()
    {
        if (tweener != null && !tweener.IsComplete() && !tweener.IsPlaying())
        {
            tweener.TogglePause();
        }
    }

    public void SetColor(PlayerColor color)
    {
        foreach (var mesh in meshes)
        {
            mesh.material.mainTexture = textures.Find(texture => texture.color == color).texture;
        }
    }

    public void SetFaction(int faction)
    {
        unit.factionID = faction;
        SetColor(GameManager.Instance.gameConfig.Players.Find(p => p.Id == faction).color);
    }

    // Call this method when move is triggered (e.g., via a button press).
    public void MoveAlongPath(List<RegionCapturePoint> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to move along.");
            return;
        }
        // Remove the starting region since unit is already there

        CurrentState = UnitState.Moving;

        unit.Walk();

        // Create a DOTween sequence to chain movements.
        Sequence moveSequence = DOTween.Sequence();

        List<Road> roads = new();

        for (int i = 0; i < path.Count - 1; i++)
        {
            var currentRegion = path[i];
            var region = path[i + 1];
            var road = RoadManager.Instance.GetRoad(path[i].region.regionID, path[i+1].region.regionID);

            if (road != null)
            {
                road.SetSelectable(true);
                roads.Add(road);
            }
            else
            {
                Debug.LogError($"No road found between region {currentRegion.region.regionID} and {region.region.regionID}");
            }
        }

        for (int i =1; i < path.Count; i++)
        {
            var region = path[i];

            // Get the region's position, but override its z coordinate
            Vector3 targetPos = region.transform.position;
            targetPos.z = 0f;  // or your desired constant value

            var distance = targetPos - transform.position;
            var road = roads[i - 1];

            // Assuming each RegionCapturePoint’s transform.position is the center of that region.
            var tween = transform.DOMove(targetPos, distance.magnitude / unit.moveSpeed).SetEase(Ease.Linear)
                // Update enemy visibility continuously during the movement.
                .OnUpdate(() =>
                {
                    SetRegion(region);
                    UnitManager.Instance.UpdateEnemyUnitVisibility(transform.position);
                })
                .OnComplete(() =>
                {
                    road.SetSelectable(false);
                    // When reaching this region, set its owner if it's not contested.
                    if (!region.IsContested())
                    {
                        region.region.SetOwner(unit.factionID);

                    }
                });
            moveSequence.Append(pivot.DOLookAt(targetPos, 0.2f, AxisConstraint.Y));
            moveSequence.Append(tween);
        }

        tweener = moveSequence;
        moveSequence.Play().OnComplete(() =>
        {
            // At the end of the movement, capture the destination region.
            RegionCapturePoint finalRegion = path[path.Count - 1];
            if (!finalRegion.IsContested())
            {
                finalRegion.region.SetOwner(unit.factionID);

                // If the region action panel is open, refresh it.
                if (MaproomUIManager.Instance.regionActionPanel.gameObject.activeSelf)
                {
                    MaproomUIManager.Instance.OpenRegionActionPanel(finalRegion.region);
                }
            }

            unit.Reset();
            CurrentState = UnitState.Idle;

            Debug.Log("Move complete. New position: " + transform.position);
            StartCoroutine(DelayedEnemyVisibilityUpdate());
        });
    }

    private IEnumerator DelayedEnemyVisibilityUpdate()
    {
        yield return null; // waiting one frame
        UnitManager.Instance.UpdateEnemyUnitVisibility(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.gameObject.GetComponent<Unit>();

        // Declare enemyController here:
        var enemyController = collision.gameObject.GetComponent<UnitController>();
        if (enemy != null && enemy.factionID != unit.factionID)
        {
            if (enemy != null && enemy.factionID != unit.factionID)
            {
            
                if (tweener != null && !tweener.IsComplete())
                {
                tweener.Pause();
                }

                CurrentState = UnitState.Attacking;


                // Force the enemy to remain visible for 60 seconds.
                if (enemyController != null)
                {
                    UnitManager.Instance.ForceEnemyVisibility(enemyController, 60f);
                }

                unit.StartAttack(enemy);
                pivot.DOLookAt(enemy.transform.position+ Vector3.back*10, 0.2f, AxisConstraint.Y);
                //Vector3.up
                //Vector3.down
                //Vector3.right
                //Vector3.left
     

            }
        }

    }
}




