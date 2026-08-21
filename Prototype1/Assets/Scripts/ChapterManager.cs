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

    private int currentChapterIndex = 0;

    void Start()
    {
        ShowChapter(0);
    }

    public void ShowChapter(int index)
    {
        if (chapters == null || chapters.Length == 0)
            return;

        if (index < 0 || index >= chapters.Length)
            return;

        currentChapterIndex = index;

        chapterTitleText.text = chapters[currentChapterIndex].title;
        chapterTimeText.text = chapters[currentChapterIndex].time;

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