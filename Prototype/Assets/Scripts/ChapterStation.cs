using UnityEngine;
using TMPro;

public class ChapterStation : MonoBehaviour
{
    [Header("Station Data")]
    [SerializeField] private int _chapterIndex;

    [Header("References")]
    [SerializeField] private ChapterManager _chapterManager;
    [SerializeField] private ChapterStationController _controller;
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private TMP_Text _timeLabel;

    [Header("Visual Highlight")]
    [SerializeField] private Renderer _stationRenderer;

    [SerializeField]
    private Color _normalColor =
        new Color(0.4196f, 0.4863f, 0.5765f, 1f); // #6B7C93

    [SerializeField]
    private Color _proximityColor =
        new Color(0.6078f, 0.4863f, 0.9412f, 1f); // #9B7CF0

    [SerializeField]
    private Color _selectedColor =
        new Color(0.1804f, 0.8000f, 0.4431f, 1f); // #2ECC71

    private Material _material;
    private bool _isSelected;
    private bool _isNearby;

    public int ChapterIndex => _chapterIndex;

    private void Awake()
    {
        if (_stationRenderer != null)
            _material = _stationRenderer.material;
    }

    private void Start()
    {
        if (_chapterManager != null &&
            _chapterManager.chapters != null &&
            _chapterIndex >= 0 &&
            _chapterIndex < _chapterManager.chapters.Length)
        {
            var data = _chapterManager.chapters[_chapterIndex];

            if (_titleLabel != null)
                _titleLabel.text = data.title;

            if (_timeLabel != null)
                _timeLabel.text = data.time;
        }

        UpdateVisual();
    }

    // =========================================================
    // XR INTERACTION
    // =========================================================

    public void XRHoverEnter()
    {
        _isNearby = true;
        UpdateVisual();
    }

    public void XRHoverExit()
    {
        _isNearby = false;
        UpdateVisual();
    }

    public void XRSelect()
    {
        if (_chapterManager != null)
            _chapterManager.ShowChapter(_chapterIndex);

        if (_controller != null)
            _controller.SetSelected(this);

        Debug.Log("XR Selected Chapter: " + _chapterIndex);
    }

    // =========================================================
    // DESKTOP FALLBACK
    // =========================================================

    private void OnMouseDown()
    {
        XRSelect();
    }

    private void OnMouseEnter()
    {
        XRHoverEnter();
    }

    private void OnMouseExit()
    {
        XRHoverExit();
    }

    // =========================================================
    // PROXIMITY PREVIEW
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearby = true;
            UpdateVisual();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearby = false;
            UpdateVisual();
        }
    }

    // =========================================================
    // SELECTION VISUAL
    // =========================================================

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (_material == null)
            return;

        if (_isSelected)
            _material.color = _selectedColor;
        else if (_isNearby)
            _material.color = _proximityColor;
        else
            _material.color = _normalColor;
    }
}