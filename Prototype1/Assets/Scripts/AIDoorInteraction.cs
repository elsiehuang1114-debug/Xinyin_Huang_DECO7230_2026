using UnityEngine;
using UnityEngine.InputSystem;

public class AIDoorInteraction : MonoBehaviour
{
    public Transform doorPivot;
    public float openAngle = 90f;
    public float openSpeed = 3f;

    private bool playerNearby = false;
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = doorPivot.localRotation;

        openRotation =
            closedRotation *
            Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        if (playerNearby &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            isOpen = true;
        }

        if (isOpen)
        {
            doorPivot.localRotation =
                Quaternion.Slerp(
                    doorPivot.localRotation,
                    openRotation,
                    openSpeed * Time.deltaTime
                );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}