using UnityEngine;
using System.Collections;

public class ChapterDoorTeleport : MonoBehaviour
{
    public DoorController doorController;
    public ChapterManager chapterManager;
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

        if (chapterManager != null)
        {
            Debug.Log(
                "Entering Chapter: " +
                chapterManager.GetCurrentChapterTitle()
            );
        }

        yield return new WaitForSeconds(delay);

        if (player != null && teleportTarget != null)
        {
            player.SetPositionAndRotation(
                teleportTarget.position,
                teleportTarget.rotation
            );
        }

        isTeleporting = false;
    }
}