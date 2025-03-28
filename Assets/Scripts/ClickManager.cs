using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    private Camera mainCamera;



    private void Start()
    {
        mainCamera = Camera.main;
    }

    private bool IsPointerOverUIObject()
    {
        var eventSystem = EventSystem.current;
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {


            // Check if the click is on a UI element.
            if (IsPointerOverUIObject())
            {
                return;
            }


            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // Get all colliders under the click.
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);


            // --- Added Section for Move Mode ---
            if (GameManager.Instance.CurrentState == GameManager.State.Move)
            {
                Debug.Log("Move mode active; processing road clicks only.");

                if (hits.Length == 0)
                {
                    Debug.Log("Empty space clicked. Cancelling move mode.");
                    GameManager.Instance.DeactivateMoveMode();
                    return;
                }
                //bool validDestinationClicked = false;

                foreach (RaycastHit2D hit in hits)
                {
                    RegionCapturePoint point = hit.collider.GetComponentInParent<RegionCapturePoint>();
                    if (point != null)
                    {
                        // Trigger the road's click handler.
                        GameManager.Instance.SetDestination(point);
                        //validDestinationClicked = true;
                        return;
                    }
                }
                Debug.Log("No selectable road was clicked.");
                return;
            }


            if (hits.Length > 0)
            {
                Debug.Log($"ClickManager: Found {hits.Length} hit(s).");

                // First, prioritize units.
                foreach (RaycastHit2D hit in hits)
                {
                    // Instead of GetComponent<UnitSelection>(), use GetComponentInParent:
                    UnitController unit = hit.collider.GetComponent<UnitController>();
                    if (unit != null)
                    {
                        if (GameManager.Instance.CurrentState == GameManager.State.Attack && unit.Unit.factionID != GameManager.Instance.LocalPlayerId && GameManager.Instance.SelectedUnit != null)
                        {
                            GameManager.Instance.AttackRegion(GameManager.Instance.LocalPlayerId, unit.CurrentPoint.region);
                        }
                        else if (!MaproomUIManager.Instance.HasPanel)
                        {
                            GameManager.Instance.SelectUnit(unit);
                            MaproomUIManager.Instance.OpenUnitActionPanel();
                        }
                        return;
                    }
                }

                foreach (RaycastHit2D hit in hits)
                {
                    RegionClickHandler region = hit.collider.GetComponentInParent<RegionClickHandler>();
                    if (region != null && !MaproomUIManager.Instance.HasPanel)
                    {
                        region.OnRegionSelected();
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("ClickManager: No hit found.");


                /*if (GameManager.Instance.selectedUnit != null)
                {
                    GameManager.Instance.DeactivateMoveMode();
                }
                */
            }

            // Optionally, if nothing relevant was hit, you can close panels.
            // Uncomment if you want immediate closing on click empty space.
            /*
            if (uiManager != null)
            {
                uiManager.CloseRegionActionPanel();
                uiManager.CloseUnitActionPanel();
            }
            */
        }
    }

    /*
    // ✅ NEW: Function to check if UI is clicked.
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0; // If there's a result, a UI element was clicked.
    }
    */
}
