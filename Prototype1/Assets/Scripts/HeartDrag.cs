using UnityEngine;

public class HeartDrag : MonoBehaviour
{
    private Camera mainCamera;

    private bool isMouseDragging = false;
    private bool isXRDragging = false;

    private float dragDistance;
    private Vector3 startPosition;
    private Quaternion startRotation;

    public Transform snapPoint;

    private bool isFavourite = false;

    private void Start()
    {
        mainCamera = Camera.main;

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // =========================================================
    // XR GRAB
    // =========================================================

    public void XRGrabStarted()
    {
        if (isFavourite)
            return;

        isXRDragging = true;

        Debug.Log("Heart XR grab started.");
    }

    public void XRGrabEnded()
    {
        isXRDragging = false;

        // If it was not placed in Favourite Area,
        // return it to the Heart Shelf.
        if (!isFavourite)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        Debug.Log("Heart XR grab ended.");
    }

    // =========================================================
    // DESKTOP MOUSE FALLBACK
    // =========================================================

    private void OnMouseDown()
    {
        if (isFavourite)
            return;

        if (mainCamera == null)
            return;

        isMouseDragging = true;

        dragDistance = Vector3.Distance(
            transform.position,
            mainCamera.transform.position
        );
    }

    private void OnMouseDrag()
    {
        if (!isMouseDragging || mainCamera == null)
            return;

        Ray ray =
            mainCamera.ScreenPointToRay(Input.mousePosition);

        Vector3 newPosition =
            ray.origin + ray.direction * dragDistance;

        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        isMouseDragging = false;

        if (!isFavourite)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }

    // =========================================================
    // FAVOURITE AREA
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Heart entered trigger: " + other.name);

        if (isFavourite)
            return;

        if (!isXRDragging && !isMouseDragging)
            return;

        if (other.CompareTag("FavouriteArea"))
        {
            Debug.Log("Favourite Area detected!");

            isFavourite = true;
            isXRDragging = false;
            isMouseDragging = false;

            if (snapPoint != null)
            {
                transform.position = snapPoint.position;
                transform.rotation = snapPoint.rotation;

                Debug.Log("Heart snapped to HeartSnapPoint!");
            }
            else
            {
                Debug.LogWarning("Heart snapPoint is missing!");
            }

            Debug.Log("Podcast added to favourites!");
        }
    }
}