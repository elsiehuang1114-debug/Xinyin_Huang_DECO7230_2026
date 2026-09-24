using UnityEngine;
using TMPro;

public class EpisodeManager : MonoBehaviour
{
    [System.Serializable]
    public class EpisodeData
    {
        public string title;
        public string duration;
        public Material coverMaterial;
    }

    [Header("Episode Data")]
    public EpisodeData[] episodes;

    [Header("Current Episode UI")]
    public TMP_Text titleText;
    public TMP_Text durationText;
    public Renderer coverRenderer;

    [Header("XR Portal")]
    public Transform xrOrigin;
    public Transform xrCamera;
    public Transform chapterTarget;

    [Header("Navigation")]
    public GameObject navigationPanel;

    private int currentIndex = 0;

    public System.Action OnEpisodeChanged;

    public int CurrentIndex => currentIndex;

    public int EpisodeCount =>
        episodes == null ? 0 : episodes.Length;

    public EpisodeData GetEpisode(int i)
    {
        if (episodes == null || episodes.Length == 0)
            return null;

        int idx =
            ((i % episodes.Length) + episodes.Length)
            % episodes.Length;

        return episodes[idx];
    }

    private void Start()
    {
        ShowEpisode();
    }

    public void NextEpisode()
    {
        if (episodes == null || episodes.Length == 0)
            return;

        currentIndex++;

        if (currentIndex >= episodes.Length)
            currentIndex = 0;

        ShowEpisode();
    }

    public void PreviousEpisode()
    {
        if (episodes == null || episodes.Length == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = episodes.Length - 1;

        ShowEpisode();
    }

    public void SelectCurrentEpisode()
    {
        if (episodes == null || episodes.Length == 0)
            return;

        Debug.Log(
            "Selected Episode: " +
            episodes[currentIndex].title
        );

        TeleportXRPlayerToChapter();

        if (navigationPanel != null)
            navigationPanel.SetActive(true);
    }

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

        // ==========================================
        // STEP 1
        // Match the player's horizontal view
        // direction with the Chapter target.
        // ==========================================

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

        // ==========================================
        // STEP 2
        // Camera position may have changed after
        // rotation. Read it again.
        // ==========================================

        Vector3 cameraPosition =
            xrCamera.position;

        // ==========================================
        // STEP 3
        // Calculate horizontal correction needed
        // to put the player's head at the
        // ChapterTeleportTarget.
        // ==========================================

        Vector3 correction =
            chapterTarget.position -
            cameraPosition;

        correction.y = 0f;

        // ==========================================
        // STEP 4
        // Move the whole XR Origin.
        //
        // IMPORTANT:
        // Do NOT disable CharacterController.
        // This avoids the Step Offset /
        // inactive controller errors.
        // ==========================================

        xrOrigin.position += correction;

        Debug.Log(
            "EPISODE TELEPORT | " +
            "Target = " + chapterTarget.position +
            " | Camera = " + xrCamera.position +
            " | XR Origin = " + xrOrigin.position
        );
    }

    private void ShowEpisode()
    {
        if (episodes == null || episodes.Length == 0)
            return;

        EpisodeData episode =
            episodes[currentIndex];

        if (titleText != null)
            titleText.text = episode.title;

        if (durationText != null)
            durationText.text = episode.duration;

        if (coverRenderer != null &&
            episode.coverMaterial != null)
        {
            coverRenderer.material =
                episode.coverMaterial;
        }

        OnEpisodeChanged?.Invoke();
    }
}