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

    [Header("Quest 2 Trigger Swipe")]

    [Tooltip("右手 Controller Transform")]
    [SerializeField]
    private Transform rightController;

    [Tooltip("右手 Trigger Input Action")]
    [SerializeField]
    private InputActionReference triggerAction;

    [Tooltip("手柄水平移动多少米触发 Swipe")]
    [SerializeField]
    private float xrSwipeThreshold = 0.05f;


    // =====================================================
    // INTERNAL STATE
    // =====================================================

    private PodcastCarousel carousel;

    private bool isMouseDragging;
    private float mouseStartX;

    // Trigger 是否正在按住
    private bool isXRDragging = false;

    // 本次按住 Trigger 是否已经切换 Episode
    private bool hasSwiped = false;

    // 手柄起始位置
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
    // QUEST 2: TRIGGER + HORIZONTAL MOVEMENT
    // =====================================================

    private void HandleXRSwipe()
    {
        if (rightController == null ||
            triggerAction == null ||
            triggerAction.action == null)
        {
            return;
        }

        InputAction action = triggerAction.action;

        // -------------------------------------------------
        // Trigger 按下：开始记录手柄位置
        // -------------------------------------------------

        if (action.WasPressedThisFrame())
        {
            xrStartPosition = rightController.position;

            isXRDragging = true;
            hasSwiped = false;

            Debug.Log("QUEST SWIPE → TRIGGER PRESSED");
        }

        // -------------------------------------------------
        // Trigger 按住：实时检测左右移动
        // -------------------------------------------------

        if (isXRDragging && !hasSwiped)
        {
            Vector3 movement =
                rightController.position - xrStartPosition;

            // 根据玩家视角判断左右方向
            Camera mainCamera = Camera.main;

            Vector3 rightDirection =
                mainCamera != null
                    ? mainCamera.transform.right
                    : Vector3.right;

            rightDirection.y = 0f;
            rightDirection.Normalize();

            float horizontalMovement =
                Vector3.Dot(movement, rightDirection);

            // 移动达到 5 cm，立即切换 Episode
            if (Mathf.Abs(horizontalMovement) >= xrSwipeThreshold)
            {
                // 左滑 → 下一集
                // 右滑 → 上一集

                carousel.Swipe(
                    horizontalMovement < 0f ? 1 : -1
                );

                // 本次按住 Trigger 只能切换一次
                hasSwiped = true;

                Debug.Log(
                    "QUEST SWIPE → SUCCESS: " +
                    horizontalMovement
                );
            }
        }

        // -------------------------------------------------
        // Trigger 松开：重置状态
        // -------------------------------------------------

        if (action.WasReleasedThisFrame())
        {
            isXRDragging = false;
            hasSwiped = false;

            Debug.Log("QUEST SWIPE → TRIGGER RELEASED");
        }
    }
}