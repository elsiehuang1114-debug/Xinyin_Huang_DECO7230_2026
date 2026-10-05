using UnityEngine;

public class PodcastAudioTrigger : MonoBehaviour
{
    // =========================================================
    // PODCAST AUDIO
    // Podcast Experience 中负责播放 Podcast 的 AudioSource
    // =========================================================

    [SerializeField]
    private AudioSource podcastAudio;


    // =========================================================
    // PLAYER STATE
    // 防止同一次进入 Trigger 时重复播放
    // =========================================================

    private bool isPlayerInside = false;


    // =========================================================
    // ENTER PODCAST EXPERIENCE
    //
    // 玩家进入 PodcastAudioTrigger：
    // 开始播放 Podcast Audio
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        // 只识别 XR Player
        if (!IsPlayer(other))
            return;


        // 防止同一次进入重复触发
        if (isPlayerInside)
            return;


        isPlayerInside = true;


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


    // =========================================================
    // EXIT PODCAST EXPERIENCE
    //
    // 玩家离开 PodcastAudioTrigger：
    // 立即停止 Podcast Audio
    //
    // 不执行任何 Teleport。
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        // 只识别 XR Player
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


    // =========================================================
    // CHECK XR PLAYER
    //
    // 保留你原本已经成功工作的 Player 判断方式
    // =========================================================

    private bool IsPlayer(Collider other)
    {
        return
            other.GetComponent<CharacterController>() != null ||
            other.CompareTag("Player");
    }
}