using UnityEngine;

public class EpisodeButton : MonoBehaviour
{
    public EpisodeManager episodeManager;
    public bool isNextButton;

    private void OnMouseDown()
    {
        if (episodeManager == null) return;

        if (isNextButton)
        {
            episodeManager.NextEpisode();
        }
        else
        {
            episodeManager.PreviousEpisode();
        }
    }
}