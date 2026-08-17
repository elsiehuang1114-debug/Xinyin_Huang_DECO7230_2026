using UnityEngine;

public class DoorHandleClick : MonoBehaviour
{
    public DoorController doorController;

    private void OnMouseDown()
    {
        Debug.Log("Door handle clicked!");

        if (doorController != null)
        {
            doorController.ToggleDoor();
        }
    }
}