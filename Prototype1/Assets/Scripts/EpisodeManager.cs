using UnityEngine;
using TMPro;

public class EpisodeManager : MonoBehaviour
{
    [System.Serializable]
    public class EpisodeData
    {
        public string title;
        public string duration;
        public Material coverMaterial;
    }

    public EpisodeData[] episodes;

    public TMP_Text titleText;
    public TMP_Text durationText;
    public Renderer coverRenderer;

    private int currentIndex = 0;

    void Start()
    {
        ShowEpisode();
    }

    public void NextEpisode()
    {
        currentIndex++;

        if (currentIndex >= episodes.Length)
        {
            currentIndex = 0;
        }

        ShowEpisode();
    }

    public void PreviousEpisode()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = episodes.Length - 1;
        }

        ShowEpisode();
    }

    void ShowEpisode()
    {
        if (episodes.Length == 0) return;

        titleText.text = episodes[currentIndex].title;
        durationText.text = episodes[currentIndex].duration;

        if (episodes[currentIndex].coverMaterial != null)
        {
            coverRenderer.material = episodes[currentIndex].coverMaterial;
        }
    }
}