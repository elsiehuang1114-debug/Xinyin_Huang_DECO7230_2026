using UnityEngine;

public class HeartDrag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float dragDistance;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
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
    }
}