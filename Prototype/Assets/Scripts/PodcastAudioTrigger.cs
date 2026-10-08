using UnityEngine;

public class PodcastAudioTrigger : MonoBehaviour
{
    // =====================================================
    // PODCAST AUDIO
    // =====================================================

    [Header("Podcast Audio")]
    [SerializeField]
    private AudioSource podcastAudio;

    // =====================================================
    // HEART RESET
    // 玩家重新进入 Experience 时恢复 Heart
    // =====================================================

    [Header("Heart Reset")]
    [SerializeField]
    private HeartDrag heartDrag;

    // =====================================================
    // PLAYER STATE
    // 防止玩家在同一次进入时重复触发
    // =====================================================

    private bool isPlayerInside = false;

    // =====================================================
    // ENTER PODCAST EXPERIENCE
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        // 只识别 XR Player
        if (!IsPlayer(other))
            return;

        // 防止重复触发
        if (isPlayerInside)
            return;

        isPlayerInside = true;

        // 重新进入 Experience，恢复 Heart
        if (heartDrag != null)
        {
            heartDrag.ResetHeart();

            Debug.Log(
                "PODCAST EXPERIENCE → HEART RESET"
            );
        }
        else
        {
            Debug.LogWarning(
                "PodcastAudioTrigger: HeartDrag is missing."
            );
        }

        // 保留原来的音频播放功能
        if (podcastAudio != null)
        {
            podcastAudio.Play();

            Debug.Log(
                "PODCAST EXPERIENCE → AUDIO STARTED"
            );
        }
        else
        {
            Debug.LogWarning(
                "PodcastAudioTrigger: Podcast Audio is missing."
            );
        }
    }

    // =====================================================
    // EXIT PODCAST EXPERIENCE
    // =====================================================

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        isPlayerInside = false;

        if (podcastAudio != null)
        {
            podcastAudio.Stop();

            Debug.Log(
                "PODCAST EXPERIENCE → AUDIO STOPPED"
            );
        }
    }

    // =====================================================
    // CHECK XR PLAYER
    // =====================================================

    private bool IsPlayer(Collider other)
    {
        return
            other.GetComponent<CharacterController>() != null ||
            other.CompareTag("Player");
    }
}