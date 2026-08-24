using UnityEngine;
using System.Collections;

public class DoorTeleport : MonoBehaviour
{
    public DoorController doorController;
    public Transform player;
    public Transform teleportTarget;
    public float delay = 0.3f;

    private bool isTeleporting = false;

    private void OnMouseDown()
    {
        if (!isTeleporting)
        {
            StartCoroutine(OpenAndTeleport());
        }
    }

    private IEnumerator OpenAndTeleport()
    {
        isTeleporting = true;

        if (doorController != null)
        {
            doorController.ToggleDoor();
        }

        yield return new WaitForSeconds(delay);

        if (player != null && teleportTarget != null)
        {
            player.SetPositionAndRotation(
                teleportTarget.position,
                teleportTarget.rotation
            );
        }

        // Teleport 后自动关门
        if (doorController != null)
        {
            doorController.CloseDoor();
        }

        isTeleporting = false;
    }
}