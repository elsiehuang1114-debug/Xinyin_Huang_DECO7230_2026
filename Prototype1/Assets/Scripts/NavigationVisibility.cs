using UnityEngine;

public class NavigationVisibility : MonoBehaviour
{
    public GameObject navigationPanel;

    public void ShowNavigation()
    {
        if (navigationPanel != null)
        {
            navigationPanel.SetActive(true);
        }
    }

    public void HideNavigation()
    {
        if (navigationPanel != null)
        {
            navigationPanel.SetActive(false);
        }
    }
}