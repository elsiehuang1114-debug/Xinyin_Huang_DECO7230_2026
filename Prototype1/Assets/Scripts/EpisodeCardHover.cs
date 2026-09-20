using UnityEngine;

public class EpisodeCardHover : MonoBehaviour
{
    [SerializeField] private Transform card;
    [SerializeField] private float hoverScaleMultiplier = 1.05f;

    private Vector3 normalScale;

    private void Awake()
    {
        if (card != null)
            normalScale = card.localScale;
    }

    public void HoverEnter()
    {
        if (card != null)
            card.localScale =
                normalScale * hoverScaleMultiplier;
    }

    public void HoverExit()
    {
        if (card != null)
            card.localScale = normalScale;
    }
}