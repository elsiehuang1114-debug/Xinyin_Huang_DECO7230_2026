using UnityEngine;

public class ToggleNavigationPanel : MonoBehaviour
{
    public GameObject navigationPanel;

    private void OnMouseDown()
    {
        if (navigationPanel == null)
            return;

        navigationPanel.SetActive(!navigationPanel.activeSelf);
    }
}