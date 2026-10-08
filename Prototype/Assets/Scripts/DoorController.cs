using UnityEngine;

public class DoorController : MonoBehaviour
{
    // =====================================================
    // DOOR SETTINGS
    // 保留原来的开门角度和速度
    // =====================================================

    public float openAngle = -90f;
    public float openSpeed = 3f;

    // =====================================================
    // AUTO CLOSE SETTINGS
    // 新增：自动关门时间
    // =====================================================

    [Header("Auto Close")]
    public bool autoClose = true;

    // 门打开后等待多少秒自动关闭
    public float autoCloseDelay = 5f;

    // =====================================================
    // DOOR STATE
    // =====================================================

    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    // 自动关门计时器
    private float autoCloseTimer = 0f;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // 保存门原来的关闭角度
        closedRotation = transform.localRotation;

        // 计算打开后的角度
        openRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + openAngle,
            transform.localEulerAngles.z
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        // -------------------------------------------------
        // 保留原来的平滑旋转
        // -------------------------------------------------

        Quaternion targetRotation =
            isOpen ? openRotation : closedRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );

        // -------------------------------------------------
        // 新增：自动关门计时
        // -------------------------------------------------

        if (isOpen && autoClose)
        {
            autoCloseTimer += Time.deltaTime;

            if (autoCloseTimer >= autoCloseDelay)
            {
                CloseDoor();

                Debug.Log(
                    "CHAPTER DOOR → AUTO CLOSED"
                );
            }
        }
    }

    // =====================================================
    // TOGGLE DOOR
    //
    // 保留原来的 XR Grip 开关门方式
    // =====================================================

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        // 每次操作门，都重新计算时间
        autoCloseTimer = 0f;

        Debug.Log(
            isOpen
                ? "CHAPTER DOOR → OPENED"
                : "CHAPTER DOOR → CLOSED"
        );
    }

    // =====================================================
    // CLOSE DOOR
    //
    // 可以被其他脚本调用
    // =====================================================

    public void CloseDoor()
    {
        isOpen = false;

        autoCloseTimer = 0f;
    }
}