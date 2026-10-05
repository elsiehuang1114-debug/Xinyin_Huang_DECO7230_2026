using UnityEngine;

public class HeartDrag : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    private Camera mainCamera;


    // =========================================================
    // EPISODE MANAGER
    // Favourite 数据统一保存在 EpisodeManager
    // =========================================================

    [Header("Episode")]
    public EpisodeManager episodeManager;


    // =========================================================
    // FAVOURITE SNAP POINT
    // Heart 收藏成功后停留的位置
    // =========================================================

    [Header("Favourite Snap Point")]
    public Transform snapPoint;


    // =========================================================
    // FAVOURITE VISUAL FEEDBACK
    //
    // 收藏成功以后：
    // DockingPlatform 会变成绿色
    // =========================================================

    [Header("Favourite Visual Feedback")]
    public Renderer dockingPlatformRenderer;

    public Material savedMaterial;


    // =========================================================
    // DRAG STATE
    // =========================================================

    private bool isMouseDragging = false;
    private bool isXRDragging = false;

    private float dragDistance;

    private Vector3 startPosition;
    private Quaternion startRotation;


    // =========================================================
    // FAVOURITE STATE
    // =========================================================

    private bool isFavourite = false;

    public bool IsFavourite =>
        isFavourite;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        mainCamera = Camera.main;

        // 保存 Heart 在 Heart Shelf 上的初始位置
        startPosition =
            transform.position;

        startRotation =
            transform.rotation;
    }


    // =========================================================
    // XR GRAB START
    // Grip 开始抓 Heart
    // =========================================================

    public void XRGrabStarted()
    {
        if (isFavourite)
            return;

        isXRDragging = true;

        Debug.Log(
            "Heart XR grab started."
        );
    }


    // =========================================================
    // XR GRAB END
    //
    // 如果没有成功收藏，
    // Heart 回到 Heart Shelf
    // =========================================================

    public void XRGrabEnded()
    {
        isXRDragging = false;

        if (!isFavourite)
        {
            ReturnToShelf();
        }

        Debug.Log(
            "Heart XR grab ended."
        );
    }


    // =========================================================
    // DESKTOP MOUSE FALLBACK
    // =========================================================

    private void OnMouseDown()
    {
        if (isFavourite)
            return;

        if (mainCamera == null)
            return;

        isMouseDragging = true;

        dragDistance =
            Vector3.Distance(
                transform.position,
                mainCamera.transform.position
            );
    }


    private void OnMouseDrag()
    {
        if (!isMouseDragging ||
            mainCamera == null)
        {
            return;
        }

        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );

        Vector3 newPosition =
            ray.origin +
            ray.direction * dragDistance;

        transform.position =
            newPosition;
    }


    private void OnMouseUp()
    {
        isMouseDragging = false;

        if (!isFavourite)
        {
            ReturnToShelf();
        }
    }


    // =========================================================
    // FAVOURITE AREA
    //
    // Heart 进入 FavouriteDropArea：
    //
    // 1. 保存当前 Episode
    // 2. Heart Snap 到 HeartSnapPoint
    // 3. DockingPlatform 变绿色
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "Heart entered trigger: " +
            other.name
        );


        // 已经收藏，不重复执行
        if (isFavourite)
            return;


        // Heart 必须正在被用户 Grab / Drag
        if (!isXRDragging &&
            !isMouseDragging)
        {
            return;
        }


        // 必须进入 FavouriteArea
        if (!other.CompareTag("FavouriteArea"))
        {
            return;
        }


        Debug.Log(
            "Favourite Area detected!"
        );


        // =====================================================
        // CHECK EPISODE MANAGER
        // =====================================================

        if (episodeManager == null)
        {
            Debug.LogWarning(
                "HeartDrag: EpisodeManager is missing."
            );

            return;
        }


        // =====================================================
        // SAVE FAVOURITE
        // =====================================================

        bool saved =
            episodeManager
                .SaveSelectedEpisodeAsFavourite();


        // 保存失败就不继续
        if (!saved)
        {
            Debug.LogWarning(
                "HeartDrag: Favourite could not be saved."
            );

            return;
        }


        // =====================================================
        // FAVOURITE SUCCESS
        // =====================================================

        isFavourite = true;

        isXRDragging = false;
        isMouseDragging = false;


        // =====================================================
        // SNAP HEART
        // =====================================================

        if (snapPoint != null)
        {
            transform.position =
                snapPoint.position;

            transform.rotation =
                snapPoint.rotation;

            Debug.Log(
                "Heart snapped to HeartSnapPoint!"
            );
        }
        else
        {
            Debug.LogWarning(
                "HeartDrag: snapPoint is missing!"
            );
        }


        // =====================================================
        // VISUAL CONFIRMATION
        //
        // 收藏成功后 DockingPlatform 变绿色
        // =====================================================

        if (dockingPlatformRenderer != null &&
            savedMaterial != null)
        {
            dockingPlatformRenderer.material =
                savedMaterial;

            Debug.Log(
                "Favourite DockingPlatform changed to GREEN."
            );
        }
        else
        {
            Debug.LogWarning(
                "HeartDrag: DockingPlatform Renderer " +
                "or Saved Material is missing."
            );
        }


        // =====================================================
        // COMPLETE
        // =====================================================

        Debug.Log(
            "HEART FAVOURITE COMPLETE: " +
            episodeManager
                .FavouriteEpisode
                .title
        );
    }


    // =========================================================
    // RETURN TO HEART SHELF
    // =========================================================

    private void ReturnToShelf()
    {
        transform.position =
            startPosition;

        transform.rotation =
            startRotation;
    }
}