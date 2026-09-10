using UnityEngine;

/// <summary>
/// Desktop simulation of the future two-hand "enlarge" gesture on the current episode card.
/// XR-ready intent boundary: BeginEnlarge() / UpdateEnlarge(float) / Confirm().
/// Desktop driver: mouse-wheel scroll or vertical mouse drag while hovering the card.
/// Crossing the confirm threshold calls EpisodeManager.SelectCurrentEpisode() (existing
/// teleport to ChapterTeleportTarget) then resets card scale.
/// </summary>
public class CardEnlargeGesture : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EpisodeManager _episodeManager;

    [Header("Settings")]
    [Tooltip("amount01 value at which the card is auto-selected and the player teleports.")]
    [SerializeField] private float _confirmThreshold = 0.85f;
    [Tooltip("Maximum scale multiplier applied to the original card scale at amount01 = 1.")]
    [SerializeField] private float _maxScaleMultiplier = 1.5f;
    [Tooltip("How much each scroll wheel tick changes amount01.")]
    [SerializeField] private float _scrollSensitivity = 0.15f;
    [Tooltip("Pixels of upward drag needed to reach amount01 = 1.")]
    [SerializeField] private float _dragPixelsForFull = 300f;

    private Vector3 _originalScale;
    private float   _amount01;
    private bool    _isHovered;
    private bool    _confirmed;
    private bool    _isDragging;
    private float   _dragStartY;

    // ── Lifecycle ─────────────────────────────────────────────────────────

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    // ── XR-ready intent boundary ──────────────────────────────────────────

    /// <summary>Called to start a fresh enlarge gesture (resets state).</summary>
    public void BeginEnlarge()
    {
        _amount01  = 0f;
        _confirmed = false;
        transform.localScale = _originalScale;
    }

    /// <summary>
    /// Drive the card scale. amount01 ∈ [0,1]; crossing _confirmThreshold triggers Confirm().
    /// Desktop: called each frame by drag/scroll. XR: call from hand-tracking driver.
    /// </summary>
    public void UpdateEnlarge(float amount01)
    {
        if (_confirmed) return;
        _amount01 = Mathf.Clamp01(amount01);
        ApplyScale();
        if (_amount01 >= _confirmThreshold)
            Confirm();
    }

    /// <summary>
    /// Finalise the gesture: resets scale and calls EpisodeManager.SelectCurrentEpisode().
    /// XR: can also be called directly when a gesture completes confidently.
    /// </summary>
    public void Confirm()
    {
        if (_confirmed) return;
        _confirmed = true;
        ResetScale();
        _episodeManager?.SelectCurrentEpisode();
    }

    // ── Internal helpers ──────────────────────────────────────────────────

    private void ApplyScale()
    {
        float mult = 1f + _amount01 * (_maxScaleMultiplier - 1f);
        transform.localScale = _originalScale * mult;
    }

    private void ResetScale()
    {
        _amount01 = 0f;
        transform.localScale = _originalScale;
    }

    // ── Desktop simulation (mouse) ────────────────────────────────────────

    private void OnMouseEnter()
    {
        _isHovered = true;
        // Allow re-use after player returns from ChapterPortal
        if (_confirmed)
        {
            _confirmed = false;
            _amount01  = 0f;
        }
    }

    private void OnMouseExit() => _isHovered = false;

    private void OnMouseDown()
    {
        BeginEnlarge();
        _isDragging  = true;
        _dragStartY  = Input.mousePosition.y;
    }

    private void OnMouseUp() => _isDragging = false;

    private void Update()
    {
        if (_confirmed) return;

        // Vertical drag on the card (drag up to enlarge)
        if (_isDragging)
        {
            float dragDelta = (Input.mousePosition.y - _dragStartY) / _dragPixelsForFull;
            UpdateEnlarge(Mathf.Max(0f, dragDelta));
            return;
        }

        // Mouse wheel while hovering
        if (_isHovered)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f)
                UpdateEnlarge(_amount01 + scroll * _scrollSensitivity);
        }
    }
}
