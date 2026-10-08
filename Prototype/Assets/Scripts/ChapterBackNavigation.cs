using UnityEngine;

public class ChapterBackNavigation : MonoBehaviour
{
    // =====================================================
    // XR PLAYER
    // =====================================================

    [Header("XR Player")]
    [SerializeField] private Transform xrOrigin;

    [SerializeField] private Transform xrCamera;


    // =====================================================
    // EPISODE ROOM TARGET
    // 使用 Unity 场景中已有的 EpisodeTeleportTarget
    // =====================================================

    [Header("Episode Room Target")]
    [SerializeField] private Transform episodeTeleportTarget;


    // =====================================================
    // BACK TO EPISODES
    //
    // Trigger 点击 BackButton 时调用
    //
    // 不禁用 CharacterController
    // 不修改 XR Input Actions
    // 不影响 ChapterManager / EpisodeManager
    // =====================================================

    public void BackToEpisodes()
    {
        // 检查必要的对象是否已经连接
        if (xrOrigin == null ||
            xrCamera == null ||
            episodeTeleportTarget == null)
        {
            Debug.LogWarning(
                "ChapterBackNavigation: Missing XR references."
            );

            return;
        }

        // -------------------------------------------------
        // STEP 1：对齐 Camera 的水平朝向
        // -------------------------------------------------

        Vector3 cameraForward = xrCamera.forward;
        cameraForward.y = 0f;

        Vector3 targetForward = episodeTeleportTarget.forward;
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

        // -------------------------------------------------
        // STEP 2：对齐 Camera 和 EpisodeTeleportTarget
        // 的水平位置
        // -------------------------------------------------

        Vector3 correction =
            episodeTeleportTarget.position - xrCamera.position;

        // 保留 XR Floor Tracking 高度
        correction.y = 0f;

        xrOrigin.position += correction;

        Debug.Log("CHAPTER BACK → EPISODE ROOM");
    }
}