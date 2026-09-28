using UnityEngine;

public class ChapterButton : MonoBehaviour
{
    public ChapterManager chapterManager;
    public int chapterIndex;

    private void OnMouseDown()
    {
        if (chapterManager != null)
        {
            chapterManager.ShowChapter(chapterIndex);
        }
    }
}