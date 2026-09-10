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

    public Transform player;
    public Transform chapterTarget;
    public GameObject navigationPanel;

    private int currentIndex = 0;

    /// <summary>Fired at the end of ShowEpisode so listeners can refresh carousel visuals.</summary>
    public System.Action OnEpisodeChanged;

    /// <summary>Read-only access to the current index for the carousel.</summary>
    public int CurrentIndex => currentIndex;

    /// <summary>Total number of episodes (0 if array is null).</summary>
    public int EpisodeCount => episodes == null ? 0 : episodes.Length;

    /// <summary>Safe modulo access: returns null if episodes is empty.</summary>
    public EpisodeData GetEpisode(int i)
    {
        if (episodes == null || episodes.Length == 0) return null;
        int idx = ((i % episodes.Length) + episodes.Length) % episodes.Length;
        return episodes[idx];
    }

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

    public void SelectCurrentEpisode()
    {
        Debug.Log("Selected Episode: " + episodes[currentIndex].title);

        if (player != null && chapterTarget != null)
        {
            player.SetPositionAndRotation(
                chapterTarget.position,
                chapterTarget.rotation
            );
        }

        if (navigationPanel != null)
        {
            navigationPanel.SetActive(true);
        }
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

        OnEpisodeChanged?.Invoke();
    }
}