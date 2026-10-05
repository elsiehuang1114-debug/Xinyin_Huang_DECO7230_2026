using UnityEngine;

// =============================================================
// TOPIC SELECTOR
//
// Trigger = Select Topic
// Grip    = Open Door
//
// AI / Emotional:
// 直接进入对应 Dataset
//
// Like List:
// 没 Favourite → 提示 + Door Locked
// 有 Favourite → Favourite Dataset + Door Unlocked
// =============================================================

public class TopicSelector : MonoBehaviour
{
    // =========================================================
    // EPISODE MANAGER
    // =========================================================

    [Header("Episode Manager")]
    public EpisodeManager episodeManager;


    // =========================================================
    // PODCAST DOOR
    // =========================================================

    [Header("Podcast Door")]
    public XRDoorInteractable podcastDoor;


    // =========================================================
    // TOPIC RENDERERS
    // =========================================================

    [Header("Topic Renderers")]
    public Renderer aiRenderer;
    public Renderer emotionalRenderer;
    public Renderer likeListRenderer;


    // =========================================================
    // MATERIALS
    // =========================================================

    [Header("Topic Materials")]
    public Material normalMaterial;
    public Material selectedMaterial;


    // =========================================================
    // LIKE LIST MESSAGE
    //
    // 这个 GameObject 是：
    // "No saved episodes yet
    //  Save an episode first."
    // =========================================================

    [Header("Like List Feedback")]
    public GameObject likeListMessage;


    // =========================================================
    // TOPIC TYPE
    // =========================================================

    private enum Topic
    {
        AI,
        Emotional,
        LikeList
    }

    private Topic currentTopic =
        Topic.AI;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SelectAI();
    }


    // =========================================================
    // SELECT AI
    // =========================================================

    public void SelectAI()
    {
        currentTopic =
            Topic.AI;


        UpdateVisuals();

        HideLikeListMessage();


        // AI 有 Episode，可以开门
        if (podcastDoor != null)
        {
            podcastDoor.SetCanOpen(true);
        }


        if (episodeManager != null)
        {
            episodeManager.SetAI();
        }
        else
        {
            Debug.LogWarning(
                "TopicSelector: EpisodeManager is missing."
            );
        }


        Debug.Log(
            "TOPIC SELECTED: AI"
        );
    }


    // =========================================================
    // SELECT EMOTIONAL
    // =========================================================

    public void SelectEmotional()
    {
        currentTopic =
            Topic.Emotional;


        UpdateVisuals();

        HideLikeListMessage();


        // Emotional 有 Episode，可以开门
        if (podcastDoor != null)
        {
            podcastDoor.SetCanOpen(true);
        }


        if (episodeManager != null)
        {
            episodeManager.SetEmotional();
        }
        else
        {
            Debug.LogWarning(
                "TopicSelector: EpisodeManager is missing."
            );
        }


        Debug.Log(
            "TOPIC SELECTED: EMOTIONAL"
        );
    }


    // =========================================================
    // SELECT LIKE LIST
    //
    // 这里现在会真正检查 Favourite。
    // =========================================================

    public void SelectLikeList()
    {
        currentTopic =
            Topic.LikeList;


        // Like List 方块先变绿
        UpdateVisuals();


        // -----------------------------------------------------
        // EpisodeManager 不存在
        // -----------------------------------------------------

        if (episodeManager == null)
        {
            Debug.LogWarning(
                "TopicSelector: EpisodeManager is missing."
            );

            ShowLikeListMessage();

            if (podcastDoor != null)
            {
                podcastDoor.SetCanOpen(false);
            }

            return;
        }


        // -----------------------------------------------------
        // CASE 1:
        // 没有 Favourite
        // -----------------------------------------------------

        if (!episodeManager.HasFavourite)
        {
            ShowLikeListMessage();


            // 空 Like List 不允许开门
            if (podcastDoor != null)
            {
                podcastDoor.SetCanOpen(false);
            }


            Debug.Log(
                "LIKE LIST EMPTY: No saved episodes yet."
            );

            return;
        }


        // -----------------------------------------------------
        // CASE 2:
        // 已经有 Favourite
        // -----------------------------------------------------

        HideLikeListMessage();


        // Episode Room 切换到 Favourite Dataset
        episodeManager.SetLikeList();


        // Favourite 存在，所以允许开门
        if (podcastDoor != null)
        {
            podcastDoor.SetCanOpen(true);
        }


        Debug.Log(
            "TOPIC SELECTED: LIKE LIST"
        );


        Debug.Log(
            "LIKE LIST READY: " +
            episodeManager
                .FavouriteEpisode
                .title
        );
    }


    // =========================================================
    // UPDATE VISUALS
    // =========================================================

    private void UpdateVisuals()
    {
        if (aiRenderer != null)
        {
            aiRenderer.material =
                currentTopic == Topic.AI
                ? selectedMaterial
                : normalMaterial;
        }


        if (emotionalRenderer != null)
        {
            emotionalRenderer.material =
                currentTopic == Topic.Emotional
                ? selectedMaterial
                : normalMaterial;
        }


        if (likeListRenderer != null)
        {
            likeListRenderer.material =
                currentTopic == Topic.LikeList
                ? selectedMaterial
                : normalMaterial;
        }
    }


    // =========================================================
    // LIKE LIST MESSAGE
    // =========================================================

    private void ShowLikeListMessage()
    {
        if (likeListMessage != null)
        {
            likeListMessage.SetActive(true);
        }
    }


    private void HideLikeListMessage()
    {
        if (likeListMessage != null)
        {
            likeListMessage.SetActive(false);
        }
    }
}