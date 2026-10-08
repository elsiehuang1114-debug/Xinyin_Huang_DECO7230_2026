using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PodcastCarousel))]
public class SwipeInput : MonoBehaviour
{
    // =====================================================
    // DESKTOP SETTINGS
    // =====================================================

    [Header("Desktop Swipe")]
    [SerializeField]
    private float swipeThresholdPixels = 60f;

    // =====================================================
    // QUEST 2 SETTINGS
    // =====================================================

    [Header("Quest 2 Controller")]

    [Tooltip("右手手柄 Transform")]
    [SerializeField]
    private Transform rightController;

    [Tooltip("右手 Trigger 输入")]
    [SerializeField]
    private InputActionReference triggerAction;

    [Tooltip("手柄水平移动多少米才算一次滑动")]
    [SerializeField]
    private float xrSwipeThreshold = 0.15f;

    // =====================================================
    // INTERNAL STATE
    // =====================================================

    private PodcastCarousel carousel;

    private bool isMouseDragging;
    private float mouseStartX;

    private bool isXRDragging;
    private Vector3 xrStartPosition;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        carousel = GetComponent<PodcastCarousel>();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        HandleArrowKeys();
        HandleMouseDrag();
        HandleXRSwipe();
    }

    // =====================================================
    // DESKTOP: KEYBOARD
    // =====================================================

    private void HandleArrowKeys()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            carousel.Swipe(1);
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            carousel.Swipe(-1);
        }
    }

    // =====================================================
    // DESKTOP: MOUSE
    // =====================================================

    private void HandleMouseDrag()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            mouseStartX =
                Mouse.current.position.ReadValue().x;

            isMouseDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame &&
            isMouseDragging)
        {
            isMouseDragging = false;

            float delta =
                Mouse.current.position.ReadValue().x
                - mouseStartX;

            if (Mathf.Abs(delta) >= swipeThresholdPixels)
            {
                carousel.Swipe(delta < 0f ? 1 : -1);
            }
        }
    }

    // =====================================================
    // QUEST 2: CONTROLLER SWIPE
    // =====================================================

    private void HandleXRSwipe()
    {
        if (rightController == null ||
            triggerAction == null ||
            triggerAction.action == null)
        {
            return;
        }

        // Trigger 按下时，记录手柄起点
        if (triggerAction.action.WasPressedThisFrame())
        {
            xrStartPosition = rightController.position;
            isXRDragging = true;
        }

        // Trigger 松开时，计算手柄移动距离
        if (triggerAction.action.WasReleasedThisFrame() &&
            isXRDragging)
        {
            isXRDragging = false;

            Vector3 movement =
                rightController.position - xrStartPosition;

            // 使用玩家视角的右方向，
            // 避免玩家转身后左右方向错误
            Camera mainCamera = Camera.main;

            Vector3 rightDirection =
                mainCamera != null
                ? mainCamera.transform.right
                : Vector3.right;

            rightDirection.y = 0f;
            rightDirection.Normalize();

            float horizontalMovement =
                Vector3.Dot(movement, rightDirection);

            if (Mathf.Abs(horizontalMovement) >=
                xrSwipeThreshold)
            {
                // 向左 → 下一集
                // 向右 → 上一集
                carousel.Swipe(
                    horizontalMovement < 0f ? 1 : -1
                );

                Debug.Log(
                    "QUEST 2 SWIPE → " +
                    horizontalMovement
                );
            }
        }
    }
}