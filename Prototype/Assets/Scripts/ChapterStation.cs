using UnityEngine;
using TMPro;

public class ChapterStation : MonoBehaviour
{
    // =========================================================
    // STATION DATA
    // =========================================================

    [Header("Station Data")]
    [SerializeField]
    private int _chapterIndex;


    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]

    [SerializeField]
    private ChapterManager _chapterManager;

    [SerializeField]
    private ChapterStationController _controller;

    [SerializeField]
    private TMP_Text _titleLabel;

    [SerializeField]
    private TMP_Text _timeLabel;


    // =========================================================
    // VISUAL HIGHLIGHT
    // =========================================================

    [Header("Visual Highlight")]

    [SerializeField]
    private Renderer _stationRenderer;


    [SerializeField]
    private Color _normalColor =
        new Color(
            0.4196f,
            0.4863f,
            0.5765f,
            1f
        );


    [SerializeField]
    private Color _proximityColor =
        new Color(
            0.6078f,
            0.4863f,
            0.9412f,
            1f
        );


    [SerializeField]
    private Color _selectedColor =
        new Color(
            0.1804f,
            0.8000f,
            0.4431f,
            1f
        );


    private Material _material;

    private bool _isSelected;

    private bool _isNearby;


    public int ChapterIndex =>
        _chapterIndex;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (_stationRenderer != null)
        {
            _material =
                _stationRenderer.material;
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        RefreshContent();

        UpdateVisual();
    }


    // =========================================================
    // REFRESH CONTENT
    //
    // ChapterManager 在 Episode 改变以后调用。
    // =========================================================

    public void RefreshContent()
    {
        if (_chapterManager == null)
            return;


        if (_chapterManager.chapters == null)
            return;


        if (_chapterIndex < 0 ||
            _chapterIndex >=
            _chapterManager.chapters.Length)
        {
            // 如果这个 Episode 没有对应的 Chapter，
            // 清空这个 Station。
            if (_titleLabel != null)
            {
                _titleLabel.text = "";
            }


            if (_timeLabel != null)
            {
                _timeLabel.text = "";
            }


            return;
        }


        ChapterManager.ChapterData data =
            _chapterManager
                .chapters[_chapterIndex];


        if (_titleLabel != null)
        {
            _titleLabel.text =
                data.title;
        }


        if (_timeLabel != null)
        {
            _timeLabel.text =
                data.time;
        }
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
        {
            _chapterManager.ShowChapter(
                _chapterIndex
            );
        }


        if (_controller != null)
        {
            _controller.SetSelected(this);
        }


        Debug.Log(
            "XR Selected Chapter: " +
            _chapterIndex
        );
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

    private void OnTriggerEnter(
        Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearby = true;

            UpdateVisual();
        }
    }


    private void OnTriggerExit(
        Collider other)
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
        {
            _material.color =
                _selectedColor;
        }
        else if (_isNearby)
        {
            _material.color =
                _proximityColor;
        }
        else
        {
            _material.color =
                _normalColor;
        }
    }
}