using UnityEngine;
using TMPro;

public class ChapterManager : MonoBehaviour
{
    [System.Serializable]
    public class ChapterData
    {
        public string title;
        public string time;
    }

    public ChapterData[] chapters;

    public TMP_Text chapterTitleText;
    public TMP_Text chapterTimeText;

    [Header("Door Visual State")]
    [Tooltip("Renderer of the CurrentChapterDoor panel to reflect selection state.")]
    [SerializeField] private Renderer doorRenderer;
    [SerializeField] private Color doorInactiveColor = new Color(0.3529f, 0.4196f, 0.4902f, 1f); // #5A6B7D Blue-grey / subdued
    [SerializeField] private Color doorSelectedColor = new Color(0.1804f, 0.8000f, 0.4431f, 1f); // #2ECC71 Bright green matching selected chapter

    private Material doorMaterial;
    private int currentChapterIndex = 0;
    private bool hasSelection = false;

    void Awake()
    {
        if (doorRenderer != null)
        {
            doorMaterial = doorRenderer.material;
            doorMaterial.color = doorInactiveColor;
        }
    }

    void Start()
    {
        // Keep door in inactive subdued blue-grey initially
        if (doorMaterial != null)
        {
            doorMaterial.color = doorInactiveColor;
        }
    }

    public void ShowChapter(int index)
    {
        if (chapters == null || chapters.Length == 0)
            return;

        if (index < 0 || index >= chapters.Length)
            return;

        currentChapterIndex = index;
        hasSelection = true;

        if (chapterTitleText != null)
            chapterTitleText.text = chapters[currentChapterIndex].title;
        if (chapterTimeText != null)
            chapterTimeText.text = chapters[currentChapterIndex].time;

        // Transition door panel to match the selected Chapter Node green
        if (doorMaterial != null)
        {
            doorMaterial.color = doorSelectedColor;
        }

        Debug.Log("Current Chapter: " + chapters[currentChapterIndex].title);
    }

    public int GetCurrentChapterIndex()
    {
        return currentChapterIndex;
    }

    public string GetCurrentChapterTitle()
    {
        if (chapters == null || chapters.Length == 0)
            return "";

        return chapters[currentChapterIndex].title;
    }
}