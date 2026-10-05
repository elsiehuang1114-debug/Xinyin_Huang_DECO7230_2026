using UnityEngine;
using TMPro;

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
    }


    // =========================================================
    // EPISODE CATEGORY
    //
    // 现在有三个浏览模式：
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
    // =========================================================

    [Header("Current Episode UI")]
    public TMP_Text titleText;
    public TMP_Text durationText;
    public Renderer coverRenderer;


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
    // CURRENT BROWSING STATE
    // =========================================================

    private int currentIndex = 0;

    public System.Action OnEpisodeChanged;

    public int CurrentIndex => currentIndex;


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
    // FAVOURITE EPISODE
    //
    // 当前 Prototype 先保存一个 Favourite。
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
    // 因为 Prototype 当前只支持一个 Favourite，
    // 所以这里动态建立一个只有一集的 Array。
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
    // 根据当前浏览模式决定 Episode Room 显示什么。
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
    // 只有存在 Favourite 时才允许切换。
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


    private void SetCategory(
        EpisodeCategory category)
    {
        currentCategory = category;

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


        selectedEpisode =
            episodes[currentIndex];


        // -----------------------------------------------------
        // 如果从 Like List 选择 Episode，
        // 保留这个 Favourite 原本的 Category。
        //
        // 例如：
        // Slow Down 原本属于 Emotional，
        // 不应该把它记录成 LikeList Category。
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


        TeleportXRPlayerToChapter();


        if (navigationPanel != null)
        {
            navigationPanel.SetActive(true);
        }
    }


    // =========================================================
    // SAVE FAVOURITE
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
    // 保留现有成功逻辑
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


        Vector3 cameraForward =
            xrCamera.forward;

        cameraForward.y = 0f;


        Vector3 targetForward =
            chapterTarget.forward;

        targetForward.y = 0f;


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


        Vector3 cameraPosition =
            xrCamera.position;


        Vector3 correction =
            chapterTarget.position -
            cameraPosition;

        correction.y = 0f;


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


        if (currentIndex < 0 ||
            currentIndex >= episodes.Length)
        {
            currentIndex = 0;
        }


        EpisodeData episode =
            episodes[currentIndex];


        if (titleText != null)
        {
            titleText.text =
                episode.title;
        }


        if (durationText != null)
        {
            durationText.text =
                episode.duration;
        }


        if (coverRenderer != null &&
            episode.coverMaterial != null)
        {
            coverRenderer.material =
                episode.coverMaterial;
        }


        OnEpisodeChanged?.Invoke();
    }
}