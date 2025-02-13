using UnityEngine;

public class RegionClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private static RegionClickHandler currentlySelectedRegion = null; // Store last selected region

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Save original color
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect actual mouse clicks (Left Click)
        {
            DetectTouchOrClick();
        }
    }

    void DetectTouchOrClick()
    {
        // Convert mouse position to world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure the click is in the 2D plane

        // Perform a Raycast2D to detect the clicked object
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log($"✅ Clicked on: {hit.collider.gameObject.name}");

            // Ensure we are clicking a region and not something else
            RegionClickHandler clickedRegion = hit.collider.GetComponent<RegionClickHandler>();

            if (clickedRegion != null)
            {
                clickedRegion.OnRegionSelected();
            }
        }
        else
        {
            Debug.Log("❌ Clicked on: Nothing (Collider not detected!)");
        }
    }

    void OnRegionSelected()
    {
        Debug.Log($"🎯 Region Selected: {gameObject.name}");

        // Reset the previously selected region's color (if exists)
        if (currentlySelectedRegion != null && currentlySelectedRegion != this)
        {
            currentlySelectedRegion.ResetColor();
        }

        // Highlight the new region
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        // Store this region as the currently selected region
        currentlySelectedRegion = this;
    }

    void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
