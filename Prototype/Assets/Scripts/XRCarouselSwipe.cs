using UnityEngine;

public class XRCarouselSwipe : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    [SerializeField] private PodcastCarousel carousel;

    // 右手 Controller
    [SerializeField] private Transform rightController;


    // =========================================================
    // SWIPE SETTINGS
    // =========================================================

    [Header("Swipe Settings")]

    // Controller 左右移动超过这个距离，
    // 才认为用户是在 Swipe。
    [SerializeField] private float swipeThreshold = 0.15f;


    // =========================================================
    // INTERNAL STATE
    // =========================================================

    private Vector3 startPosition;
    private bool isTracking = false;


    // =========================================================
    // TRIGGER PRESSED
    //
    // T / Trigger 按下时开始记录 Controller 位置。
    //
    // Trigger 现在只负责 Swipe，
    // 不负责选择 Episode。
    // =========================================================

    public void StartInteraction()
    {
        if (rightController == null)
        {
            Debug.LogWarning(
                "XRCarouselSwipe: Right Controller is missing."
            );

            return;
        }

        startPosition = rightController.position;
        isTracking = true;

        Debug.Log(
            "EPISODE SWIPE → START"
        );
    }


    // =========================================================
    // TRIGGER RELEASED
    //
    // T / Trigger 松开以后判断有没有 Swipe。
    //
    // 重要：
    // 即使移动距离很小，也不会选择 Episode，
    // 更不会进入 Chapter Portal。
    // =========================================================

    public void EndInteraction()
    {
        if (!isTracking)
            return;

        isTracking = false;


        if (rightController == null)
            return;


        Vector3 movement =
            rightController.position - startPosition;


        // 使用 EpisodeRoom 自己的左右方向
        float horizontalMovement =
            Vector3.Dot(
                movement,
                transform.right
            );


        Debug.Log(
            "EPISODE SWIPE → END | Movement = " +
            horizontalMovement
        );


        // =====================================================
        // 没有达到 Swipe 距离
        //
        // 什么都不做。
        // 不 Select Episode。
        // 不进入 Chapter Portal。
        // =====================================================

        if (Mathf.Abs(horizontalMovement) < swipeThreshold)
        {
            Debug.Log(
                "EPISODE SWIPE → TOO SMALL / NO ACTION"
            );

            return;
        }


        if (carousel == null)
        {
            Debug.LogWarning(
                "XRCarouselSwipe: PodcastCarousel is missing."
            );

            return;
        }


        // =====================================================
        // LEFT SWIPE → NEXT
        // =====================================================

        if (horizontalMovement < 0f)
        {
            carousel.Swipe(1);

            Debug.Log(
                "XR SWIPE LEFT → NEXT EPISODE"
            );
        }


        // =====================================================
        // RIGHT SWIPE → PREVIOUS
        // =====================================================

        else
        {
            carousel.Swipe(-1);

            Debug.Log(
                "XR SWIPE RIGHT → PREVIOUS EPISODE"
            );
        }
    }
}