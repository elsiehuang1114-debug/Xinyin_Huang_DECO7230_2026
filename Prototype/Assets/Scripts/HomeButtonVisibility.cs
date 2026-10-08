using UnityEngine;

public class HomeButtonVisibility : MonoBehaviour
{
    // =====================================================
    // XR CAMERA
    // 使用 Main Camera 的实际世界坐标判断玩家位置
    // =====================================================

    [Header("XR Camera")]
    [SerializeField] private Transform xrCamera;

    // =====================================================
    // HOME BUTTON
    // 只隐藏视觉和交互，不关闭跟随脚本
    // =====================================================

    [Header("Home Button")]
    [SerializeField] private GameObject homeVisual;

    // =====================================================
    // PODCAST HUB AREA
    // 根据目前 Hub 地面的范围设置
    // =====================================================

    [Header("Podcast Hub Bounds")]
    [SerializeField] private Vector2 hubXRange =
        new Vector2(-3f, 3f);

    [SerializeField] private Vector2 hubZRange =
        new Vector2(-3f, 3f);

    // =====================================================
    // UPDATE
    // 每帧检查玩家是否在 Podcast Hub
    // =====================================================

    private void Update()
    {
        if (xrCamera == null || homeVisual == null)
            return;

        Vector3 cameraPosition = xrCamera.position;

        bool insideHub =
            cameraPosition.x >= hubXRange.x &&
            cameraPosition.x <= hubXRange.y &&
            cameraPosition.z >= hubZRange.x &&
            cameraPosition.z <= hubZRange.y;

        // Hub：隐藏
        // 其他房间：显示
        bool shouldShow = !insideHub;

        if (homeVisual.activeSelf != shouldShow)
        {
            homeVisual.SetActive(shouldShow);
        }
    }
}