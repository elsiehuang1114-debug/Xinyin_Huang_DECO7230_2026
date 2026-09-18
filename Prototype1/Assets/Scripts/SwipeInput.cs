using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Desktop swipe detector for the podcast carousel.
/// Detects horizontal mouse drag and Left/Right arrow keys,
/// then calls PodcastCarousel.Swipe().
/// Uses Unity's new Input System.
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

    // ── Keyboard ─────────────────────────────────────────────

    private void HandleArrowKeys()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            _carousel.Swipe(1); // next
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            _carousel.Swipe(-1); // previous
        }
    }

    // ── Mouse ────────────────────────────────────────────────

    private void HandleMouseDrag()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _dragStartX = Mouse.current.position.ReadValue().x;
            _isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
        {
            _isDragging = false;

            float currentX = Mouse.current.position.ReadValue().x;
            float delta = currentX - _dragStartX;

            if (Mathf.Abs(delta) >= _swipeThresholdPixels)
            {
                // Swipe left → next
                // Swipe right → previous
                _carousel.Swipe(delta < 0f ? 1 : -1);
            }
        }
    }
}