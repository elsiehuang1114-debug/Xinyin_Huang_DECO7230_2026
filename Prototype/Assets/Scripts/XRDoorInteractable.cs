using UnityEngine;

public class XRDoorInteractable : MonoBehaviour
{
    // =========================================================
    // DOOR CONTROLLER
    // 真正负责门的打开 / 关闭动画
    // =========================================================

    [SerializeField]
    private DoorController doorController;


    // =========================================================
    // CAN OPEN
    //
    // true  = 当前 Topic 允许进入，可以打开门
    // false = 当前 Topic 不允许进入，例如空的 Like List
    //
    // 默认 true，因为游戏开始默认选择 AI
    // =========================================================

    [Header("Door Access")]
    [SerializeField]
    private bool canOpen = true;


    // =========================================================
    // TOGGLE DOOR
    //
    // DoorPanel 或 Topic 方块收到 Grip / SelectEntered 时，
    // 都可以调用这个方法。
    // =========================================================

    public void ToggleDoor()
    {
        // 如果当前 Topic 不允许进入，就不打开门
        if (!canOpen)
        {
            Debug.Log(
                "PODCAST DOOR LOCKED: Current topic has no available episodes."
            );

            return;
        }

        // 检查 DoorController Reference
        if (doorController == null)
        {
            Debug.LogWarning(
                "XRDoorInteractable: DoorController is not assigned."
            );

            return;
        }

        // 打开 / 关闭门
        doorController.ToggleDoor();
    }


    // =========================================================
    // SET DOOR ACCESS
    //
    // TopicSelector 会调用这个方法，
    // 决定当前 Topic 是否允许打开门。
    // =========================================================

    public void SetCanOpen(bool value)
    {
        canOpen = value;

        Debug.Log(
            "PODCAST DOOR ACCESS: " +
            (canOpen ? "OPEN ENABLED" : "LOCKED")
        );
    }
}