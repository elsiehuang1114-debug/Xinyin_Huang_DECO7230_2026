using UnityEngine;

public class DoorAutoClose : MonoBehaviour
{
    // =========================================================
    // PODCAST DOOR
    // 负责真正控制门打开 / 关闭的 DoorController
    // =========================================================

    [Header("Podcast Door")]
    public DoorController doorController;


    // =========================================================
    // XR PLAYER
    // 用来确认进入 Trigger 的对象属于 XR Origin
    // =========================================================

    [Header("XR Player")]
    public Transform xrOrigin;


    // =========================================================
    // TRIGGER ENTER
    //
    // 玩家进入 InteractionZone 时：
    // 只关闭 Podcast Door。
    //
    // 这个脚本不会 Teleport 玩家，
    // 不会修改 XR Origin 的位置，
    // 也不会修改 Main Camera。
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        if (xrOrigin == null)
        {
            Debug.LogWarning(
                "DoorAutoClose: XR Origin is missing."
            );

            return;
        }


        // 检查进入 Trigger 的 Collider
        // 是否属于 XR Origin
        Transform enteredTransform = other.transform;

        bool isXRPlayer =
            enteredTransform == xrOrigin ||
            enteredTransform.IsChildOf(xrOrigin);


        // 不是玩家就不执行
        if (!isXRPlayer)
            return;


        // 检查 DoorController
        if (doorController == null)
        {
            Debug.LogWarning(
                "DoorAutoClose: Door Controller is missing."
            );

            return;
        }


        // =====================================================
        // 只关闭门
        // 不做任何 Teleport
        // =====================================================

        doorController.CloseDoor();

        Debug.Log(
            "PODCAST DOOR → AUTO CLOSE"
        );
    }
}