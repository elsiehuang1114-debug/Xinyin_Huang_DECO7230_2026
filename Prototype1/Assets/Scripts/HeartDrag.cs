using UnityEngine;

public class HeartDrag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float dragDistance;

    private Vector3 startPosition;

    public Transform snapPoint;

    private bool isFavourite = false;

    void Start()
    {
        mainCamera = Camera.main;
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (isFavourite)
            return;

        if (mainCamera == null)
            return;

        isDragging = true;

        dragDistance = Vector3.Distance(
            transform.position,
            mainCamera.transform.position
        );
    }

    void OnMouseDrag()
    {
        if (!isDragging || mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Vector3 newPosition =
            ray.origin + ray.direction * dragDistance;

        transform.position = newPosition;
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (!isFavourite)
        {
            transform.position = startPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFavourite)
            return;

        if (!isDragging)
            return;

        if (other.CompareTag("FavouriteArea"))
        {
            isFavourite = true;
            isDragging = false;

            if (snapPoint != null)
            {
                transform.position = snapPoint.position;
                transform.rotation = snapPoint.rotation;
            }

            Debug.Log("Podcast added to favourites!");
        }
    }
}