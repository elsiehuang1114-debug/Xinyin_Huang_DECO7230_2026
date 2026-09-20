using UnityEngine;

public class XRDoorInteractable : MonoBehaviour
{
    [SerializeField] private DoorController doorController;

    public void ToggleDoor()
    {
        if (doorController == null)
        {
            Debug.LogWarning("XRDoorInteractable: DoorController is not assigned.");
            return;
        }

        doorController.ToggleDoor();
    }
}