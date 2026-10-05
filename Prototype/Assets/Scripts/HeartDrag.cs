using UnityEngine;

public class HeartDrag : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    private Camera mainCamera;


    // =========================================================
    // EPISODE MANAGER
    //
    // Favourite Episode 的真正数据保存在 EpisodeManager。
    // =========================================================

    [Header("Episode")]
    public EpisodeManager episodeManager;


    // =========================================================
    // FAVOURITE SNAP POINT
    //
    // Heart 成功收藏后，
    // 会固定到 Favourite Area 的这个位置。
    // =========================================================

    [Header("Favourite Snap Point")]
    public Transform snapPoint;


    // =========================================================
    // DRAG STATE
    // =========================================================

    private bool isMouseDragging = false;
    private bool isXRDragging = false;

    private float dragDistance;


    // Heart Shelf 原来的位置
    private Vector3 startPosition;
    private Quaternion startRotation;


    // =========================================================
    // HEART FAVOURITE STATE
    //
    // 这里只记录 Heart 是否已经被放进 Favourite Area。
    //
    // Episode Favourite 数据本身不保存在这里，
    // 而是统一保存在 EpisodeManager。
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


        // -----------------------------------------------------
        // 保存 Heart 在 Heart Shelf 上的初始位置。
        //
        // 如果用户 Grab 后没有成功放入 Favourite Area，
        // Heart 会回到这里。
        // -----------------------------------------------------

        startPosition =
            transform.position;

        startRotation =
            transform.rotation;
    }


    // =========================================================
    // XR GRAB START
    //
    // Grip 开始抓 Heart。
    // =========================================================

    public void XRGrabStarted()
    {
        // 已经成功 Favourite 后，
        // 不再允许重新执行 Favourite 流程。
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
    // 如果没有成功进入 Favourite Area，
    // Heart 回到 Heart Shelf。
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
    //
    // 保留原来的 Mouse 测试功能。
    // 不影响 XR Grip。
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
    // Heart 在被用户拖动的时候进入 FavouriteArea：
    //
    // 1. 确认 EpisodeManager 存在
    // 2. 要求 EpisodeManager 保存 Selected Episode
    // 3. 保存成功后才把 Heart 标记为 Favourite
    // 4. Heart Snap 到 Favourite Area
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "Heart entered trigger: " +
            other.name
        );


        // -----------------------------------------------------
        // 已经 Favourite，不重复执行。
        // -----------------------------------------------------

        if (isFavourite)
            return;


        // -----------------------------------------------------
        // Heart 必须正在被用户 Grab / Drag。
        //
        // 防止 Heart 只是碰到 Trigger 就自动收藏。
        // -----------------------------------------------------

        if (!isXRDragging &&
            !isMouseDragging)
        {
            return;
        }


        // -----------------------------------------------------
        // 必须是 FavouriteArea。
        // -----------------------------------------------------

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
        //
        // EpisodeManager 会读取：
        //
        // SelectedEpisode
        // SelectedEpisodeCategory
        //
        // 并保存成 Favourite。
        // =====================================================

        bool saved =
            episodeManager
                .SaveSelectedEpisodeAsFavourite();


        // -----------------------------------------------------
        // 如果保存失败，
        // Heart 不进入 Favourite 状态。
        // -----------------------------------------------------

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
        // SNAP HEART TO FAVOURITE AREA
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
        // CONFIRMATION
        //
        // 目前先使用 Console。
        // 下一步可以做世界空间文字：
        //
        // "Saved to Like List ✓"
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