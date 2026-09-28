using UnityEngine;
using Unity.XR.CoreUtils;

public class ChapterBackButton : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private Camera xrCamera;
    [SerializeField] private Transform episodeTarget;

    public void BackToEpisodes()
    {
        if (xrOrigin == null || xrCamera == null || episodeTarget == null)
            return;

        CharacterController cc = xrOrigin.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        Vector3 cameraOffset =
            xrCamera.transform.position - xrOrigin.transform.position;

        cameraOffset.y = 0;

        xrOrigin.transform.position =
            episodeTarget.position - cameraOffset;

        xrOrigin.transform.rotation = episodeTarget.rotation;

        if (cc != null)
            cc.enabled = true;
    }
}