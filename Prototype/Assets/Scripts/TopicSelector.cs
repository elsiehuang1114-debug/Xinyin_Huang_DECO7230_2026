using UnityEngine;

// =============================================================
// TOPIC SELECTOR
// 控制 Podcast Door 上面的 Topic 选择
//
// 功能：
// 1. 选择 AI / Emotional / Like List
// 2. 当前选择的 Topic 变成绿色
// 3. AI / Emotional 会通知 EpisodeManager 切换 Dataset
// 4. Like List 目前只做视觉选择，之后再连接 Favourite 数据
// =============================================================

public class TopicSelector : MonoBehaviour
{
    // =========================================================
    // EPISODE MANAGER
    // 使用现有的 EpisodeManager，不重新建立 Episode Room
    // =========================================================

    [Header("Episode Manager")]
    public EpisodeManager episodeManager;


    // =========================================================
    // TOPIC OBJECTS
    // 三个门上的 Topic 方框
    // Renderer 用来改变它们的 Material
    // =========================================================

    [Header("Topic Renderers")]
    public Renderer aiRenderer;
    public Renderer emotionalRenderer;
    public Renderer likeListRenderer;


    // =========================================================
    // MATERIALS
    // Normal = 没有选择
    // Selected = 当前选择，建议使用绿色
    // =========================================================

    [Header("Topic Materials")]
    public Material normalMaterial;
    public Material selectedMaterial;


    // =========================================================
    // 当前 Topic
    // 默认 AI
    // =========================================================

    private enum Topic
    {
        AI,
        Emotional,
        LikeList
    }

    private Topic currentTopic = Topic.AI;


    // =========================================================
    // START
    // 游戏开始时默认选择 AI
    // =========================================================

    private void Start()
    {
        SelectAI();
    }


    // =========================================================
    // SELECT AI
    // 由 AI_Select 的 XR Simple Interactable 调用
    // =========================================================

    public void SelectAI()
    {
        currentTopic = Topic.AI;

        // 更新门上的绿色选择状态
        UpdateVisuals();

        // 调用我们已经存在的 EpisodeManager AI Dataset
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

        Debug.Log("TOPIC SELECTED: AI");
    }


    // =========================================================
    // SELECT EMOTIONAL
    // 由 Emotional_Select 的 XR Simple Interactable 调用
    // =========================================================

    public void SelectEmotional()
    {
        currentTopic = Topic.Emotional;

        // 更新门上的绿色选择状态
        UpdateVisuals();

        // 调用已经存在的 Emotional Dataset
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

        Debug.Log("TOPIC SELECTED: EMOTIONAL");
    }


    // =========================================================
    // SELECT LIKE LIST
    // 目前先完成选择 + 绿色反馈
    //
    // Favourite Dataset 后面完成以后，
    // 再在这里连接 EpisodeManager。
    // =========================================================

    public void SelectLikeList()
    {
        currentTopic = Topic.LikeList;

        // 更新绿色选择状态
        UpdateVisuals();

        Debug.Log("TOPIC SELECTED: LIKE LIST");

        // 后面 Favourite 系统完成后，
        // 我们会在这里加入：
        //
        // episodeManager.SetFavourites();
    }


    // =========================================================
    // UPDATE VISUALS
    // 每次 Topic 改变以后：
    //
    // 当前 Topic → Selected Material（绿色）
    // 其他 Topic → Normal Material
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
}