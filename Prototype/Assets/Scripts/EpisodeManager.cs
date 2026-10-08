using UnityEngine;
using TMPro;

public class EpisodeManager : MonoBehaviour
{
    // =========================================================
    // EPISODE DATA
    //
    // 每一个 Episode 现在拥有：
    // 1. Title
    // 2. Duration
    // 3. Cover
    // 4. 自己对应的 Chapters
    // =========================================================

    [System.Serializable]
    public class EpisodeData
    {
        public string title;
        public string duration;
        public Material coverMaterial;

        // 每一个 Episode 自己的 Chapter 数据
        public ChapterManager.ChapterData[] chapters;
    }


    // =========================================================
    // EPISODE CATEGORY
    //
    // 三个浏览模式：
    // AI
    // Emotional
    // LikeList
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
    //
    // Episode Room 中间主卡片显示的内容
    // =========================================================

    [Header("Current Episode UI")]
    public TMP_Text titleText;
    public TMP_Text durationText;
    public Renderer coverRenderer;


    // =========================================================
    // CHAPTER PORTAL
    //
    // 用户 G 确认 Episode 后，
    // 把 Selected Episode 的 Chapter 数据交给 ChapterManager。
    // =========================================================

    [Header("Chapter Portal")]
    public ChapterManager chapterManager;


    // =========================================================
    // XR PORTAL
    //
    // 保留你现在已经成功的 Teleport 系统
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
    // CURRENT BROWSING STATE
    // =========================================================

    private int currentIndex = 0;


    public System.Action OnEpisodeChanged;


    public int CurrentIndex =>
        currentIndex;


    // =========================================================
    // SELECTED EPISODE
    //
    // 用户最后通过 G 确认的 Episode。
    //
    // 后面的 Chapter Portal、
    // Podcast Experience、
    // Now Playing
    // 都应该使用这一份 SelectedEpisode。
    // =========================================================

    private EpisodeData selectedEpisode = null;


    private EpisodeCategory selectedEpisodeCategory;


    public EpisodeData SelectedEpisode =>
        selectedEpisode;


    public EpisodeCategory SelectedEpisodeCategory =>
        selectedEpisodeCategory;


    // =========================================================
    // FAVOURITE EPISODE
    //
    // 当前 Prototype 只保存一个 Favourite。
    // 新 Favourite 会替换旧 Favourite。
    // =========================================================

    private EpisodeData favouriteEpisode = null;


    private EpisodeCategory favouriteCategory;


    private bool hasFavourite = false;


    public EpisodeData FavouriteEpisode =>
        favouriteEpisode;


    public EpisodeCategory FavouriteCategory =>
        favouriteCategory;


    public bool HasFavourite =>
        hasFavourite;


    // =========================================================
    // LIKE LIST DATASET
    //
    // Prototype 当前只有一个 Favourite，
    // 所以 Like List 动态建立一个只有一集的 Array。
    // =========================================================

    private EpisodeData[] FavouriteEpisodes
    {
        get
        {
            if (!hasFavourite ||
                favouriteEpisode == null)
            {
                return new EpisodeData[0];
            }


            return new EpisodeData[]
            {
                favouriteEpisode
            };
        }
    }


    // =========================================================
    // CURRENT EPISODES
    //
    // 根据 Topic / Category 决定 Episode Room
    // 当前正在浏览哪一个 Dataset。
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
    // PodcastCarousel 用这个方法读取
    // Previous / Next Episode。
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
    //
    // 只有存在 Favourite 时，
    // 才允许进入 Like List。
    // =========================================================

    public void SetLikeList()
    {
        if (!hasFavourite ||
            favouriteEpisode == null)
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
        currentCategory =
            category;


        // 每次切换 Topic，
        // 从这个 Dataset 的第一集开始显示。
        currentIndex = 0;


        ShowEpisode();


        Debug.Log(
            "Episode Category Changed: " +
            currentCategory
        );
    }


    // =========================================================
    // NEXT EPISODE
    //
    // Swipe Left 或 Next Card 使用。
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


        if (currentIndex >=
            episodes.Length)
        {
            currentIndex = 0;
        }


        ShowEpisode();
    }


    // =========================================================
    // PREVIOUS EPISODE
    //
    // Swipe Right 或 Previous Card 使用。
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
    // 用户在 Episode Room 使用 G 确认当前 Episode。
    //
    // 执行顺序：
    //
    // 1. 保存 Selected Episode
    // 2. 保存它原本属于哪个 Category
    // 3. 把 Episode Chapters 同步给 ChapterManager
    // 4. Teleport 到 Chapter Portal
    // 5. 打开 Navigation Panel
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
        // STEP 2
        // 保存 Episode 原本的 Category
        //
        // 如果从 Like List 进入，
        // 不能把它记录成 LikeList。
        //
        // 例如：
        // Slow Down 原本属于 Emotional，
        // 从 Like List 再次选择时，
        // Category 仍然应该是 Emotional。
        // -----------------------------------------------------

        if (currentCategory ==
            EpisodeCategory.LikeList)
        {
            selectedEpisodeCategory =
                favouriteCategory;
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
        // STEP 3
        // 把当前 Selected Episode
        // 同步给 Chapter Portal。
        //
        // ChapterManager 会读取：
        // selectedEpisode.chapters
        //
        // 然后刷新 ChapterStation_0 ~ 3。
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
        // STEP 4
        // Teleport 到 Chapter Portal
        // -----------------------------------------------------

        TeleportXRPlayerToChapter();


        // -----------------------------------------------------
        // STEP 5
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
    // Heart 放进 Favourite Area 后调用。
    // =========================================================

    public bool SaveSelectedEpisodeAsFavourite()
    {
        if (selectedEpisode == null)
        {
            Debug.LogWarning(
                "EpisodeManager: Cannot save Favourite because " +
                "no Episode has been selected."
            );


            return false;
        }


        favouriteEpisode =
            selectedEpisode;


        favouriteCategory =
            selectedEpisodeCategory;


        hasFavourite = true;


        Debug.Log(
            "FAVOURITE SAVED: " +
            favouriteEpisode.title +
            " | Category: " +
            favouriteCategory
        );


        Debug.Log(
            "EpisodeManager HasFavourite = " +
            hasFavourite
        );


        return true;
    }


    // =========================================================
    // TELEPORT TO CHAPTER PORTAL
    //
    // 保留你之前已经成功的 XR Teleport 逻辑。
    //
    // 不 Disable CharacterController，
    // 避免 Step Offset Error。
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
        // 旋转整个 XR Origin，
        // 让玩家进入 Chapter Portal 后朝向正确方向。
        // -----------------------------------------------------

        if (cameraForward.sqrMagnitude >
                0.001f &&
            targetForward.sqrMagnitude >
                0.001f)
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
        // 旋转以后重新读取 Camera 位置
        // -----------------------------------------------------

        Vector3 cameraPosition =
            xrCamera.position;


        // -----------------------------------------------------
        // STEP 5
        // 计算 Camera 到 Chapter Target 的水平距离
        // -----------------------------------------------------

        Vector3 correction =
            chapterTarget.position -
            cameraPosition;


        // Floor Tracking：
        // 不修改玩家高度
        correction.y = 0f;


        // -----------------------------------------------------
        // STEP 6
        // 移动整个 XR Origin
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
    // 更新 Episode Room 当前中间 Episode Card。
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
        // 通知 PodcastCarousel：
        // 当前 Episode 已改变，
        // Previous / Next Card 也需要刷新。
        // -----------------------------------------------------

        OnEpisodeChanged?.Invoke();
    }
}