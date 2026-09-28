using UnityEngine;

public class FollowCameraPanel : MonoBehaviour
{
    public Transform cameraTransform;

    public Vector3 positionOffset = new Vector3(1.6f, 0f, 2.5f);

    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;

    void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        Vector3 targetPosition =
            cameraTransform.position
            + cameraTransform.right * positionOffset.x
            + cameraTransform.up * positionOffset.y
            + cameraTransform.forward * positionOffset.z;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * positionSmoothSpeed
        );

        Vector3 directionToCamera =
            cameraTransform.position - transform.position;

        if (directionToCamera != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(directionToCamera);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSmoothSpeed
            );
        }
    }
}