using UnityEngine;
using UnityEngine.InputSystem;

public class DoorTeleport : MonoBehaviour
{
    public DoorController doorController;

    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (doorController != null)
            {
                doorController.ToggleDoor();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Player near door - Press E");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}