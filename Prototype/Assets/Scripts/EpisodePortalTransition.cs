using System.Collections;
using UnityEngine;

public class EpisodePortalTransition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EpisodeManager episodeManager;
    [SerializeField] private Transform episodeCard;

    [Header("Transition")]
    [SerializeField] private float targetScaleMultiplier = 2.5f;
    [SerializeField] private float transitionDuration = 0.45f;

    private Vector3 originalScale;
    private bool isTransitioning;

    private void Awake()
    {
        if (episodeCard != null)
        {
            originalScale = episodeCard.localScale;
        }
    }

    public void PlayTransition()
    {
        if (isTransitioning || episodeCard == null)
            return;

        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        Vector3 startScale = episodeCard.localScale;
        Vector3 targetScale =
            originalScale * targetScaleMultiplier;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / transitionDuration
            );

            // Smooth acceleration/deceleration
            t = t * t * (3f - 2f * t);

            episodeCard.localScale =
                Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        episodeCard.localScale = targetScale;

        if (episodeManager != null)
        {
            episodeManager.SelectCurrentEpisode();
        }

        // Reset for when the player returns later.
        episodeCard.localScale = originalScale;
        isTransitioning = false;
    }
}