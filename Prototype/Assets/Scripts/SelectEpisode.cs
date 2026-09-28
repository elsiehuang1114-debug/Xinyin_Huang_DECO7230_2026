using UnityEngine;

public class SelectEpisode : MonoBehaviour
{
    public EpisodeManager episodeManager;

    private void OnMouseDown()
    {
        if (episodeManager != null)
        {
            episodeManager.SelectCurrentEpisode();
        }
    }
}