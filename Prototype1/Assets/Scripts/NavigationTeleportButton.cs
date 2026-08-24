using UnityEngine;

public class NavigationTeleportButton : MonoBehaviour
{
    public Transform player;
    public Transform teleportTarget;
    public GameObject navigationPanel;

    private void OnMouseDown()
    {
        if (player != null && teleportTarget != null)
        {
            player.SetPositionAndRotation(
                teleportTarget.position,
                teleportTarget.rotation
            );
        }

        if (navigationPanel != null)
        {
            navigationPanel.SetActive(false);
        }
    }
}