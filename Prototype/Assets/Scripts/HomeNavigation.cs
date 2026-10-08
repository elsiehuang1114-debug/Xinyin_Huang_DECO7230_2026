using UnityEngine;

public class HomeNavigation : MonoBehaviour
{
    // =========================================================
    // XR PLAYER
    // =========================================================

    [Header("XR Player")]
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform xrCamera;

    // =========================================================
    // HOME TARGET
    // =========================================================

    [Header("Home Target")]
    [SerializeField] private Transform hubTeleportTarget;

    // =========================================================
    // PODCAST AUDIO
    // =========================================================

    [Header("Podcast Audio")]
    [SerializeField] private AudioSource podcastAudio;

    // =========================================================
    // HOME NAVIGATION
    //
    // Trigger 点击 HomeButton 时调用。
    // 返回 Podcast Hub。
    //
    // 不禁用 CharacterController。
    // 不修改 XR Input Actions。
    // 不影响 Episode / Chapter 数据。
    // =========================================================

    public void GoHome()
    {
        if (xrOrigin == null ||
            xrCamera == null ||
            hubTeleportTarget == null)
        {
            Debug.LogWarning(
                "HomeNavigation: XR references are missing."
            );

            return;
        }

        // -----------------------------------------------------
        // STEP 1
        // 停止 Podcast Audio
        // -----------------------------------------------------

        if (podcastAudio != null)
        {
            podcastAudio.Stop();
        }

        // -----------------------------------------------------
        // STEP 2
        // 对齐玩家 Camera 和 HubTeleportTarget 的水平朝向
        // -----------------------------------------------------

        Vector3 cameraForward = xrCamera.forward;
        cameraForward.y = 0f;

        Vector3 targetForward = hubTeleportTarget.forward;
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

        // -----------------------------------------------------
        // STEP 3
        // 对齐 Camera 和 HubTeleportTarget 的水平位置
        // -----------------------------------------------------

        Vector3 correction =
            hubTeleportTarget.position - xrCamera.position;

        // 保留 XR Floor Tracking 高度
        correction.y = 0f;

        xrOrigin.position += correction;

        Debug.Log("HOME NAVIGATION → PODCAST HUB");
    }
}