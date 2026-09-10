using UnityEngine;
using TMPro;

/// <summary>
/// Per-chapter station in the Chapter Portal.
/// Reads title/time from ChapterManager.chapters[chapterIndex] at Start (display only).
/// Proximity trigger (Player enters) → preview highlight only, never selects.
/// OnMouseDown → explicit select, calls ChapterManager.ShowChapter(index),
/// then notifies ChapterStationController for single-selection enforcement.
/// </summary>
public class ChapterStation : MonoBehaviour
{
    [Header("Station Data")]
    [Tooltip("Index into ChapterManager.chapters[].")]
    [SerializeField] private int _chapterIndex;

    [Header("References")]
    [Tooltip("The scene ChapterManager (single source of truth for chapter state).")]
    [SerializeField] private ChapterManager _chapterManager;
    [Tooltip("Controller that enforces single-selection highlight across all stations.")]
    [SerializeField] private ChapterStationController _controller;
    [Tooltip("TMP label showing the chapter title (display only).")]
    [SerializeField] private TMP_Text _titleLabel;
    [Tooltip("TMP label showing the chapter time range (display only).")]
    [SerializeField] private TMP_Text _timeLabel;

    [Header("Visual Highlight")]
    [Tooltip("Renderer whose material color is driven by highlight state.")]
    [SerializeField] private Renderer _stationRenderer;
    [SerializeField] private Color _normalColor   = Color.white;
    [SerializeField] private Color _proximityColor = new Color(0.4f, 0.8f, 1f, 1f);  // soft blue
    [SerializeField] private Color _selectedColor  = new Color(0.3f, 1f, 0.5f, 1f);  // soft green

    // Runtime state
    private Material _material;     // per-instance copy so we don't tint shared materials
    private bool _isSelected;
    private bool _isNearby;

    /// <summary>Read-only index, used by ChapterStationController comparisons.</summary>
    public int ChapterIndex => _chapterIndex;

    // ───────────────────────────── Lifecycle ─────────────────────────────

    void Awake()
    {
        // Create a per-instance material so highlight colors don't bleed to other stations
        if (_stationRenderer != null)
            _material = _stationRenderer.material;
    }

    void Start()
    {
        // Populate display labels from chapter data. No state is duplicated here —
        // ChapterManager remains the single source of truth for selection state.
        if (_chapterManager != null
            && _chapterManager.chapters != null
            && _chapterIndex >= 0
            && _chapterIndex < _chapterManager.chapters.Length)
        {
            var data = _chapterManager.chapters[_chapterIndex];
            if (_titleLabel != null) _titleLabel.text = data.title;
            if (_timeLabel != null)  _timeLabel.text  = data.time;
        }
        UpdateVisual();
    }

    // ─────────────────────────── Input / Events ──────────────────────────

    /// <summary>Explicit click → select this chapter and update the Current Chapter Door.</summary>
    void OnMouseDown()
    {
        if (_chapterManager != null)
            _chapterManager.ShowChapter(_chapterIndex);   // updates ChapterTitle/ChapterTime TMP on door

        if (_controller != null)
            _controller.SetSelected(this);                // enforce single-selection highlight
    }

    /// <summary>Player enters proximity → preview highlight only, never selects.</summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearby = true;
            UpdateVisual();
        }
    }

    /// <summary>Player leaves proximity → remove preview (selected highlight remains if set).</summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearby = false;
            UpdateVisual();
        }
    }

    // ────────────────────────── Public API (called by controller) ─────────────────────────

    /// <summary>Called by ChapterStationController to set/clear this station's selection highlight.</summary>
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        UpdateVisual();
    }

    // ─────────────────────────── Private helpers ─────────────────────────

    private void UpdateVisual()
    {
        if (_material == null) return;

        if (_isSelected)
            _material.color = _selectedColor;
        else if (_isNearby)
            _material.color = _proximityColor;
        else
            _material.color = _normalColor;
    }
}
