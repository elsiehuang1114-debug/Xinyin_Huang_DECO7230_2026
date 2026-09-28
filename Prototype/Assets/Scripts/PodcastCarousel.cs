using UnityEngine;
using TMPro;

public class PodcastCarousel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EpisodeManager _episodeManager;
    [SerializeField] private Transform _episodeTeleportTarget;

    [Header("Card Transforms")]
    [SerializeField] private Transform _currentCard;
    [SerializeField] private Transform _prevCard;
    [SerializeField] private Transform _nextCard;

    [Header("Neighbour Visuals")]
    [SerializeField] private Renderer _prevCoverRenderer;
    [SerializeField] private Renderer _nextCoverRenderer;
    [SerializeField] private TMP_Text _prevCardLabel;
    [SerializeField] private TMP_Text _nextCardLabel;

    [Header("Layout")]
    [SerializeField] private float _currentCardForwardDist = 1.75f;
    [SerializeField] private float _neighbourForwardDist = 2.15f;
    [SerializeField] private float _neighbourSideOffset = 1.6f;
    [SerializeField] private float _neighbourYawAngle = 22f;
    [SerializeField] private float _cardWorldY = 1.5f;

    private void Start()
    {
        PositionCards();
        RefreshCards();

        if (_episodeManager != null)
            _episodeManager.OnEpisodeChanged += RefreshCards;
    }

    private void OnDestroy()
    {
        if (_episodeManager != null)
            _episodeManager.OnEpisodeChanged -= RefreshCards;
    }

    private void PositionCards()
    {
        if (_episodeTeleportTarget == null)
            return;

        Vector3 origin = _episodeTeleportTarget.position;
        Vector3 forward = _episodeTeleportTarget.forward;
        Vector3 right = _episodeTeleportTarget.right;

        // Current Card
        if (_currentCard != null)
        {
            Vector3 pos =
                origin + forward * _currentCardForwardDist;

            pos.y = _cardWorldY;

            _currentCard.position = pos;

            _currentCard.rotation =
                Quaternion.LookRotation(-forward, Vector3.up);
        }

        // Previous Card
        if (_prevCard != null)
        {
            Vector3 pos =
                origin
                + forward * _neighbourForwardDist
                - right * _neighbourSideOffset;

            pos.y = _cardWorldY;

            _prevCard.position = pos;

            _prevCard.rotation =
                Quaternion.LookRotation(
                    -forward + right * 0.35f,
                    Vector3.up
                );
        }

        // Next Card
        if (_nextCard != null)
        {
            Vector3 pos =
                origin
                + forward * _neighbourForwardDist
                + right * _neighbourSideOffset;

            pos.y = _cardWorldY;

            _nextCard.position = pos;

            _nextCard.rotation =
                Quaternion.LookRotation(
                    -forward - right * 0.35f,
                    Vector3.up
                );
        }
    }

    public void Swipe(int direction)
    {
        if (_episodeManager == null)
            return;

        if (direction > 0)
        {
            _episodeManager.NextEpisode();
        }
        else if (direction < 0)
        {
            _episodeManager.PreviousEpisode();
        }
    }

    public void RefreshCards()
    {
        if (_episodeManager == null)
            return;

        int count = _episodeManager.EpisodeCount;

        if (count == 0)
            return;

        int current = _episodeManager.CurrentIndex;

        int prevIndex =
            ((current - 1) % count + count) % count;

        int nextIndex =
            (current + 1) % count;

        EpisodeManager.EpisodeData previousEpisode =
            _episodeManager.GetEpisode(prevIndex);

        EpisodeManager.EpisodeData nextEpisode =
            _episodeManager.GetEpisode(nextIndex);

        // Previous Card
        if (previousEpisode != null)
        {
            if (_prevCardLabel != null)
            {
                _prevCardLabel.text =
                    previousEpisode.title;
            }

            if (_prevCoverRenderer != null &&
                previousEpisode.coverMaterial != null)
            {
                _prevCoverRenderer.material =
                    previousEpisode.coverMaterial;
            }
        }

        // Next Card
        if (nextEpisode != null)
        {
            if (_nextCardLabel != null)
            {
                _nextCardLabel.text =
                    nextEpisode.title;
            }

            if (_nextCoverRenderer != null &&
                nextEpisode.coverMaterial != null)
            {
                _nextCoverRenderer.material =
                    nextEpisode.coverMaterial;
            }
        }
    }
}