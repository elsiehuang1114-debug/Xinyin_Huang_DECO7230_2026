using UnityEngine;

public class PodcastAudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource podcastAudio;

    private bool hasStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasStarted)
            return;

        // Accept the XR player's CharacterController
        if (other.GetComponent<CharacterController>() != null ||
            other.CompareTag("Player"))
        {
            hasStarted = true;

            if (podcastAudio != null)
            {
                podcastAudio.Play();
                Debug.Log("Podcast audio started.");
            }
        }
    }
}