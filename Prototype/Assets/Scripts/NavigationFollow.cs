using UnityEngine;

public class NavigationFollow : MonoBehaviour
{
    // =========================================================
    // CAMERA REFERENCE
    // =========================================================

    [Header("XR Camera")]

    [SerializeField]
    private Transform xrCamera;


    // =========================================================
    // POSITION SETTINGS
    //
    // X = 右侧距离
    // Y = 上下距离
    // Z = 前方距离
    //
    // HomeButton 放在玩家视野右下方，
    // 避免挡住 Podcast 内容。
    // =========================================================

    [Header("Position Offset")]

    [SerializeField]
    private Vector3 positionOffset =
        new Vector3(0.45f, -0.25f, 0.85f);


    // =========================================================
    // SMOOTH FOLLOW
    //
    // 数值越大，跟随速度越快。
    // =========================================================

    [Header("Follow Settings")]

    [SerializeField]
    private float positionSmoothSpeed = 4f;

    [SerializeField]
    private float rotationSmoothSpeed = 5f;


    // =========================================================
    // START
    //
    // 游戏开始时先放到正确位置，
    // 避免 HomeButton 从场景原点飞过来。
    // =========================================================

    private void Start()
    {
        if (xrCamera == null)
        {
            Debug.LogWarning(
                "NavigationFollow: XR Camera is missing."
            );

            return;
        }

        transform.position = GetTargetPosition();

        transform.rotation = GetTargetRotation();
    }


    // =========================================================
    // LATE UPDATE
    //
    // Camera 移动以后，
    // HomeButton 平滑跟随。
    // =========================================================

    private void LateUpdate()
    {
        if (xrCamera == null)
            return;


        // -----------------------------------------------------
        // POSITION
        // -----------------------------------------------------

        Vector3 targetPosition =
            GetTargetPosition();


        float positionLerp =
            1f - Mathf.Exp(
                -positionSmoothSpeed * Time.deltaTime
            );


        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                positionLerp
            );


        // -----------------------------------------------------
        // ROTATION
        //
        // 让 HomeButton 面向玩家。
        // -----------------------------------------------------

        Quaternion targetRotation =
            GetTargetRotation();


        float rotationLerp =
            1f - Mathf.Exp(
                -rotationSmoothSpeed * Time.deltaTime
            );


        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationLerp
            );
    }


    // =========================================================
    // TARGET POSITION
    // =========================================================

    private Vector3 GetTargetPosition()
    {
        return
            xrCamera.position
            + xrCamera.right * positionOffset.x
            + xrCamera.up * positionOffset.y
            + xrCamera.forward * positionOffset.z;
    }


    // =========================================================
    // TARGET ROTATION
    //
    // 让按钮的正面朝向 Camera。
    // =========================================================

    private Quaternion GetTargetRotation()
    {
        Vector3 directionToCamera =
            xrCamera.position - transform.position;


        if (directionToCamera.sqrMagnitude <
            0.0001f)
        {
            return transform.rotation;
        }


        return Quaternion.LookRotation(
            -directionToCamera.normalized,
            Vector3.up
        );
    }
}