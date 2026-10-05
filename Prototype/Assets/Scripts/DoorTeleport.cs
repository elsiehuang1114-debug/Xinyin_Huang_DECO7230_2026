using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    // =========================================================
    // DOOR
    // 当前门对应的 DoorController
    // =========================================================

    [Header("Door")]
    public DoorController doorController;


    // =========================================================
    // XR PLAYER
    // xrOrigin = 整个 XR Rig
    // xrCamera = 玩家头部的 Main Camera
    // =========================================================

    [Header("XR Player")]
    public Transform xrOrigin;
    public Transform xrCamera;


    // =========================================================
    // TELEPORT TARGET
    // 玩家穿过门以后要传送到的位置
    // AI Door 和 Emotional Door 可以共用同一个 Target
    // =========================================================

    [Header("Teleport Target")]
    public Transform episodeTarget;


    // 防止 Trigger 在很短时间内重复传送
    private bool hasTeleported = false;


    // =========================================================
    // TRIGGER ENTER
    // 当一个 Collider 进入 InteractionZone 时调用
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        // -----------------------------------------------------
        // 不再使用 Player Tag。
        //
        // XR Origin 本身是 Untagged，
        // 所以这里直接检查进入 Trigger 的 Collider
        // 是否属于我们指定的 XR Origin。
        // -----------------------------------------------------

        if (xrOrigin == null)
            return;

        Transform enteredTransform = other.transform;

        bool isXRPlayer =
            enteredTransform == xrOrigin ||
            enteredTransform.IsChildOf(xrOrigin);

        if (!isXRPlayer)
            return;


        // 防止一次进入 Trigger 重复执行 Teleport
        if (hasTeleported)
            return;


        Debug.Log(
            "XR Player entered Emotional Door InteractionZone"
        );

        TeleportToEpisodeRoom();
    }


    // =========================================================
    // TRIGGER EXIT
    // 离开 Trigger 后允许下一次再次使用
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        if (xrOrigin == null)
            return;

        Transform exitedTransform = other.transform;

        bool isXRPlayer =
            exitedTransform == xrOrigin ||
            exitedTransform.IsChildOf(xrOrigin);

        if (isXRPlayer)
        {
            hasTeleported = false;
        }
    }


    // =========================================================
    // TELEPORT TO SHARED EPISODE ROOM
    // =========================================================

    private void TeleportToEpisodeRoom()
    {
        // 确保所有必要 Reference 都已经在 Inspector 设置
        if (xrOrigin == null ||
            xrCamera == null ||
            episodeTarget == null)
        {
            Debug.LogWarning(
                "DoorTeleport: XR Origin, XR Camera or Episode Target is missing."
            );

            return;
        }


        // -----------------------------------------------------
        // STEP 1
        // 让玩家传送后朝向 EpisodeTeleportTarget 的方向
        // -----------------------------------------------------

        Vector3 cameraForward = xrCamera.forward;
        cameraForward.y = 0f;

        Vector3 targetForward = episodeTarget.forward;
        targetForward.y = 0f;


        if (cameraForward.sqrMagnitude > 0.001f &&
            targetForward.sqrMagnitude > 0.001f)
        {
            float angle = Vector3.SignedAngle(
                cameraForward,
                targetForward,
                Vector3.up
            );

            // 围绕玩家头部旋转整个 XR Rig
            xrOrigin.RotateAround(
                xrCamera.position,
                Vector3.up,
                angle
            );
        }


        // -----------------------------------------------------
        // STEP 2
        // 旋转后重新读取 Camera 的位置
        // -----------------------------------------------------

        Vector3 cameraPosition = xrCamera.position;


        // -----------------------------------------------------
        // STEP 3
        // 计算当前位置到 EpisodeTeleportTarget 的水平距离
        // -----------------------------------------------------

        Vector3 correction =
            episodeTarget.position -
            cameraPosition;

        // XR 使用 Floor Tracking，
        // 所以这里不修改玩家的高度
        correction.y = 0f;


        // -----------------------------------------------------
        // STEP 4
        // 移动整个 XR Origin
        //
        // 不 Disable CharacterController，
        // 避免之前出现过的 Step Offset Error。
        // -----------------------------------------------------

        xrOrigin.position += correction;

        hasTeleported = true;


        Debug.Log(
            "DOOR TELEPORT → EPISODE ROOM | " +
            "Target = " + episodeTarget.position +
            " | Camera = " + xrCamera.position +
            " | XR Origin = " + xrOrigin.position
        );
    }
}