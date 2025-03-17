using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Vector3 destination;
    private bool isMoving = false;

    private LineRenderer unitToRoadLine;
    private LineRenderer roadWaypointLine;

    void Awake()
    {
        // First check if a LineRenderer already exists, if not then create it.
        unitToRoadLine = GetComponent<LineRenderer>();
        if (unitToRoadLine == null)
            unitToRoadLine = gameObject.AddComponent<LineRenderer>();

        ConfigureLineRenderer(unitToRoadLine, Color.yellow);

        // Create a child GameObject for the road waypoint line
        GameObject waypointLineObj = new GameObject("RoadWaypointLine");
        waypointLineObj.transform.parent = transform;

        roadWaypointLine = waypointLineObj.AddComponent<LineRenderer>();
        ConfigureLineRenderer(roadWaypointLine, Color.cyan);
    }

    private void ConfigureLineRenderer(LineRenderer lr, Color color)
    {
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.enabled = false;
    }

    public void MoveAlongRoad(Vector3 nearestPointOnRoad, Vector3 roadDestination)
    {
        destination = roadDestination;

        if (!isMoving)
            StartCoroutine(MoveRoutine(nearestPointOnRoad, roadDestination));
    }

    private IEnumerator MoveRoutine(Vector3 nearestRoadPoint, Vector3 roadDest)
    {
        isMoving = true;

        unitToRoadLine.enabled = true;
        roadWaypointLine.enabled = true;

        while (Vector3.Distance(transform.position, nearestRoadPoint) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nearestRoadPoint, moveSpeed * Time.deltaTime);
            unitToRoadLine.SetPosition(0, transform.position);
            unitToRoadLine.SetPosition(1, nearestRoadPoint);
            roadWaypointLine.SetPosition(0, nearestRoadPoint);
            roadWaypointLine.SetPosition(1, roadDest);
            yield return null;
        }

        transform.position = nearestRoadPoint;

        while (Vector3.Distance(transform.position, roadDest) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, roadDest, moveSpeed * Time.deltaTime);
            roadWaypointLine.SetPosition(0, transform.position);
            roadWaypointLine.SetPosition(1, roadDest);
            unitToRoadLine.enabled = false;
            yield return null;
        }

        transform.position = roadDest;
        isMoving = false;
        roadWaypointLine.enabled = false;
        unitToRoadLine.enabled = false;

        Debug.Log("Unit reached final road destination.");
    }

    
}
