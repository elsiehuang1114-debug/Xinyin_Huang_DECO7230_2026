using UnityEngine;

/// <summary>
/// Desktop swipe detector for the podcast carousel.
/// Detects horizontal mouse drag and Left/Right arrow keys, then calls PodcastCarousel.Swipe().
/// Thin layer: replace or supplement with an XR hand-tracking driver later
/// by calling PodcastCarousel.Swipe() directly — no changes to carousel logic needed.
/// </summary>
[RequireComponent(typeof(PodcastCarousel))]
public class SwipeInput : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Minimum horizontal pixel drag distance to register a swipe.")]
    [SerializeField] private float _swipeThresholdPixels = 60f;

    private PodcastCarousel _carousel;
    private bool _isDragging;
    private float _dragStartX;

    private void Awake()
    {
        _carousel = GetComponent<PodcastCarousel>();
    }

    private void Update()
    {
        HandleArrowKeys();
        HandleMouseDrag();
    }

    // ── Input handlers ────────────────────────────────────────────────────

    private void HandleArrowKeys()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) _carousel.Swipe(1);   // right arrow = next
        if (Input.GetKeyDown(KeyCode.LeftArrow))  _carousel.Swipe(-1);  // left arrow  = previous
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartX  = Input.mousePosition.x;
            _isDragging  = true;
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            float delta = Input.mousePosition.x - _dragStartX;

            if (Mathf.Abs(delta) >= _swipeThresholdPixels)
            {
                // Swipe left (delta < 0) → next; swipe right (delta > 0) → previous
                _carousel.Swipe(delta < 0f ? 1 : -1);
            }
        }
    }
}
