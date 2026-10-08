using UnityEngine;

public class HeartDrag : MonoBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    private Camera mainCamera;

    [Header("Episode")]
    public EpisodeManager episodeManager;

    [Header("Favourite Snap Point")]
    public Transform snapPoint;

    [Header("Favourite Visual Feedback")]
    public Renderer dockingPlatformRenderer;
    public Material savedMaterial;

    // =====================================================
    // HEART SHELF DROP AREA
    //
    // 用来检测玩家是否把 Heart 放回 Shelf。
    // 下一步在 Unity 中创建并连接。
    // =====================================================

    [Header("Heart Shelf Drop Area")]
    public Collider heartShelfDropArea;

    // =====================================================
    // DRAG STATE
    // =====================================================

    private bool isMouseDragging = false;
    private bool isXRDragging = false;

    private float dragDistance;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Material originalMaterial;

    // =====================================================
    // FAVOURITE STATE
    // =====================================================

    private bool isFavourite = false;

    public bool IsFavourite => isFavourite;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        mainCamera = Camera.main;

        // 保存 Heart 在 Shelf 上的初始位置
        startPosition = transform.position;
        startRotation = transform.rotation;

        // 保存平台默认材质
        if (dockingPlatformRenderer != null)
        {
            originalMaterial =
                dockingPlatformRenderer.sharedMaterial;
        }

        // 根据当前 Episode 的收藏状态显示 Heart
        RefreshHeartState();
    }

    // =====================================================
    // REFRESH HEART STATE
    //
    // 重新选择 Episode 时调用。
    // =====================================================

    public void RefreshHeartState()
    {
        if (episodeManager == null)
            return;

        isFavourite =
            episodeManager.IsSelectedEpisodeFavourite();

        isMouseDragging = false;
        isXRDragging = false;

        if (isFavourite)
        {
            // 已收藏：Heart 显示在绿色平台
            if (snapPoint != null)
            {
                transform.position = snapPoint.position;
                transform.rotation = snapPoint.rotation;
            }

            if (dockingPlatformRenderer != null &&
                savedMaterial != null)
            {
                dockingPlatformRenderer.material =
                    savedMaterial;
            }
        }
        else
        {
            // 未收藏：Heart 返回 Shelf
            ReturnToShelf();

            if (dockingPlatformRenderer != null &&
                originalMaterial != null)
            {
                dockingPlatformRenderer.material =
                    originalMaterial;
            }
        }

        Debug.Log(
            "HEART STATE REFRESHED | Favourite = " +
            isFavourite
        );
    }

    // =====================================================
    // XR GRAB START
    // =====================================================

    public void XRGrabStarted()
    {
        // 已收藏的 Heart 也允许再次抓取
        isXRDragging = true;

        Debug.Log("Heart XR grab started.");
    }

    // =====================================================
    // XR GRAB END
    // =====================================================

    public void XRGrabEnded()
    {
        isXRDragging = false;

        // 如果没有成功放入对应区域，
        // Heart 回到当前状态应该在的位置
        RestoreHeartPosition();
    }

    // =====================================================
    // DESKTOP MOUSE
    // =====================================================

    private void OnMouseDown()
    {
        if (mainCamera == null)
            return;

        isMouseDragging = true;

        dragDistance = Vector3.Distance(
            transform.position,
            mainCamera.transform.position
        );
    }

    private void OnMouseDrag()
    {
        if (!isMouseDragging || mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        transform.position =
            ray.origin + ray.direction * dragDistance;
    }

    private void OnMouseUp()
    {
        isMouseDragging = false;

        RestoreHeartPosition();
    }

    // =====================================================
    // TRIGGER DETECTION
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        // 只有正在拖动时才处理
        if (!isXRDragging && !isMouseDragging)
            return;

        if (episodeManager == null)
            return;

        // -------------------------------------------------
        // ADD FAVOURITE
        // -------------------------------------------------

        if (other.CompareTag("FavouriteArea"))
        {
            if (!isFavourite)
            {
                bool saved =
                    episodeManager
                        .SaveSelectedEpisodeAsFavourite();

                if (saved)
                {
                    isFavourite = true;

                    Debug.Log(
                        "HEART → FAVOURITE ADDED"
                    );
                }
            }

            return;
        }

        // -------------------------------------------------
        // REMOVE FAVOURITE
        // -------------------------------------------------

        if (heartShelfDropArea != null &&
            other == heartShelfDropArea)
        {
            if (isFavourite)
            {
                bool removed =
                    episodeManager
                        .RemoveSelectedEpisodeFromFavourite();

                if (removed)
                {
                    isFavourite = false;

                    Debug.Log(
                        "HEART → FAVOURITE REMOVED"
                    );
                }
            }
        }
    }

    // =====================================================
    // RESTORE HEART POSITION
    //
    // 抓取结束后，根据收藏状态决定位置。
    // =====================================================

    private void RestoreHeartPosition()
    {
        if (isFavourite)
        {
            if (snapPoint != null)
            {
                transform.position = snapPoint.position;
                transform.rotation = snapPoint.rotation;
            }

            if (dockingPlatformRenderer != null &&
                savedMaterial != null)
            {
                dockingPlatformRenderer.material =
                    savedMaterial;
            }
        }
        else
        {
            ReturnToShelf();

            if (dockingPlatformRenderer != null &&
                originalMaterial != null)
            {
                dockingPlatformRenderer.material =
                    originalMaterial;
            }
        }
    }

    // =====================================================
    // RETURN TO SHELF
    // =====================================================

    private void ReturnToShelf()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    // =====================================================
    // COMPATIBILITY
    //
    // 保留之前的 ResetHeart() 方法，
    // 避免 PodcastAudioTrigger 出现编译错误。
    //
    // 现在重置会根据 Episode 收藏状态显示 Heart，
    // 而不是强制取消收藏视觉状态。
    // =====================================================

    public void ResetHeart()
    {
        RefreshHeartState();
    }
}