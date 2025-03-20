using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    private Camera mainCamera;
    private MaproomUIManager uiManager;

    private void Start()
    {
        mainCamera = Camera.main;
        uiManager = FindObjectOfType<MaproomUIManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*
            if (IsPointerOverUI())
            {
                Debug.Log("🛑 ClickManager: Click ignored (UI was clicked).");
                return;
            }
            */

            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("🛑 ClickManager: Ignoring click on UI.");
                return;
            }

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // Get all colliders under the click.
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);


            // --- Added Section for Move Mode ---
            if (GameManager.Instance != null && GameManager.Instance.IsMoveModeActive)
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
                    UnitSelection unit = hit.collider.GetComponent<UnitSelection>();
                    if (unit != null)
                    {
                        // Found a unit script on the parent
                        if (GameManager.Instance != null)
                           // GameManager.Instance.unitClickedThisFrame = true;

                        unit.OnUnitSelected();
                        return;
                    }
                }

                // Before processing region clicks, check if either the unit panel is already open.
                if (uiManager != null && uiManager.unitActionPanel.activeSelf)
                {
                    Debug.Log("ClickManager: A unit panel is already open; skipping region clicks.");
                    return;
                }



                foreach (RaycastHit2D hit in hits)
                {
                    RegionClickHandler region = hit.collider.GetComponentInParent<RegionClickHandler>();
                    if (region != null)
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
