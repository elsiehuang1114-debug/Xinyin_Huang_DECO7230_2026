using UnityEngine;
using TMPro;

public class PodcastExperienceDisplay : MonoBehaviour
{
    // =========================================================
    // DATA REFERENCES
    //
    // 只读取现有 Manager 的数据。
    // 不修改 Episode / Chapter 数据。
    // =========================================================

    [Header("Data References")]

    [SerializeField]
    private EpisodeManager episodeManager;

    [SerializeField]
    private ChapterManager chapterManager;


    // =========================================================
    // EXPERIENCE DISPLAY REFERENCES
    //
    // 只修改：
    // - PodcastCover 的 Material
    // - 三个 TMP Text 的文字内容
    //
    // 不修改：
    // Font
    // Font Asset
    // Font Size
    // Color
    // Alignment
    // Transform
    // =========================================================

    [Header("Experience Display")]

    [SerializeField]
    private Renderer podcastCover;

    [SerializeField]
    private TMP_Text experienceTitle;

    [SerializeField]
    private TMP_Text chapterValue;

    [SerializeField]
    private TMP_Text timeRange;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // 如果进入 Play Mode 时已经有选择，
        // 尝试同步一次。
        RefreshDisplay();
    }


    // =========================================================
    // ENABLE / DISABLE
    //
    // 监听 ChapterManager 的 Chapter Selection。
    // =========================================================

    private void OnEnable()
    {
        if (chapterManager != null)
        {
            chapterManager.OnChapterSelected +=
                RefreshDisplay;
        }
    }


    private void OnDisable()
    {
        if (chapterManager != null)
        {
            chapterManager.OnChapterSelected -=
                RefreshDisplay;
        }
    }


    // =========================================================
    // REFRESH EXPERIENCE DISPLAY
    //
    // 当用户在 Chapter Portal 选择 Chapter 时，
    // 自动更新 Podcast Experience。
    //
    // 用户之后自己打开 Current Chapter Door，
    // 再走进 Podcast Experience。
    //
    // 这个脚本：
    // 不开门
    // 不 Teleport
    // 不移动 XR Origin
    // 不控制 Audio
    // =========================================================

    public void RefreshDisplay()
    {
        // -----------------------------------------------------
        // 检查 Manager
        // -----------------------------------------------------

        if (episodeManager == null)
        {
            Debug.LogWarning(
                "PodcastExperienceDisplay: " +
                "EpisodeManager is missing."
            );

            return;
        }


        if (chapterManager == null)
        {
            Debug.LogWarning(
                "PodcastExperienceDisplay: " +
                "ChapterManager is missing."
            );

            return;
        }


        // -----------------------------------------------------
        // 获取 Selected Episode
        // -----------------------------------------------------

        EpisodeManager.EpisodeData episode =
            episodeManager.SelectedEpisode;


        if (episode == null)
        {
            // 游戏刚开始时还没有选择 Episode，
            // 不覆盖 Inspector 中原本的显示。
            return;
        }


        // -----------------------------------------------------
        // EPISODE COVER
        //
        // 只替换 PodcastCover Renderer 的 Material。
        // -----------------------------------------------------

        if (podcastCover != null &&
            episode.coverMaterial != null)
        {
            podcastCover.material =
                episode.coverMaterial;
        }


        // -----------------------------------------------------
        // EPISODE TITLE
        //
        // 只修改 .text，
        // 不修改 TMP 字体或任何 Style。
        // -----------------------------------------------------

        if (experienceTitle != null)
        {
            experienceTitle.text =
                episode.title;
        }


        // -----------------------------------------------------
        // CHAPTER TITLE + TIME
        //
        // 只有用户真的选择 Chapter 后才更新。
        // -----------------------------------------------------

        if (chapterManager.HasSelection)
        {
            if (chapterValue != null)
            {
                chapterValue.text =
                    chapterManager
                        .GetCurrentChapterTitle();
            }


            if (timeRange != null)
            {
                timeRange.text =
                    chapterManager
                        .GetCurrentChapterTime();
            }
        }


        Debug.Log(
            "PODCAST EXPERIENCE DISPLAY UPDATED | " +
            "Episode = " +
            episode.title +
            " | Chapter = " +
            chapterManager
                .GetCurrentChapterTitle() +
            " | Time = " +
            chapterManager
                .GetCurrentChapterTime()
        );
    }
}