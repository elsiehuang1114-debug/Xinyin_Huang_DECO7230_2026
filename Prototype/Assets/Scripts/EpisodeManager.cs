using UnityEngine;
using TMPro;

public class EpisodeManager : MonoBehaviour
{
    // =========================================================
    // EPISODE DATA
    // 每一个 Episode 包含标题、时长和封面 Material
    // =========================================================

    [System.Serializable]
    public class EpisodeData
    {
        public string title;
        public string duration;
        public Material coverMaterial;
    }


    // =========================================================
    // EPISODE CATEGORY
    // 用来记录用户当前浏览的是哪一种 Episode
    // AI = 从 AI Topic Door 进入
    // Emotional = 从 Emotional Door 进入
    // =========================================================

    public enum EpisodeCategory
    {
        AI,
        Emotional
    }

    [Header("Current Episode Category")]
    public EpisodeCategory currentCategory = EpisodeCategory.AI;


    // =========================================================
    // 两套 EPISODE DATA
    // 两种 Category 共用同一个 Episode Room，
    // 只是根据 currentCategory 显示不同的数据
    // =========================================================

    [Header("AI Episodes")]
    public EpisodeData[] aiEpisodes;

    [Header("Emotional Episodes")]
    public EpisodeData[] emotionalEpisodes;


    // =========================================================
    // CURRENT EPISODE UI
    // 控制中间 Current Episode 的标题、时长和封面
    // =========================================================

    [Header("Current Episode UI")]
    public TMP_Text titleText;
    public TMP_Text durationText;
    public Renderer coverRenderer;


    // =========================================================
    // XR PORTAL
    // 用于选择 Episode 后传送到 Chapter Portal
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


    // 当前正在浏览第几个 Episode
    private int currentIndex = 0;

    // 当 Episode 改变时通知其他脚本，例如 Episode Carousel
    public System.Action OnEpisodeChanged;

    public int CurrentIndex => currentIndex;


    // =========================================================
    // CURRENT EPISODES
    //
    // 这是这次修改最重要的部分。
    //
    // 以前所有代码直接读取 episodes。
    // 现在改成读取 CurrentEpisodes。
    //
    // CurrentEpisodes 会根据 currentCategory 自动决定：
    //
    // AI         → aiEpisodes
    // Emotional  → emotionalEpisodes
    //
    // 因此不需要复制第二个 Episode Room。
    // =========================================================

    private EpisodeData[] CurrentEpisodes
    {
        get
        {
            if (currentCategory == EpisodeCategory.Emotional)
            {
                return emotionalEpisodes;
            }

            return aiEpisodes;
        }
    }


    // 返回当前 Category 一共有多少个 Episodes
    public int EpisodeCount =>
        CurrentEpisodes == null ? 0 : CurrentEpisodes.Length;


    // =========================================================
    // GET EPISODE
    // Episode Carousel 可以通过这个方法取得指定 Episode
    // =========================================================

    public EpisodeData GetEpisode(int i)
    {
        EpisodeData[] episodes = CurrentEpisodes;

        if (episodes == null || episodes.Length == 0)
            return null;

        // 使用 modulo 让 Episode 可以循环：
        // 例如最后一张再 Next，会回到第一张
        int idx =
            ((i % episodes.Length) + episodes.Length)
            % episodes.Length;

        return episodes[idx];
    }


    // =========================================================
    // START
    // 游戏开始时显示默认 Episode
    // 默认 Category 是 AI
    // =========================================================

    private void Start()
    {
        ShowEpisode();
    }


    // =========================================================
    // CATEGORY SWITCHING
    //
    // 以后 AI Door 会调用 SetAI()
    // Emotional Door 会调用 SetEmotional()
    // =========================================================

    public void SetAI()
    {
        SetCategory(EpisodeCategory.AI);
    }

    public void SetEmotional()
    {
        SetCategory(EpisodeCategory.Emotional);
    }


    // 真正负责切换 Episode Category 的方法
    private void SetCategory(EpisodeCategory category)
    {
        currentCategory = category;

        // 切换 Category 后回到第一张 Episode
        currentIndex = 0;

        // 立即刷新 Episode Room 的内容
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
        EpisodeData[] episodes = CurrentEpisodes;

        if (episodes == null || episodes.Length == 0)
            return;

        currentIndex++;

        // 如果已经超过最后一个 Episode，
        // 就重新回到第一个
        if (currentIndex >= episodes.Length)
            currentIndex = 0;

        ShowEpisode();
    }


    // =========================================================
    // PREVIOUS EPISODE
    // =========================================================

    public void PreviousEpisode()
    {
        EpisodeData[] episodes = CurrentEpisodes;

        if (episodes == null || episodes.Length == 0)
            return;

        currentIndex--;

        // 如果已经在第一张还继续 Previous，
        // 就跳到最后一个 Episode
        if (currentIndex < 0)
            currentIndex = episodes.Length - 1;

        ShowEpisode();
    }


    // =========================================================
    // SELECT CURRENT EPISODE
    // 用户选择当前 Episode 后进入 Chapter Portal
    // =========================================================

    public void SelectCurrentEpisode()
    {
        EpisodeData[] episodes = CurrentEpisodes;

        if (episodes == null || episodes.Length == 0)
            return;

        Debug.Log(
            "Selected Episode: " +
            episodes[currentIndex].title +
            " | Category: " +
            currentCategory
        );

        TeleportXRPlayerToChapter();

        if (navigationPanel != null)
            navigationPanel.SetActive(true);
    }


    // =========================================================
    // TELEPORT TO CHAPTER PORTAL
    //
    // 这一部分保留你原来已经成功工作的逻辑。
    // 没有修改 CharacterController。
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
        // 让玩家进入 Chapter Portal 后，
        // 水平方向朝向 ChapterTeleportTarget 的方向
        // -----------------------------------------------------

        Vector3 cameraForward = xrCamera.forward;
        cameraForward.y = 0f;

        Vector3 targetForward = chapterTarget.forward;
        targetForward.y = 0f;

        if (cameraForward.sqrMagnitude > 0.001f &&
            targetForward.sqrMagnitude > 0.001f)
        {
            float angle = Vector3.SignedAngle(
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
        // STEP 2
        // Rotation 后 Camera 的位置可能发生变化，
        // 所以重新读取 Camera position
        // -----------------------------------------------------

        Vector3 cameraPosition =
            xrCamera.position;


        // -----------------------------------------------------
        // STEP 3
        // 计算玩家当前位置和 Chapter Target 的水平距离
        // -----------------------------------------------------

        Vector3 correction =
            chapterTarget.position -
            cameraPosition;

        // 不修改玩家高度，只进行水平移动
        correction.y = 0f;


        // -----------------------------------------------------
        // STEP 4
        // 移动整个 XR Origin
        //
        // 不关闭 CharacterController，
        // 避免之前出现过的 Step Offset Error。
        // -----------------------------------------------------

        xrOrigin.position += correction;

        Debug.Log(
            "EPISODE TELEPORT | " +
            "Target = " + chapterTarget.position +
            " | Camera = " + xrCamera.position +
            " | XR Origin = " + xrOrigin.position
        );
    }


    // =========================================================
    // SHOW EPISODE
    // 根据 currentCategory + currentIndex
    // 更新当前 Episode 的 UI
    // =========================================================

    private void ShowEpisode()
    {
        EpisodeData[] episodes = CurrentEpisodes;

        if (episodes == null || episodes.Length == 0)
            return;

        // 防止切换 Dataset 后 Index 超出范围
        if (currentIndex < 0 ||
            currentIndex >= episodes.Length)
        {
            currentIndex = 0;
        }

        EpisodeData episode =
            episodes[currentIndex];


        // 更新 Episode Title
        if (titleText != null)
            titleText.text = episode.title;


        // 更新 Episode Duration
        if (durationText != null)
            durationText.text = episode.duration;


        // 更新 Episode Cover
        if (coverRenderer != null &&
            episode.coverMaterial != null)
        {
            coverRenderer.material =
                episode.coverMaterial;
        }


        // 通知 Episode Carousel：
        // Current Episode 已经改变，需要一起刷新左右 Card
        OnEpisodeChanged?.Invoke();
    }
}