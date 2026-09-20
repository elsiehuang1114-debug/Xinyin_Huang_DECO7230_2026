using UnityEngine;

public class XREpisodeSelect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EpisodePortalTransition portalTransition;

    public void SelectEpisode()
    {
        if (portalTransition == null)
        {
            Debug.LogWarning(
                "XREpisodeSelect: EpisodePortalTransition is not assigned."
            );
            return;
        }

        portalTransition.PlayTransition();
    }
}