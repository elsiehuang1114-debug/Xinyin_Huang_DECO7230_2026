using UnityEngine;
using TMPro;

public class ChapterManager : MonoBehaviour
{
    // =========================================================
    // CHAPTER DATA
    // =========================================================

    [System.Serializable]
    public class ChapterData
    {
        public string title;
        public string time;
    }


    // 当前 Selected Episode 对应的 Chapters
    public ChapterData[] chapters;


    // =========================================================
    // CURRENT CHAPTER DOOR
    // =========================================================

    public TMP_Text chapterTitleText;
    public TMP_Text chapterTimeText;


    // =========================================================
    // CHAPTER STATIONS
    //
    // Episode 改变以后刷新四个 Chapter Station。
    // =========================================================

    [Header("Chapter Stations")]
    [SerializeField]
    private ChapterStation[] chapterStations;


    // =========================================================
    // DOOR VISUAL STATE
    // =========================================================

    [Header("Door Visual State")]

    [SerializeField]
    private Renderer doorRenderer;

    [SerializeField]
    private Color doorInactiveColor =
        new Color(
            0.3529f,
            0.4196f,
            0.4902f,
            1f
        );

    [SerializeField]
    private Color doorSelectedColor =
        new Color(
            0.1804f,
            0.8000f,
            0.4431f,
            1f
        );


    private Material doorMaterial;

    private int currentChapterIndex = 0;

    private bool hasSelection = false;


    // =========================================================
    // PUBLIC STATE
    //
    // Experience Room 可以读取当前是否已经选择 Chapter。
    // =========================================================

    public bool HasSelection =>
        hasSelection;


    // =========================================================
    // CHAPTER SELECTED EVENT
    //
    // 当用户选择 Chapter 时，
    // PodcastExperienceDisplay 可以立即刷新显示内容。
    // =========================================================

    public System.Action OnChapterSelected;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (doorRenderer != null)
        {
            doorMaterial =
                doorRenderer.material;

            doorMaterial.color =
                doorInactiveColor;
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ResetChapterSelection();
    }


    // =========================================================
    // LOAD SELECTED EPISODE
    //
    // EpisodeManager 在用户 G 确认 Episode 后调用。
    // =========================================================

    public void LoadEpisode(
        EpisodeManager.EpisodeData episode)
    {
        if (episode == null)
        {
            Debug.LogWarning(
                "ChapterManager: Episode is null."
            );

            return;
        }


        // 使用这个 Episode 自己的 Chapter 数据
        chapters =
            episode.chapters;


        Debug.Log(
            "CHAPTER PORTAL LOADED EPISODE: " +
            episode.title
        );


        // 新 Episode 进入 Chapter Portal 时，
        // 清除上一集的 Chapter Selection。
        ResetChapterSelection();


        // 刷新 ChapterStation_0 ~ 3
        RefreshStations();
    }


    // =========================================================
    // REFRESH STATIONS
    // =========================================================

    private void RefreshStations()
    {
        if (chapterStations == null)
            return;


        foreach (
            ChapterStation station
            in chapterStations)
        {
            if (station != null)
            {
                station.RefreshContent();
            }
        }
    }


    // =========================================================
    // RESET CHAPTER SELECTION
    // =========================================================

    private void ResetChapterSelection()
    {
        currentChapterIndex = 0;

        hasSelection = false;


        // Current Chapter Door 恢复未选择颜色
        if (doorMaterial != null)
        {
            doorMaterial.color =
                doorInactiveColor;
        }


        // 清除上一集留下来的 Chapter Title
        if (chapterTitleText != null)
        {
            chapterTitleText.text =
                "Select a Chapter";
        }


        // 清除上一集留下来的 Chapter Time
        if (chapterTimeText != null)
        {
            chapterTimeText.text = "";
        }


        // 清除所有 Chapter Station 的绿色 Selected 状态
        if (chapterStations != null)
        {
            foreach (
                ChapterStation station
                in chapterStations)
            {
                if (station != null)
                {
                    station.SetSelected(false);
                }
            }
        }
    }


    // =========================================================
    // SHOW CHAPTER
    //
    // 用户选择 Chapter Station 后调用。
    // =========================================================

    public void ShowChapter(int index)
    {
        if (chapters == null ||
            chapters.Length == 0)
        {
            return;
        }


        if (index < 0 ||
            index >= chapters.Length)
        {
            return;
        }


        // 保存当前选择的 Chapter
        currentChapterIndex =
            index;


        hasSelection =
            true;


        // -----------------------------------------------------
        // 更新 Current Chapter Door 的 Chapter Title
        // -----------------------------------------------------

        if (chapterTitleText != null)
        {
            chapterTitleText.text =
                chapters[
                    currentChapterIndex
                ].title;
        }


        // -----------------------------------------------------
        // 更新 Current Chapter Door 的 Chapter Time
        // -----------------------------------------------------

        if (chapterTimeText != null)
        {
            chapterTimeText.text =
                chapters[
                    currentChapterIndex
                ].time;
        }


        // -----------------------------------------------------
        // Chapter 已选择：
        // Current Chapter Door 变绿色
        // -----------------------------------------------------

        if (doorMaterial != null)
        {
            doorMaterial.color =
                doorSelectedColor;
        }


        Debug.Log(
            "Current Chapter: " +
            chapters[
                currentChapterIndex
            ].title
        );


        // -----------------------------------------------------
        // 通知 Podcast Experience：
        // 当前 Chapter 已经改变。
        //
        // 不负责开门。
        // 不负责 Teleport。
        // 不负责移动玩家。
        // -----------------------------------------------------

        OnChapterSelected?.Invoke();
    }


    // =========================================================
    // GET CURRENT CHAPTER INDEX
    // =========================================================

    public int GetCurrentChapterIndex()
    {
        return currentChapterIndex;
    }


    // =========================================================
    // GET CURRENT CHAPTER TITLE
    //
    // Podcast Experience 使用。
    // =========================================================

    public string GetCurrentChapterTitle()
    {
        if (!hasSelection ||
            chapters == null ||
            chapters.Length == 0)
        {
            return "";
        }


        if (currentChapterIndex < 0 ||
            currentChapterIndex >=
            chapters.Length)
        {
            return "";
        }


        return chapters[
            currentChapterIndex
        ].title;
    }


    // =========================================================
    // GET CURRENT CHAPTER TIME
    //
    // Podcast Experience 使用。
    // =========================================================

    public string GetCurrentChapterTime()
    {
        if (!hasSelection ||
            chapters == null ||
            chapters.Length == 0)
        {
            return "";
        }


        if (currentChapterIndex < 0 ||
            currentChapterIndex >=
            chapters.Length)
        {
            return "";
        }


        return chapters[
            currentChapterIndex
        ].time;
    }
}