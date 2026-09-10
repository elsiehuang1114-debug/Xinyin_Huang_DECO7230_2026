using UnityEngine;
using TMPro;

/// <summary>
/// Manages the 3-card spatial carousel in EpisodeRoom.
/// Positions cards relative to EpisodeTeleportTarget at runtime.
/// Exposes Swipe(int) as the XR-ready intent boundary for browse input.
/// Neighbour cards are VISUAL ONLY — no interaction scripts, no EpisodeManager refs.
/// </summary>
public class PodcastCarousel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EpisodeManager _episodeManager;
    [SerializeField] private Transform _episodeTeleportTarget;

    [Header("Card Transforms")]
    [SerializeField] private Transform _currentCard;
    [SerializeField] private Transform _prevCard;
    [SerializeField] private Transform _nextCard;

    [Header("Neighbour Labels")]
    [SerializeField] private TMP_Text _prevCardLabel;
    [SerializeField] private TMP_Text _nextCardLabel;

    [Header("Layout (relative to EpisodeTeleportTarget)")]
    [Tooltip("How far in front of the teleport target the current card sits.")]
    [SerializeField] private float _currentCardForwardDist = 1.75f;
    [Tooltip("How far forward the neighbour cards sit (further back than current).")]
    [SerializeField] private float _neighbourForwardDist = 2.15f;
    [Tooltip("Horizontal offset (right axis) for each neighbour card.")]
    [SerializeField] private float _neighbourSideOffset = 1.6f;
    [Tooltip("Yaw angle in degrees each neighbour is rotated toward the user.")]
    [SerializeField] private float _neighbourYawAngle = 22f;
    [Tooltip("World-space Y position for all cards.")]
    [SerializeField] private float _cardWorldY = 1.5f;

    // ── Lifecycle ─────────────────────────────────────────────────────────

    private void Start()
    {
        PositionCards();
        RefreshNeighbourLabels();

        if (_episodeManager != null)
            _episodeManager.OnEpisodeChanged += RefreshNeighbourLabels;
    }

    private void OnDestroy()
    {
        if (_episodeManager != null)
            _episodeManager.OnEpisodeChanged -= RefreshNeighbourLabels;
    }

    // ── Card positioning ──────────────────────────────────────────────────

    /// <summary>
    /// Places the three cards relative to EpisodeTeleportTarget.
    /// Called once in Start(); scale is preserved from scene setup.
    /// </summary>
    private void PositionCards()
    {
        if (_episodeTeleportTarget == null) return;

        Vector3 origin  = _episodeTeleportTarget.position;
        Vector3 forward = _episodeTeleportTarget.forward;
        Vector3 right   = _episodeTeleportTarget.right;

        // Current card — centred, closest to user, facing +Z (default)
        if (_currentCard != null)
        {
            Vector3 pos = origin + forward * _currentCardForwardDist;
            pos.y = _cardWorldY;
            _currentCard.position = pos;
            _currentCard.rotation = Quaternion.identity;
        }

        // Prev card — left side, slightly further back, angled toward user
        if (_prevCard != null)
        {
            Vector3 pos = origin + forward * _neighbourForwardDist - right * _neighbourSideOffset;
            pos.y = _cardWorldY;
            _prevCard.position = pos;
            _prevCard.rotation = Quaternion.Euler(0f, _neighbourYawAngle, 0f);
        }

        // Next card — right side, slightly further back, angled toward user
        if (_nextCard != null)
        {
            Vector3 pos = origin + forward * _neighbourForwardDist + right * _neighbourSideOffset;
            pos.y = _cardWorldY;
            _nextCard.position = pos;
            _nextCard.rotation = Quaternion.Euler(0f, -_neighbourYawAngle, 0f);
        }
    }

    // ── XR-ready intent method ────────────────────────────────────────────

    /// <summary>
    /// Browse the carousel. direction > 0 = next episode; direction &lt; 0 = previous.
    /// Desktop: called by SwipeInput. XR: call directly from hand-tracking driver.
    /// </summary>
    public void Swipe(int direction)
    {
        if (_episodeManager == null) return;

        if (direction > 0)
            _episodeManager.NextEpisode();
        else if (direction < 0)
            _episodeManager.PreviousEpisode();

        // Neighbour labels are refreshed via OnEpisodeChanged → RefreshNeighbourLabels
    }

    // ── Visual refresh ────────────────────────────────────────────────────

    /// <summary>Updates neighbour card title labels from EpisodeManager data.</summary>
    public void RefreshNeighbourLabels()
    {
        if (_episodeManager == null) return;

        int count = _episodeManager.EpisodeCount;
        if (count == 0) return;

        int current  = _episodeManager.CurrentIndex;
        int prevIdx  = ((current - 1) % count + count) % count;
        int nextIdx  = (current + 1) % count;

        if (_prevCardLabel != null)
        {
            EpisodeManager.EpisodeData ep = _episodeManager.GetEpisode(prevIdx);
            _prevCardLabel.text = ep != null ? ep.title : string.Empty;
        }

        if (_nextCardLabel != null)
        {
            EpisodeManager.EpisodeData ep = _episodeManager.GetEpisode(nextIdx);
            _nextCardLabel.text = ep != null ? ep.title : string.Empty;
        }
    }
}
