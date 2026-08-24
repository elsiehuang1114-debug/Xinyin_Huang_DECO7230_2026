using UnityEngine;
using System.Collections;

public class ChapterDoorTeleport : MonoBehaviour
{
    public DoorController doorController;
    public ChapterManager chapterManager;
    public Transform player;
    public Transform teleportTarget;
    public GameObject navigationPanel;

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

        // Open the chapter door
        if (doorController != null)
        {
            doorController.ToggleDoor();
        }

        // Print which chapter is being entered
        if (chapterManager != null)
        {
            Debug.Log(
                "Entering Chapter: " +
                chapterManager.GetCurrentChapterTitle()
            );
        }

        // Wait briefly so the user can see the door opening
        yield return new WaitForSeconds(delay);

        // Teleport to Podcast Experience
        if (player != null && teleportTarget != null)
        {
            player.SetPositionAndRotation(
                teleportTarget.position,
                teleportTarget.rotation
            );
        }

        // Hide navigation panel in Podcast Experience
        if (navigationPanel != null)
        {
            navigationPanel.SetActive(false);
        }

        // Close the door after teleporting
        if (doorController != null)
        {
            doorController.CloseDoor();
        }

        isTeleporting = false;
    }
}