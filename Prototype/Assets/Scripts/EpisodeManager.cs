using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EpisodeManager : MonoBehaviour
{
    // =========================================================
    // EPISODE DATA
    // =========================================================

    [System.Serializable]
    public class EpisodeData
    {
        public string title;
        public string duration;
        public Material coverMaterial;

        // 每个 Episode 自己的 Chapter 数据
        public ChapterManager.ChapterData[] chapters;
    }

    // =========================================================
    // EPISODE CATEGORY
    // =========================================================

    public enum EpisodeCategory
    {
        AI,
        Emotional,
        LikeList
    }

    [Header("Current Episode Category")]
    public EpisodeCategory currentCategory =
        EpisodeCategory.AI;

    // =========================================================
    // EPISODE DATASETS
    // =========================================================

    [Header("AI Episodes")]
    public EpisodeData[] aiEpisodes;

    [Header("Emotional Episodes")]
    public EpisodeData[] emotionalEpisodes;

    // =========================================================
    // CURRENT EPISODE UI
    // =========================================================

    [Header("Current Episode UI")]
    public TMP_Text titleText;
    public TMP_Text durationText;
    public Renderer coverRenderer;

    // =========================================================
    // CHAPTER PORTAL
    // =========================================================

    [Header("Chapter Portal")]
    public ChapterManager chapterManager;

    // =========================================================
    // XR PORTAL
    // =========================================================

    [Header("XR Portal")]
    public Transform xrOrigin;
    public Transform xrCamera;
    public Transform chapterTarget;

    // =========================================================
    // NAVIGATION
    // =========================================================

    [Header("Navigation")]
    public GameObject navigationPanel;

    // =========================================================
    // HEART FAVOURITE DISPLAY
    //
    // 新增：
    // 玩家选择 Episode 后，
    // 自动刷新 Heart 的收藏状态。
    // =========================================================

    [Header("Heart Favourite Display")]
    public HeartDrag heartDrag;

    // =========================================================
    // CURRENT BROWSING STATE
    // =========================================================

    private int currentIndex = 0;

    public System.Action OnEpisodeChanged;

    public int CurrentIndex =>
        currentIndex;

    // =========================================================
    // SELECTED EPISODE
    // =========================================================

    private EpisodeData selectedEpisode = null;

    private EpisodeCategory selectedEpisodeCategory;

    public EpisodeData SelectedEpisode =>
        selectedEpisode;

    public EpisodeCategory SelectedEpisodeCategory =>
        selectedEpisodeCategory;

    // =========================================================
    // FAVOURITE EPISODES
    //
    // 最多保存 6 个 Episode。
    // 每个 Episode 独立收藏。
    // =========================================================

    private const int MaxFavourites = 6;

    private readonly List<EpisodeData> favouriteEpisodes =
        new List<EpisodeData>();

    // =========================================================
    // FAVOURITE COMPATIBILITY
    //
    // 保留旧属性，兼容其他脚本。
    // =========================================================

    public EpisodeData FavouriteEpisode =>
        favouriteEpisodes.Count > 0
            ? favouriteEpisodes[0]
            : null;

    public EpisodeCategory FavouriteCategory =>
        GetOriginalCategory(FavouriteEpisode);

    public bool HasFavourite =>
        favouriteEpisodes.Count > 0;

    public int FavouriteCount =>
        favouriteEpisodes.Count;

    // =========================================================
    // LIKE LIST DATASET
    // =========================================================

    private EpisodeData[] FavouriteEpisodes =>
        favouriteEpisodes.ToArray();

    // =========================================================
    // GET ORIGINAL CATEGORY
    //
    // 从 LikeList 进入时，
    // 仍能识别 Episode 原本属于哪个分类。
    // =========================================================

    private EpisodeCategory GetOriginalCategory(
        EpisodeData episode)
    {
        if (episode == null)
        {
            return EpisodeCategory.AI;
        }

        if (emotionalEpisodes != null &&
            System.Array.IndexOf(
                emotionalEpisodes,
                episode
            ) >= 0)
        {
            return EpisodeCategory.Emotional;
        }

        return EpisodeCategory.AI;
    }

    // =========================================================
    // CHECK FAVOURITE
    // =========================================================

    public bool IsSelectedEpisodeFavourite()
    {
        return selectedEpisode != null &&
               favouriteEpisodes.Contains(selectedEpisode);
    }

    // =========================================================
    // REMOVE FAVOURITE
    //
    // Heart 放回 Shelf 时调用。
    // =========================================================

    public bool RemoveSelectedEpisodeFromFavourite()
    {
        if (selectedEpisode == null)
        {
            Debug.LogWarning(
                "EpisodeManager: Cannot remove Favourite " +
                "because no Episode is selected."
            );

            return false;
        }

        bool removed =
            favouriteEpisodes.Remove(selectedEpisode);

        if (!removed)
        {
            Debug.Log(
                "EPISODE IS NOT FAVOURITED: " +
                selectedEpisode.title
            );

            return false;
        }

        Debug.Log(
            "FAVOURITE REMOVED: " +
            selectedEpisode.title +
            " | Remaining: " +
            favouriteEpisodes.Count
        );

        // 通知 Carousel 更新
        OnEpisodeChanged?.Invoke();

        return true;
    }

    // =========================================================
    // CURRENT EPISODES
    // =========================================================

    private EpisodeData[] CurrentEpisodes
    {
        get
        {
            if (currentCategory ==
                EpisodeCategory.Emotional)
            {
                return emotionalEpisodes;
            }

            if (currentCategory ==
                EpisodeCategory.LikeList)
            {
                return FavouriteEpisodes;
            }

            return aiEpisodes;
        }
    }

    // =========================================================
    // EPISODE COUNT
    // =========================================================

    public int EpisodeCount =>
        CurrentEpisodes == null
            ? 0
            : CurrentEpisodes.Length;

    // =========================================================
    // GET EPISODE
    //
    // PodcastCarousel 用于读取前后卡片。
    // =========================================================

    public EpisodeData GetEpisode(int i)
    {
        EpisodeData[] episodes =
            CurrentEpisodes;

        if (episodes == null ||
            episodes.Length == 0)
        {
            return null;
        }

        int idx =
            ((i % episodes.Length) +
             episodes.Length)
            % episodes.Length;

        return episodes[idx];
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ShowEpisode();
    }

    // =========================================================
    // CATEGORY SWITCHING
    // =========================================================

    public void SetAI()
    {
        SetCategory(
            EpisodeCategory.AI
        );
    }

    public void SetEmotional()
    {
        SetCategory(
            EpisodeCategory.Emotional
        );
    }

    // =========================================================
    // SET LIKE LIST
    // =========================================================

    public void SetLikeList()
    {
        if (!HasFavourite)
        {
            Debug.Log(
                "LIKE LIST EMPTY: No saved episodes."
            );

            return;
        }

        SetCategory(
            EpisodeCategory.LikeList
        );
    }

    // =========================================================
    // SET CATEGORY
    // =========================================================

    private void SetCategory(
        EpisodeCategory category)
    {
        currentCategory = category;

        // 每次切换 Topic，从第一集开始
        currentIndex = 0;

        ShowEpisode();

        Debug.Log(
            "Episode Category Changed: " +
            currentCategory
        );
    }

    // =========================================================
    // NEXT EPISODE
    // =========================================================

    public void NextEpisode()
    {
        EpisodeData[] episodes =
            CurrentEpisodes;

        if (episodes == null ||
            episodes.Length == 0)
        {
            return;
        }

        currentIndex++;

        if (currentIndex >= episodes.Length)
        {
            currentIndex = 0;
        }

        ShowEpisode();
    }

    // =========================================================
    // PREVIOUS EPISODE
    // =========================================================

    public void PreviousEpisode()
    {
        EpisodeData[] episodes =
            CurrentEpisodes;

        if (episodes == null ||
            episodes.Length == 0)
        {
            return;
        }

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex =
                episodes.Length - 1;
        }

        ShowEpisode();
    }

    // =========================================================
    // SELECT CURRENT EPISODE
    //
    // 用户在 Episode Room 确认 Episode。
    //
    // 1. 保存 Selected Episode
    // 2. 刷新 Heart 收藏状态（新增）
    // 3. 保存原始 Category
    // 4. 同步 Chapters
    // 5. Teleport 到 Chapter Portal
    // 6. 显示 Navigation Panel
    // =========================================================

    public void SelectCurrentEpisode()
    {
        EpisodeData[] episodes =
            CurrentEpisodes;

        if (episodes == null ||
            episodes.Length == 0)
        {
            return;
        }

        // -----------------------------------------------------
        // STEP 1
        // 保存当前 Episode
        // -----------------------------------------------------

        selectedEpisode =
            episodes[currentIndex];

        // -----------------------------------------------------
        // STEP 2（新增）
        // 根据当前 Episode 的收藏状态刷新 Heart
        //
        // 已收藏：
        // Heart 显示在绿色平台。
        //
        // 未收藏：
        // Heart 显示在 Shelf。
        // -----------------------------------------------------

        if (heartDrag != null)
        {
            heartDrag.RefreshHeartState();

            Debug.Log(
                "EPISODE SELECTED → HEART STATE REFRESHED"
            );
        }

        // -----------------------------------------------------
        // STEP 3
        // 保存 Episode 原本的 Category
        // -----------------------------------------------------

        if (currentCategory ==
            EpisodeCategory.LikeList)
        {
            selectedEpisodeCategory =
                GetOriginalCategory(selectedEpisode);
        }
        else
        {
            selectedEpisodeCategory =
                currentCategory;
        }

        Debug.Log(
            "SELECTED EPISODE SAVED: " +
            selectedEpisode.title +
            " | Original Category: " +
            selectedEpisodeCategory
        );

        // -----------------------------------------------------
        // STEP 4
        // 同步 Chapter 数据
        // -----------------------------------------------------

        if (chapterManager != null)
        {
            chapterManager.LoadEpisode(
                selectedEpisode
            );
        }
        else
        {
            Debug.LogWarning(
                "EpisodeManager: ChapterManager is missing."
            );
        }

        // -----------------------------------------------------
        // STEP 5
        // Teleport 到 Chapter Portal
        //
        // 保留原来的 XR Teleport 算法。
        // -----------------------------------------------------

        TeleportXRPlayerToChapter();

        // -----------------------------------------------------
        // STEP 6
        // 显示 Navigation Panel
        // -----------------------------------------------------

        if (navigationPanel != null)
        {
            navigationPanel.SetActive(true);
        }
    }

    // =========================================================
    // SAVE FAVOURITE
    //
    // 最多收藏 6 个 Episode。
    // =========================================================

    public bool SaveSelectedEpisodeAsFavourite()
    {
        if (selectedEpisode == null)
        {
            Debug.LogWarning(
                "EpisodeManager: Cannot save Favourite " +
                "because no Episode has been selected."
            );

            return false;
        }

        // 已收藏：不重复添加
        if (favouriteEpisodes.Contains(selectedEpisode))
        {
            Debug.Log(
                "EPISODE ALREADY FAVOURITED: " +
                selectedEpisode.title
            );

            return true;
        }

        // 最多收藏 6 个
        if (favouriteEpisodes.Count >= MaxFavourites)
        {
            Debug.LogWarning(
                "FAVOURITE LIST FULL: Maximum 6 episodes."
            );

            return false;
        }

        // 添加收藏
        favouriteEpisodes.Add(selectedEpisode);

        Debug.Log(
            "FAVOURITE SAVED: " +
            selectedEpisode.title +
            " | Total: " +
            favouriteEpisodes.Count
        );

        // 通知 Carousel 更新
        OnEpisodeChanged?.Invoke();

        return true;
    }

    // =========================================================
    // TELEPORT TO CHAPTER PORTAL
    //
    // 保留原来的 XR Teleport 算法。
    //
    // 不禁用 CharacterController。
    // 不修改 XR Floor Tracking 高度。
    // =========================================================

    private void TeleportXRPlayerToChapter()
    {
        if (xrOrigin == null ||
            xrCamera == null ||
            chapterTarget == null)
        {
            Debug.LogWarning(
                "EpisodeManager: XR teleport references are missing."
            );

            return;
        }

        // -----------------------------------------------------
        // STEP 1
        // 获取玩家 Camera 当前朝向
        // -----------------------------------------------------

        Vector3 cameraForward =
            xrCamera.forward;

        cameraForward.y = 0f;

        // -----------------------------------------------------
        // STEP 2
        // 获取 ChapterTeleportTarget 朝向
        // -----------------------------------------------------

        Vector3 targetForward =
            chapterTarget.forward;

        targetForward.y = 0f;

        // -----------------------------------------------------
        // STEP 3
        // 旋转 XR Origin
        // -----------------------------------------------------

        if (cameraForward.sqrMagnitude > 0.001f &&
            targetForward.sqrMagnitude > 0.001f)
        {
            float angle =
                Vector3.SignedAngle(
                    cameraForward,
                    targetForward,
                    Vector3.up
                );

            xrOrigin.RotateAround(
                xrCamera.position,
                Vector3.up,
                angle
            );
        }

        // -----------------------------------------------------
        // STEP 4
        // 重新读取 Camera 位置
        // -----------------------------------------------------

        Vector3 cameraPosition =
            xrCamera.position;

        // -----------------------------------------------------
        // STEP 5
        // 计算水平位置差
        // -----------------------------------------------------

        Vector3 correction =
            chapterTarget.position -
            cameraPosition;

        // 保留 XR Floor Tracking 高度
        correction.y = 0f;

        // -----------------------------------------------------
        // STEP 6
        // 移动 XR Origin
        // -----------------------------------------------------

        xrOrigin.position +=
            correction;

        Debug.Log(
            "EPISODE TELEPORT | " +
            "Target = " +
            chapterTarget.position +
            " | Camera = " +
            xrCamera.position +
            " | XR Origin = " +
            xrOrigin.position
        );
    }

    // =========================================================
    // SHOW EPISODE
    //
    // 更新 Episode Room 中间卡片。
    // =========================================================

    private void ShowEpisode()
    {
        EpisodeData[] episodes =
            CurrentEpisodes;

        if (episodes == null ||
            episodes.Length == 0)
        {
            Debug.Log(
                "SHOW EPISODE: Current dataset is empty."
            );

            return;
        }

        // 防止 Index 超出范围
        if (currentIndex < 0 ||
            currentIndex >= episodes.Length)
        {
            currentIndex = 0;
        }

        EpisodeData episode =
            episodes[currentIndex];

        // -----------------------------------------------------
        // TITLE
        // -----------------------------------------------------

        if (titleText != null)
        {
            titleText.text =
                episode.title;
        }

        // -----------------------------------------------------
        // DURATION
        // -----------------------------------------------------

        if (durationText != null)
        {
            durationText.text =
                episode.duration;
        }

        // -----------------------------------------------------
        // COVER
        // -----------------------------------------------------

        if (coverRenderer != null &&
            episode.coverMaterial != null)
        {
            coverRenderer.material =
                episode.coverMaterial;
        }

        // -----------------------------------------------------
        // 通知 PodcastCarousel 刷新卡片
        // -----------------------------------------------------

        OnEpisodeChanged?.Invoke();
    }
}